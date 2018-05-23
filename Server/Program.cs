using System;
using System.Net;
using ConnectionProtocol;
using System.IO;
using CardEnvironmentShared;
using SimpleDatabase;
using CardEnvironmentServer;
using AuxiliaryLibrary;
using System.Collections.Generic;
using CardSessionServer;

namespace Server
{
    class Program
    {
        static Database database;
        static Supervisor supervisor;
        static ConnectionProvider provider;

        static void Main(string[] args)
        {
            Console.WriteLine("Server starting...");
            var connectionOpitions = new ConnectionOpitions(true, new TimeSpan(0, 0, 10), new TimeSpan(0, 0, 15));
            provider = new ConnectionProvider(IPAddress.Any, 8888, 10, connectionOpitions);
            provider.OnConnectionsCountChanged += ReDrawUI;
            provider.OnIncomingConnection += OnIncomingConnection;
            database = new Database("CardCollectiveUsersDatabase", Directory.GetCurrentDirectory());

            //init

            var spells = new List<Pair<Container, Info>>()
            {
            new Pair<Container, Info>(new SpellCard(10, new List<Modifier>() {new Modifier(typeof(Destroyable), "DeltaHealth", new object[] { 5 })}), new Info("Healing potion", "restores 5 units of health")),
            new Pair<Container, Info>(new SpellCard(10, new List<Modifier>() { new Modifier(typeof(Attacker), "DeltaPower", new object[] { 5 }) }), new Info("Warming up potion", "restores 5 units of attack power")),
            new Pair<Container, Info>(new SpellCard(10, new List<Modifier>() { new Modifier(typeof(Destroyable), "DeltaHealth", new object[] { -5 }) }), new Info("Destructive potion", "damages 5 units of health")),
            };
            var soliders = new List<Pair<Container, Info>>()
            {
                new Pair<Container, Info>(new SoliderCard(5,10,10,5,1), new Info("Peasant", "a peasant from a nearby village, a weak but cheap unit")),
                new Pair<Container, Info>(new SoliderCard(10,5,5,15,1), new Info("Militiaman", "a peasant from a nearby village, not so weak but cheap unit")),
                new Pair<Container, Info>(new SoliderCard(15,10,10,25,1), new Info("Old man", "in a long-standing war he killed people")),
                new Pair<Container, Info>(new SoliderCard(15,30,30,10,1), new Info("Strong peasant", "young, strong, able to hold back the pressure for a long time")),
                new Pair<Container, Info>(new SoliderCard(20,40,40,10,1), new Info("Warrior", "at least he has a real sword")),
                new Pair<Container, Info>(new SoliderCard(25,30,30,40,1), new Info("Trained warrior", "knows where to hit to kill")),
                new Pair<Container, Info>(new SoliderCard(30,40,40,50,1), new Info("Veteran", "he saw blood liters")),
                new Pair<Container, Info>(new SoliderCard(35,60,60,60,1), new Info("Mercenary", "on your side while you have money")),
                new Pair<Container, Info>(new SoliderCard(40,75,75,75,1), new Info("Knight", "elite, heavily armored, very expensive")),
                new Pair<Container, Info>(new SoliderCard(50,100,100,100,1), new Info("Paladin", "best of the best, capable of almost everything")),
                new Pair<Container, Info>(new CasterSoliderCard(40,20,20,5,1,25,25,new List<SpellCard>()
                {new SpellCard(10, new List<Modifier>() {new Modifier(typeof(Destroyable), "DeltaHealth", new object[] { 5 })}),
                new SpellCard(10, new List<Modifier>() { new Modifier(typeof(Attacker), "DeltaPower", new object[] { 5 }) }),
                new SpellCard(10, new List<Modifier>() { new Modifier(typeof(Destroyable), "DeltaHealth", new object[] { -5 }) })
                }), new Info("Witch", "creates spells on the battlefield")),
            };

            //init
            
            supervisor = new Supervisor(soliders, spells, database, 10, 10);
            provider.AllowNewConnections = true;
            Console.WriteLine("Server in work...");
            while (Console.ReadLine() != "stop") ;
        }

        static void OnIncomingConnection(ConnectionProvider provider, ServerConnection connection)
        { if (connection != null) new Seance(connection, new CardGameAPI(supervisor)); }
        static void ReDrawUI(ConnectionProvider provider, int count)
        { Console.WriteLine(DateTime.Now + " connections count: [" + count + "]/[" + provider.MaxConnections + "]"); }
    }
}