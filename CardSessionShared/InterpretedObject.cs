using System;
using System.Collections.Generic;
using AuxiliaryLibrary;

namespace CardSessionShared
{
    /// <summary>
    /// Интерпретированный объект
    /// </summary>
    [Serializable]
    public class InterpretedObject
    {
        /// <summary>
        /// Тип объекта
        /// </summary>
        public string Type { get; protected set; }
        /// <summary>
        /// Параметры объекта
        /// </summary>
        public List<FreePair<string, object>> Params { get; protected set; }
        public InterpretedObject(string type, List<FreePair<string, object>> param)
        {
            if ((Type = type) == null) throw new ArgumentNullException(nameof(param));
            if ((Params = param) == null) throw new ArgumentNullException(nameof(param));
        }
    }
}