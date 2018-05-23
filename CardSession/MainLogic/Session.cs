using System;
using System.Collections.Generic;
using System.Reflection;
using AuxiliaryLibrary;
using System.Threading.Tasks;
using CardSessionShared;

namespace CardSessionServer
{
    [Interptered("Session")]
    [Serializable]
    public class Session
    {
        public Session(Position mapSize, Pair<Modifier, int>[] mapModifiersProbability = null)
        { Map = mapModifiersProbability == null ? new Map(mapSize) : new Map(mapSize, mapModifiersProbability); }
        [Interptered("IsPlay")]
        public bool IsPlay { get; private set; }
        [Interptered("ID")]
        public long ID { get; }
        public bool CanBeUploaded { get; private set; } = false;
        static public readonly DistributionRandom Random = new DistributionRandom();
        int maxID = 0;
        [Interptered("SessionObjects")]
        public List<Container> SessionObjects { get; } = new List<Container>();
        [Interptered("Controllers")]
        public List<FreePair<IControllerInfo, TurnStatus>> Controllers { get; } = new List<FreePair<IControllerInfo, TurnStatus>>();
        internal Map Map { get; }
        /// <summary>
        /// Этот игрок ходит прямо сейчас
        /// </summary>
        public SessionsController HisTurn { get; private set; }
        /// <summary>
        /// Номер текущего хода
        /// </summary>
        [Interptered("Turn")]
        public int Turn { get; private set; } = 0;
        /// <summary>
        /// Время отведенное на ход
        /// </summary>
        [Interptered("TimePerTurn")]
        public TimeSpan TimePerTurn { get; private set; }
        /// <summary>
        /// Время начала текущего хода
        /// </summary>
        [Interptered("TurnStartTime")]
        public DateTime TurnStartTime { get; private set; }

        /// <summary>
        /// Добавить контроллер
        /// </summary>
        public void AddController(SessionsController controller)
        {
            if (controller == null) throw new ArgumentNullException(nameof(controller));
            if (Controllers.Find(q => q.Obj1 == controller) != null) throw new ArgumentException("Already in session");
            if (IsPlay) throw new ArgumentException("Can't add controller on play");
            Controllers.Add(new FreePair<IControllerInfo, TurnStatus>(controller.ControllerInfo, TurnStatus.expected));
            try
            { controller.AddSession(this); }
            catch { throw; }
        }
        /// <summary>
        /// Сделать ход
        /// </summary>
        public void MakeMove(SessionsController sender, int subjectID, string command, object[] param)
        {
            if (sender == null) throw new ArgumentNullException(nameof(sender));
            if (command == null) throw new ArgumentNullException(nameof(command));
            if (param == null) throw new ArgumentNullException(nameof(param));

            var controller = Controllers.Find(q => q.Obj1 == sender.ControllerInfo);
            if (controller == null) throw new ArgumentException("Controller is'nt player of this session");
            if (controller.Obj2 != TurnStatus.expected) throw new ArgumentException("Controller already make his move");

            var subject = GetObject(subjectID);
            if (subject == null) throw new ArgumentException("No subject with this ID in this session");

            var components = subject.GetComponentsOf<Component>();
            foreach (var component in components)
            {
                List<Type> types = new List<Type>();
                foreach (var f in param) types.Add(f.GetType());

                var method = component.GetType().GetMethod(command, types.ToArray());
                if (method == null) throw new ArgumentException("Wrong method or params");

                var attr = method.GetCustomAttribute(typeof(ControllerCommand), true);
                if (attr == null) throw new ArgumentException("Method is'nt controller command");
                if ((attr as ControllerCommand).OnMyTurn != (HisTurn == sender)) throw new ArgumentException("Turn error, not your turn");
                method.Invoke(component, param);
                return;
            }
        }

        /// <summary>
        /// Добавить объект в игру
        /// </summary>
        public void AddObject(Container obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (SessionObjects.Contains(obj)) throw new ArgumentException("Already in session");
            SessionObjects.Add(obj);
            obj.SetSession(this, maxID++);
        }
        /// <summary>
        /// Получает объект по его ID в сессии
        /// </summary>
        public Container GetObject(int id)
        {
            return SessionObjects.Find(f => f.ID == id);
        }
        /// <summary>
        /// Получает объекты определенного типа
        /// </summary>
        public List<Container> GetObjects<T>()
        {
            return SessionObjects.FindAll(f => f.GetType() == typeof(T));
        }
        /// <summary>
        /// Удаляет объект из сессии
        /// </summary>
        public bool DelObject(Container container)
        {
            if (SessionObjects.Remove(container))
            {
                container.SetSession(null);
                return true;
            }
            return false;
        }

        public NonParametrizedEventHandler<SessionsController> OnTurnEnds;
        public NonParametrizedEventHandler<SessionsController> OnSessionEnded;
        public EmptyEventHandler OnSessionStart;
        public ParametrizedEventHandler<Session, SessionChange> OnSessionChanged;

        internal void SessionChanged(SessionChange change)
        {
            if (IsPlay)
            {
                Changes.Add(change);
                if (OnSessionChanged != null) OnSessionChanged.Invoke(this, change);
            }
        }

        /// <summary>
        /// Изменения в сессии
        /// </summary>
        [Interptered("Changes")]
        public List<SessionChange> Changes { get; } = new List<SessionChange>();

        /// <summary>
        /// Начать игровую сессию
        /// </summary>
        public void StartSession()
        {
            if (IsPlay) throw new ArgumentException("Already in game");
            if (Controllers.Count < 2) throw new ArgumentException("Not enought players");
            IsPlay = true;
            if (OnSessionStart != null) OnSessionStart.Invoke();
        }
        /// <summary>
        /// Вызывается для проверки условия завершения сессии
        /// </summary>
        public virtual bool SessionEndsCondition()
        {
            if (Controllers.Count == 1) return true;
            return false;
        }
        /// <summary>
        /// Вызывается для проверки, выбыл ли игрок
        /// </summary>
        public virtual bool SessionRemoveControllerCondition(SessionsController controller)
        {
            if (GetObjects<Castle>().Find(q => q.Owner == controller.ControllerInfo) == null)
                return true;
            return false;
        }
        /// <summary>
        /// Вызывается по окончании сессии для выяснения победителя
        /// </summary>
        public virtual IControllerInfo Winner()
        {
            if (Controllers.Count == 1) return Controllers[0].Obj1;
            return null;
        }
        /// <summary>
        /// Удаляет игрока и его солдат из игры
        /// </summary>
        public void RemoveController(SessionsController controller)
        {
            if (controller == null) throw new ArgumentNullException(nameof(controller));
            var c = Controllers.Find(q => q.Obj1 == controller.ControllerInfo);
            if (c == null) throw new ArgumentException("This controller is'nt in this session");
            Controllers.Remove(c);
            var objs = SessionObjects.FindAll(cont => cont.Owner == controller);
            foreach (var f in objs)
            {
                f.SetSession(null);
                SessionObjects.Remove(f);
            }
        }

        //todo
        public async void TurnExpect()
        {
            Turn++;
            foreach (var f in Controllers) f.SetObj2(TurnStatus.expected);

            await Task.Delay(TimePerTurn);

        }
    }
}