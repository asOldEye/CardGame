using System.Net;
using System.Net.Sockets;

namespace ConnectionProtocol
{
    /// <summary>
    /// Клиент - соединение
    /// </summary>
    public class ClientConnection : Connection
    {
        public ClientConnection(bool eventOriented, int bufferSize)
            : base(new ConnectionOpitions(eventOriented, ConnectionOpitions.Default.AverageDisconnectAvait, ConnectionOpitions.Default.MaxDisconnectAvait, bufferSize))
        { connection = new TcpClient(); }

        /// <summary>
        /// Установить соединение с конечной точкой
        /// </summary>
        /// <param name="address">Адрес конечной точки</param>
        /// <param name="port">Порт конечной точки</param>
        public void Connect(IPAddress address, int port)
        {
            try
            { connection.Connect(address, port); }
            catch { throw; }
            OnConnectionOpitionsReceived += ConnectionOpitionsReceived;
            OnConnect();
        }
        /// <summary>
        /// Установить соединение с конечной точкой
        /// </summary>
        /// <param name="address">Адрес конечной точки</param>
        /// <param name="port">Порт конечной точки</param>
        /// <returns></returns>
        public void Connect(string address, int port)
        {
            try
            { connection = new TcpClient(address, port); }
            catch { throw; }
            OnConnectionOpitionsReceived += ConnectionOpitionsReceived;
            OnConnect();
        }
        void ConnectionOpitionsReceived(Connection sender, ConnectionOpitions opitions)
        { Opitions.SetDisconnectAvait(opitions.MaxDisconnectAvait, opitions.AverageDisconnectAvait); }
    }
}