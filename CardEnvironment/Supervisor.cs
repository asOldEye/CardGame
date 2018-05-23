using System;
using System.Collections.Generic;
using AuxiliaryLibrary;
using System.Threading.Tasks;
using CardSessionShared;
using CardEnvironmentShared;
using CardSessionServer;

namespace CardEnvironmentServer
{
    public class Supervisor
    {
        List<Pair<Container, Pair<InterpretedObject, Info>>> Soliders { get; } = new List<Pair<Container, Pair<InterpretedObject, Info>>>();
        List<Pair<Container, Pair<InterpretedObject, Info>>> Spells { get; } = new List<Pair<Container, Pair<InterpretedObject, Info>>>();

        SimpleDatabase.Database database;

        List<Player> onlinePlayers = new List<Player>();
        List<Player> sessionAwaitPlayers = new List<Player>();

        int StartSolidersCount { get; }
        int StartSpellsCount { get; }

        Task sessionFounder;
        public void FindSession(Player api)
        {
            lock (sessionAwaitPlayers) sessionAwaitPlayers.Add(api);
            if (sessionFounder.IsCompleted)
                (sessionFounder = new Task(new Action(SessionFounder))).Start();
        }
        void SessionFounder()
        {
            Random random = new Random();
            while (sessionAwaitPlayers.Count > 1)
            {
                lock (sessionAwaitPlayers)
                {
                    var p1 = sessionAwaitPlayers[random.Next(sessionAwaitPlayers.Count)];
                    sessionAwaitPlayers.Remove(p1);
                    var p2 = sessionAwaitPlayers[random.Next(sessionAwaitPlayers.Count)];
                    sessionAwaitPlayers.Remove(p2);
                    var session = new CardSessionServer.Session(new Position(8, 8), new Pair<Modifier, int>[]
                        {
                        new Pair<Modifier, int>(new DurableModifier(typeof(Destroyable), "DeltaHealth", new object[] { 10 }), 15),
                        new Pair<Modifier, int>(new Modifier(typeof(Attacker), "DeltaAttack", new object[] { -10 }), 15)
                        });
                    
                    foreach (var f in p1.PlayerInfo.SharedPlayerInfo.CurrentSoliders)
                    {
                        var ser = Soliders.Find(q => q.Obj2 == f).Obj1;
                        var o = BinarySerializer.Deserialize(BinarySerializer.Serialize(ser));
                        ((Container)o).SetOwner(p1.ControllerInfo);
                        session.AddObject((Container)o);
                    }
                    foreach (var f in p1.PlayerInfo.SharedPlayerInfo.CurrentSpells)
                    {
                        var ser = Spells.Find(q => q.Obj2 == f).Obj1;
                        var o = BinarySerializer.Deserialize(BinarySerializer.Serialize(ser));
                        ((Container)o).SetOwner(p1.ControllerInfo);
                        session.AddObject((Container)o);
                    }

                    foreach (var f in p2.PlayerInfo.SharedPlayerInfo.CurrentSoliders)
                    {
                        var ser = Soliders.Find(q => q.Obj2 == f).Obj1;
                        var o = BinarySerializer.Deserialize(BinarySerializer.Serialize(ser));
                        ((Container)o).SetOwner(p2.ControllerInfo);
                        session.AddObject((Container)o);
                    }
                    foreach (var f in p2.PlayerInfo.SharedPlayerInfo.CurrentSpells)
                    {
                        var ser = Spells.Find(q => q.Obj2 == f).Obj1;
                        var o = BinarySerializer.Deserialize(BinarySerializer.Serialize(ser));
                        ((Container)o).SetOwner(p2.ControllerInfo);
                        session.AddObject((Container)o);
                    }

                    session.AddController(p1);
                    session.AddController(p2);

                    session.StartSession();
                }
            }
        }

