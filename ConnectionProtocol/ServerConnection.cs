using System;
using System.Net.Sockets;

namespace ConnectionProtocol
{
    /// <summary>
    /// Принимающее(серверное) соединение
    /// </summary>
    public class ServerConnection : Connection
    {
        /// <param name="client">Клиентское подключение</param>
        /// <param name="opitions">Настройки подключения</param>
        public ServerConnection(TcpClient client, ConnectionOpitions opitions) : base(opitions)
        {
            if ((connection = client) == null) throw new ArgumentNullException(nameof(client));
            OnConnect();
            Send(opitions);
        }
    }
}