using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using AuxiliaryLibrary;

namespace ConnectionProtocol
{
    /// <summary>
    /// Соединение с конечной точкой
    /// </summary>
    public abstract class Connection
    {
        #region Timeout disconnection region
        static List<Connection> connectionsToDisconnectAwait = new List<Connection>();
        static Task disconnectAwaitHandler;

        static void DisconnectAwait()
        {
            while (connectionsToDisconnectAwait.Count > 0)
            {
                var currTime = DateTime.Now;
                lock (connectionsToDisconnectAwait)
                    foreach (var f in connectionsToDisconnectAwait.ToArray())
                        if (f != null)
                        {
                            if (currTime - f.Opitions.AverageDisconnectAvait >= f.lastSended)
                                f.Send(StatusByte.timeoutCheck);
                            if (currTime - f.Opitions.MaxDisconnectAvait > f.lastReceived || !f.connection.Connected)
                                f.Disconnect();
                        }
                        else connectionsToDisconnectAwait.Remove(f);
                Task.Delay(500).Wait();
            }
        }
        #endregion
        DateTime lastSended, lastReceived;

        /// <summary>
        /// Соединен ли сейчас с конечной точкой
        /// </summary>
        public bool Connected
        {
            get
            {
                try { return connection.Connected; }
                catch { return false; }
            }
        }

        Queue<object> receivedObjects;
        public Queue<object> ReceivedObjects
        {
            get
            {
                if (Opitions.EventOriented)
                    throw new NotSupportedException("You can't use ReceivedObjects in event oriented connection");
                else return receivedObjects;
            }
        }

        protected TcpClient connection;
        NetworkStream networkStream;

        Task send, receive;

        Queue<Pair<Stream, StatusByte>> toSend = new Queue<Pair<Stream, StatusByte>>();
        Stream receiver;

        protected Connection(ConnectionOpitions opitions)
        {
            if ((Opitions = opitions) == null) throw new ArgumentNullException(nameof(opitions));
            if (!Opitions.EventOriented) receivedObjects = new Queue<object>();
        }

        /// <summary>
        /// Настройки текущего соединения
        /// </summary>
        public ConnectionOpitions Opitions { get; }

        /// <summary>
        /// Отправить поток
        /// </summary>
        /// <param name="stream">Отправляемый поток</param>
        /// <param name="info">Информация о потоке</param>
        public void Send(Stream stream, StreamInfo info)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (!stream.CanRead) throw new ArgumentException("Can't read stream");
            if (info == null) throw new ArgumentNullException(nameof(info));
            lock (toSend)
            {
                toSend.Enqueue(new Pair<Stream, StatusByte>(BinarySerializer.Serialize(info), StatusByte.streamInfo));
                toSend.Enqueue(new Pair<Stream, StatusByte>(stream, StatusByte.data));
            }
            StartSend();
        }
        /// <summary>
        /// Отправить объект, помеченный аттрибутом [Serializable]
        /// </summary>
        /// <param name="obj"></param>
        public void Send(object obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            Stream s;
            try
            { s = BinarySerializer.Serialize(obj); }
            catch (NotSupportedException) { throw; }

            StatusByte statusByte = obj is ConnectionOpitions ? StatusByte.connectionOpitions :
                (obj is StreamInfo ? StatusByte.streamInfo :
                obj is StatusByte ? StatusByte.timeoutCheck : StatusByte.data);

            toSend.Enqueue(new Pair<Stream, StatusByte>(s, statusByte));
            StartSend();
        }
        /// <summary>
        /// Указать поток, в который будет писаться входящий поток
        /// </summary>
        /// <param name="destination"></param>
        public void Receive(Stream destination)
        {
            if (destination == null) throw new ArgumentNullException(nameof(destination));
            if (!destination.CanWrite) throw new ArgumentException("Can't write in destionation stream");
            receiver = destination;
        }

        protected void OnConnect()
        {
            networkStream = connection.GetStream();

            lock (connectionsToDisconnectAwait)
                connectionsToDisconnectAwait.Add(this);
            if (disconnectAwaitHandler == null || disconnectAwaitHandler.IsCompleted)
                (disconnectAwaitHandler = new Task(new Action(DisconnectAwait))).Start();
            lastSended = lastReceived = DateTime.Now;

            StartReceive();

            if (Opitions.EventOriented && OnConnected != null)
                OnConnected.Invoke(this);
        }
        /// <summary>
        /// Отключиться от конечной точки
        /// </summary>
        public void Disconnect()
        {
            bool disconnect;
            lock (connectionsToDisconnectAwait)
                disconnect = connectionsToDisconnectAwait.Remove(this);

            if (disconnect)
                if (Opitions.EventOriented && OnDisconnected != null)
                    OnDisconnected.Invoke(this);

            if (connection != null && connection.Connected)
                connection.Close();
        }

