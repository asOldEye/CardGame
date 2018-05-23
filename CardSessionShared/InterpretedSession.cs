using System;
using System.Collections.Generic;
using AuxiliaryLibrary;

namespace CardSessionShared
{
    /// <summary>
    /// Интерпретированная сессия
    /// </summary>
    [Serializable]
    public class InterpretedSession : InterpretedObject
    {
        public InterpretedSession(string type, List<FreePair<string, object>> param, int frameNumber) : base(type, param)
        { FrameNumber = frameNumber; }
        /// <summary>
        /// Номер кадра
        /// </summary>
        public int FrameNumber { get; private set; }

        /// <summary>
        /// Изменить сессию
        /// </summary>
        public void MakeChange(InterpretedSession changed)
        {
            if (changed.FrameNumber < FrameNumber) return;
            FrameNumber = changed.FrameNumber;
            Params = changed.Params;
            if (OnSessionChanged != null) OnSessionChanged.Invoke(this);
        }
        /// <summary>
        /// Карта
        /// </summary>
        public InterpretedObject[,] Map;

        public event NonParametrizedEventHandler<InterpretedSession> OnSessionChanged;
    }
}