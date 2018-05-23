using System;
using System.Collections.Generic;
using CardSessionShared;

namespace CardSessionServer
{
    /// <summary>
    /// Контроллер сессии
    /// </summary>
    [Serializable]
    public abstract class SessionsController
    {
        public IControllerInfo ControllerInfo { get; protected set; }
        List<Session> sessions = new List<Session>();

        public void AddSession(Session session)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));
            if (sessions.Contains(session)) throw new ArgumentException("Already in session");
            sessions.Add(session);
            session.OnSessionChanged += OnSessionChanged;
        }

        public void MakeMove(long sessionID, int subjectID, string command, object[] param)
        {
            var s = sessions.Find(q => q.ID == sessionID);
            if (s == null) throw new ArgumentException("Not my session");
            try
            { s.MakeMove(this, subjectID, command, param); }
            catch { throw; }
        }

        /// <summary>
        /// Выгрузить сессию целиком
        /// </summary>
        public List<InterpretedObject> UploadWholeSessions()
        {
            List<InterpretedObject> ssns = new List<InterpretedObject>();
            foreach (var f in sessions)
                if (f.CanBeUploaded) lock (f) ssns.Add(Interpretor.Interpretate(f));
            return ssns;
        }

        /// <summary>
        /// Выгрузжать изменения в сессии
        /// </summary>
        public bool UnloadSessionChanges { get; set; }

        protected virtual void OnSessionChanged(Session session, SessionChange change) { }
    }
}