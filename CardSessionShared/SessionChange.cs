using System;

namespace CardSessionShared
{
    /// <summary>
    /// Изменения в сессии
    /// </summary>
    [Serializable]
    public class SessionChange
    {
        public SessionChange(string message, int senderID)
        {
            if ((Message = message) == null) Message = string.Empty;
            SenderID = senderID;
        }
        public SessionChange(string message)
        { if ((Message = message) == null) Message = string.Empty; }
        /// <summary>
        /// Сопутствующее сообщение 
        /// </summary>
        public string Message { get; }
        /// <summary>
        /// Объект, вызвавший изменение
        /// </summary>
        public int SenderID { get; }
    }
}