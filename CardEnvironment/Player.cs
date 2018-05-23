using System.Collections.Generic;
using CardSessionServer;
using System;
using CardSessionShared;

namespace CardEnvironmentServer
{
    public class Player : SessionsController
    {
        public Action<object> Send { get; set; }

        public PlayerInfo PlayerInfo { get; }
        public List<Session> Sessions { get; } = new List<Session>();

        public Player(PlayerInfo ownerInfo)
        {
            if ((PlayerInfo = ownerInfo) == null) throw new ArgumentNullException(nameof(ownerInfo));
            ControllerInfo = PlayerInfo.SharedPlayerInfo;
        }

        protected override void OnSessionChanged(Session session, SessionChange change)
        { Send.Invoke(Interpretor.Interpretate(session)); }
    }
}