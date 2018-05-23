using System;

namespace Chat
{
    /// <summary>
    /// Запись в чате
    /// </summary>
    [Serializable]
    public class Entry
    {
        /// <summary>
        /// Владелец записи
        /// </summary>
        public IChatOwnerInfo Owner
        { get; protected set; }
        /// <summary>
        /// Время создания записи
        /// </summary>
        public DateTime Time
        { get; protected set; }
        /// <summary>
        /// Текст записи
        /// </summary>
        public String Message
        { get; protected set; }
        
        internal Entry(IChatOwnerInfo owner, String message, DateTime dateTime)
        {
            Message = message;
            Owner = owner;
            Time = dateTime;
        }
    }
}