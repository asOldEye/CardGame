using System;
using System.Collections.Generic;
using CardSessionShared;
using AuxiliaryLibrary;

namespace CardEnvironmentShared
{
    /// <summary>
    /// Сессия, отправляемая на клиент
    /// </summary>
    [Serializable]
    public class Session
    {
        public Session(InterpretedSession session, Info sessionInfo, List<Pair<int, Info>> idInfo)
        {
            if ((SessionRepresentation = session) == null) throw new ArgumentNullException(nameof(session));
            if ((SessionInfo = sessionInfo) == null) throw new ArgumentNullException(nameof(sessionInfo));
            if ((IdInfo = idInfo) == null) throw new ArgumentNullException(nameof(idInfo));
        }

        /// <summary>
        /// Представление сессии
        /// </summary>
        public InterpretedSession SessionRepresentation { get; }
        /// <summary>
        /// Информация о сессии
        /// </summary>
        public Info SessionInfo { get; }
        /// <summary>
        /// Описания объектов в сессии
        /// </summary>
        public List<Pair<int, Info>> IdInfo { get; }
    }
}