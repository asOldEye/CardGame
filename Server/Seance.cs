using System;
using ConnectionProtocol;
using AuxiliaryLibrary;

namespace Server
{
    class Seance
    {
        API api;
        public Seance(Connection connection, API api)
        {
            this.api = api ?? throw new ArgumentNullException(nameof(api));
            api.sendObject = connection.Send;
            api.sendStream = connection.Send;
            connection.OnObjectReceived += OnReceived;
            connection.OnIncomingStream += OnIncomingStream;
            connection.OnDisconnected += OnDisconnected;
        }

        void OnDisconnected(Connection sender)
        { if (api != null) api.OnDisconnected(); }
        void OnReceived(Connection sender, object obj)
        { api.OnReceived(obj); }
        void OnIncomingStream(Connection sender, StreamInfo info)
        { api.OnIncomingStream(info); }
    }
}