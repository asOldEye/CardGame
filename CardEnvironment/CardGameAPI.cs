using AuxiliaryLibrary;
using CardEnvironmentShared;
using System;
using System.Collections.Generic;

namespace CardEnvironmentServer
{
    public class CardGameAPI : API
    {
        public Supervisor supervisor;
        public Player player = null;

        public CardGameAPI(Supervisor supervisor)
        {
            if ((this.supervisor = supervisor) == null)
                throw new ArgumentNullException(nameof(supervisor));
            InitializeAPICommands(new Dictionary<string, Action<APICommand>>()
            {
                {"Login", new Action<APICommand>(Login) },
                {"Registration", new Action<APICommand>(Registration) },
                {"Unlogin", new Action<APICommand>(Unlogin) },
                {"FindSession", new Action<APICommand>(FindSession) },
                {"MakeMove", new Action<APICommand>(MakeMove) },
                {"LoadNewDeck", new Action<APICommand>(LoadNewDeck) },
                {"LoadPlayerInfo", new Action<APICommand>(LoadPlayerInfo) }

            });
        }
        public override void OnDisconnected() { supervisor.Disconnect(player); player = null; }
        
        #region API methods
        [APICommandAttr(new Type[] { typeof(LoginningForm) })]
        void Login(APICommand command)
        {
            if (player != null) SendObject(new APIAnswer(command, null, new ArgumentException("Already loginned")));
            var f = (LoginningForm)command.Params[0];
            supervisor.Login(f.Login, f.Password, out player, out Exception e);
            if (e != null) SendObject(new APIAnswer(command, null, e));
            else if (player != null)
                SendObject(new APIAnswer(command, player.PlayerInfo.SharedPlayerInfo));
            player.Send = SendObject;
        }
        [APICommandAttr(new Type[] { typeof(LoginningForm) })]
        void Registration(APICommand command)
        {
            if (player != null) SendObject(new APIAnswer(command, null, new ArgumentException("Already loginned")));
            var f = (LoginningForm)command.Params[0];
            supervisor.Registration(f.Login, f.Password, out player, out Exception e);
            if (e != null) SendObject(new APIAnswer(command, null, e));
            else if (player != null)
                SendObject(new APIAnswer(command, player.PlayerInfo.SharedPlayerInfo));
            player.Send = SendObject;
        }
        [APICommandAttr]
        void Unlogin(APICommand command)
        {
            supervisor.Disconnect(player);
            player = null;
            player.Send = null;
        }
        [APICommandAttr]
        void FindSession(APICommand command)
        { supervisor.FindSession(player); }
        [APICommandAttr(new Type[] { typeof(CardSessionShared.SessionCommand) })]
        void MakeMove(APICommand command)
        {
            var f = command.Params[0] as CardSessionShared.SessionCommand;
            if (player == null)
            { SendObject(new APIAnswer(command, null, new ArgumentException("Not loginned"))); return; }
            var s = player.Sessions.Find(q => q.ID == f.SessionID);
            if (s == null) { SendObject(new APIAnswer(command, null, new ArgumentException("Not my session"))); return; }
            try
            {
                if (f.TargetPosition == null)
                    player.MakeMove(f.SessionID, f.SubjectID, f.Command, new object[] { f.TargetID });
                else player.MakeMove(f.SessionID, f.SubjectID, f.Command, new object[] { f.TargetPosition });
            }
            catch (Exception e) { SendObject(new APIAnswer(command, null, new ArgumentException(e.Message))); }
        }
        [APICommandAttr(new Type[] { typeof(CardEnvironmentShared.PlayerInfo) })]
        void LoadNewDeck(APICommand command)
        {
            var f = command.Params[0] as CardEnvironmentShared.PlayerInfo;
            if (player == null) SendObject(new APIAnswer(command, null, new ArgumentException("Not loginned")));
            else
            {
                player.PlayerInfo.SharedPlayerInfo.CurrentSoliders = f.CurrentSoliders;
                player.PlayerInfo.SharedPlayerInfo.CurrentSpells = f.CurrentSpells;
            }
        }
        [APICommandAttr(new Type[] { typeof(string) })]
        void LoadPlayerInfo(APICommand command)
        {
            supervisor.FindPlayer((string)command.Params[0], out Pair<CardEnvironmentShared.PlayerInfo, bool> p, out Exception e);
            if (e != null) SendObject(new APIAnswer(command, null, e));
            else if (p != null) SendObject(new APIAnswer(command, p));
        }
        #endregion
    }
}