        #region Кишки
        void StartSend()
        {
            if (send == null || send.Status == TaskStatus.RanToCompletion)
                (send = new Task(new Action(Send))).Start();
        }
        void StartReceive()
        {
            if (receive == null ||
                receive.Status == TaskStatus.RanToCompletion ||
                receive.Status == TaskStatus.Canceled ||
                receive.Status == TaskStatus.Faulted)

                (receive = new Task(new Action(Receive))).Start();
        }

        void Receive()
        {
            byte[] receiveBuffer = new byte[connection.ReceiveBufferSize = Opitions.BufferSize],
                head = new byte[9];

            int readed;
            Stream destination = null;
            bool stream = false;

            try
            {
                while (connection.Connected)
                {
                    networkStream.Read(head, 0, head.Length);
                    lastReceived = DateTime.Now;
                    Heading(out long size, out StatusByte statusByte, head);

                    if (stream)
                    {
                        if ((destination = receiver) == null)
                            while (size > 0)
                            {
                                size -= networkStream.Read(receiveBuffer, 0, receiveBuffer.Length);
                                lastReceived = DateTime.Now;
                            }
                    }
                    else destination = new MemoryStream();

                    while (size > 0)
                    {
                        readed = networkStream.Read(receiveBuffer, 0, receiveBuffer.Length);
                        lastReceived = DateTime.Now;
                        destination.Write(receiveBuffer, 0, readed);
                        size -= readed;
                    }

                    if (stream) stream = false;
                    else
                    {
                        object obj = BinarySerializer.Deserialize(destination);
                        switch (statusByte)
                        {
                            case StatusByte.connectionOpitions:
                                if (OnConnectionOpitionsReceived != null)
                                    OnConnectionOpitionsReceived.BeginInvoke(this, obj as ConnectionOpitions, null, null);
                                break;
                            case StatusByte.streamInfo:
                                if (!Opitions.EventOriented)
                                    receivedObjects.Enqueue(obj);
                                else if (OnIncomingStream != null)
                                    OnIncomingStream.Invoke(this, obj as StreamInfo);
                                stream = true;
                                break;
                            case StatusByte.data:
                                if (!Opitions.EventOriented)
                                    receivedObjects.Enqueue(obj);
                                else if (OnObjectReceived != null)
                                    OnObjectReceived.BeginInvoke(this, obj, null, null);
                                break;
                        }
                    }
                }
            }
            catch (IOException e) { if (e.InnerException is SocketException) Disconnect(); }
            catch (System.Runtime.Serialization.SerializationException)
            {
                if (destination.Length == 0) Disconnect();
            }
            Disconnect();
        }
        void Send()
        {
            byte[] sendBuffer = new byte[connection.SendBufferSize = Opitions.BufferSize];
            try
            {
                while (toSend.Count > 0)
                {
                    var source = toSend.Peek();
                    var head = Heading(source.Obj1.Length, source.Obj2);

                    networkStream.Write(head, 0, head.Length);
                    lastSended = DateTime.Now;

                    source.Obj1.Position = 0;

                    while (source.Obj1.Position <= source.Obj1.Length - 1)
                    {
                        int readed = source.Obj1.Read(sendBuffer, 0, sendBuffer.Length);
                        networkStream.Write(sendBuffer, 0, readed);
                        lastSended = DateTime.Now;
                    }
                    toSend.Dequeue();
                }
            }
            catch (IOException e) { if (e.InnerException is SocketException) Disconnect(); }
        }

        byte[] Heading(long size, StatusByte statusByte)
        {
            var arr = new byte[9];
            var s = BitConverter.GetBytes(size);
            Array.Copy(s, arr, s.Length);
            arr[8] = (byte)statusByte;
            return arr;
        }
        void Heading(out long size, out StatusByte statusByte, byte[] heading)
        {
            size = BitConverter.ToInt64(heading, 0);
            statusByte = (StatusByte)heading[8];
        }

        [Serializable]
        enum StatusByte
        {
            streamInfo,
            data,
            connectionOpitions,
            timeoutCheck
        }
        #endregion

        #region IDisposable realization
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    Disconnect();

                    if (receive != null) receive.Dispose();
                    if (send != null) send.Dispose();

                    if (networkStream != null)
                        networkStream.Dispose();
                }
                disposed = true;
            }
        }

        ~Connection()
        { Dispose(false); }
        #endregion

        public event NonParametrizedEventHandler<Connection> OnConnected;
        public event NonParametrizedEventHandler<Connection> OnDisconnected;

        public event ParametrizedEventHandler<Connection, object> OnObjectReceived;
        protected event ParametrizedEventHandler<Connection, ConnectionOpitions> OnConnectionOpitionsReceived;
        public event ParametrizedEventHandler<Connection, StreamInfo> OnIncomingStream;
    }
}