        public Supervisor(List<Pair<Container, Info>> soliders, List<Pair<Container, Info>> spells, 
            SimpleDatabase.Database database, int startSolidersCount, int startSpellsCount)
        {
            if ((this.database = database) == null) throw new ArgumentNullException(nameof(database));
            foreach (var f in soliders)
                Soliders.Add(new Pair<Container, Pair<InterpretedObject, Info>>(f.Obj1, new Pair<InterpretedObject, Info>(Interpretor.Interpretate(f.Obj1), f.Obj2)));
            foreach (var f in spells)
                Spells.Add(new Pair<Container, Pair<InterpretedObject, Info>>(f.Obj1, new Pair<InterpretedObject, Info>(Interpretor.Interpretate(f.Obj1), f.Obj2)));
            (sessionFounder = new Task(new Action(SessionFounder))).Start();
            StartSolidersCount = startSolidersCount;
            StartSpellsCount = startSpellsCount;
        }

        public void Disconnect(Player player)
        {
            lock (onlinePlayers) onlinePlayers.Remove(player);
            lock (sessionAwaitPlayers)
                sessionAwaitPlayers.Remove(sessionAwaitPlayers.Find(q => q == player));
            RefreshDatabase(player.PlayerInfo);
        }
        PlayerInfo ReadDatabase(string login)
        { try { return (PlayerInfo)database.ReadObject(login); } catch { return null; } }
        public void RefreshDatabase(PlayerInfo p)
        { database.ReWriteObject(p.SharedPlayerInfo.Name, p); }

        public void Login(string login, string password, out Player player, out Exception exception)
        {
            exception = null;
            player = null;
            var f = ReadDatabase(login);
            if (f == null) exception = new ArgumentException("Wrong login");
            else if (f.Password != password) exception = new ArgumentException("Wrong password");
            else
            {
                var p = onlinePlayers.Find(q => q.PlayerInfo == f);
                if (p != null) exception = new ArgumentException("Already online");
                onlinePlayers.Add(player = new Player(f));
            }
        }
        public void Registration(string login, string password, out Player player, out Exception exception)
        {
            exception = null;
            player = null;
            lock (database)
            {
                var f = ReadDatabase(login);
                if (f != null) exception = new ArgumentException("Login already in use");
                else
                {
                    var sol = new List<Pair<InterpretedObject, Info>>();
                    for (int i = 0; i < StartSolidersCount; i++)
                        sol.Add(LootSolider(new List<Pair<InterpretedObject, Info>>()));
                    var spls = new List<Pair<InterpretedObject, Info>>();
                    for (int i = 0; i < StartSpellsCount; i++)
                        spls.Add(LootSpell(new List<Pair<InterpretedObject, Info>>()));
                    var info = new CardEnvironmentShared.PlayerInfo
                    {
                        Name = login,
                        Soliders = sol,
                        Spells = spls
                    };
                    try
                    { database.WriteObject(login, f = new PlayerInfo(info, password)); }
                    catch { return; }
                    lock (onlinePlayers) onlinePlayers.Add(player = new Player(f));
                }
            }
        }

        public void FindPlayer(string login, out Pair<CardEnvironmentShared.PlayerInfo, bool> pl, out Exception e)
        {
            e = null; pl = null;
            var f = ReadDatabase(login);
            if (f == null) e = new ArgumentException("Player is'nt exists");
            else
            {
                var p = onlinePlayers.Find(q => q.PlayerInfo.SharedPlayerInfo.Name == login);
                pl = new Pair<CardEnvironmentShared.PlayerInfo, bool>(f.SharedPlayerInfo, p == null ? false : true);
            }
        }

        public Pair<InterpretedObject, Info> LootSolider(List<Pair<InterpretedObject, Info>> owned)
        {
            if (owned.Count == Soliders.Count) return null;
            Random random = new Random();
            var f = Soliders.FindAll(q => !owned.Contains(q.Obj2));
            return f[random.Next(f.Count)].Obj2;
        }
        public Pair<InterpretedObject, Info> LootSpell(List<Pair<InterpretedObject, Info>> owned)
        {
            if (owned.Count == Spells.Count) return null;
            Random random = new Random();
            var f = Spells.FindAll(q => !owned.Contains(q.Obj2));
            return f[random.Next(f.Count)].Obj2;
        }
    }
}