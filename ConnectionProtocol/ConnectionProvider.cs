using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using AuxiliaryLibrary;

namespace ConnectionProtocol
{
    /// <summary>
    /// Считывает новые входящие соединения
    /// </summary>
    public class ConnectionProvider
    {
        TcpListener listener;
        bool allowNewConnections = false;
        Task listening;

        ConnectionOpitions connectionOpitions;
        public ConnectionOpitions ConnectionOpitions
        {
            get { return connectionOpitions; }
            set
            {
                if ((connectionOpitions = value) == null)
                    throw new ArgumentNullException(nameof(value));
            }
        }

        /// <summary>
        /// Разрешается ли создавать новые соединения
        /// </summary>
        public bool AllowNewConnections
        {
            get { return allowNewConnections; }
            set
            {
                if (allowNewConnections = value) StartListening();
            }
        }

        /// <summary>
        /// Порт, на котором ведется прослушивание
        /// </summary>
        public int Port { get; }
        /// <summary>
        /// Локальный адрес прослушивания
        /// </summary>
        public IPAddress LocalAddr { get; }

        int maxConnections;
        /// <summary>
        /// Максимальное количество соединений
        /// </summary>
        public int MaxConnections
        {
            get => maxConnections;
            set
            {
                if (value < 1) throw new ArgumentException("Max connections value must be more than zero");
                if (value > maxConnections
                    && (listening == null || listening.IsCompleted))
                    StartListening();
                maxConnections = value;
            }
        }

        int connectionsCount;
        /// <summary>
        /// Количество соединений
        /// </summary>
        public int ConnectionsCount { get => connectionsCount; }

        /// <param name="localaddr">Локальный адрес прослушивания</param>
        /// <param name="port">Порт, на котором ведется прослушивание</param>
        /// <param name="maxConnections">Максимальное количество соединений</param>
        /// <param name="connReceiveBufferSize">Размер буфера приема для соединений</param>
        /// <param name="connSendBufferSize">Размер буфера отправки для соединений</param>
        /// <param name="streamingAvialable">Разрешена ли функция стриминга для соединений</param>
        public ConnectionProvider(IPAddress localaddr, int port, int maxConnections, ConnectionOpitions connectionOpitions)
        {
            try
            {
                listener = new TcpListener(IPAddress.Any, port);
                ConnectionOpitions = connectionOpitions;
            }
            catch { throw; }
            if ((MaxConnections = maxConnections) < 1) throw new ArgumentException("Wrong max connections count");
        }

        void Listen()
        {
            listener.Start();
            while (allowNewConnections)
            {
                if (ConnectionsCount == MaxConnections)
                {
                    if (OnMaxConnections != null)
                        OnMaxConnections.BeginInvoke(this, null, null);
                    listener.Stop();
                    return;
                }
                else
                {
                    OnConnected(new ServerConnection(listener.AcceptTcpClient(), ConnectionOpitions));
                }
            }
        }
        protected void StartListening()
        {
            if (listening == null || listening.Status == TaskStatus.RanToCompletion)
                (listening = new Task(new Action(Listen))).Start();
        }

        void OnDisconnected(Connection connection)
        {
            Interlocked.Decrement(ref connectionsCount);
            if (OnConnectionsCountChanged != null)
                OnConnectionsCountChanged.BeginInvoke(this, connectionsCount, null, null);
        }
        void OnConnected(ServerConnection connection)
        {
            connection.OnDisconnected += OnDisconnected;
            Interlocked.Increment(ref connectionsCount);
            if (OnIncomingConnection != null)
                OnIncomingConnection.Invoke(this, connection);

            if (OnConnectionsCountChanged != null)
                OnConnectionsCountChanged.BeginInvoke(this, ConnectionsCount, null, null);
        }

        public event ParametrizedEventHandler<ConnectionProvider, ServerConnection> OnIncomingConnection;
        public event NonParametrizedEventHandler<ConnectionProvider> OnMaxConnections;
        public event ParametrizedEventHandler<ConnectionProvider, int> OnConnectionsCountChanged;
    }
}