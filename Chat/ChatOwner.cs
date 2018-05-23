using System;
using System.Collections.Generic;
using AuxiliaryLibrary;

namespace Chat
{
    /// <summary>
    /// Владелец чатов
    /// </summary>
    [Serializable]
    public class ChatOwner
    {
        /// <summary>
        /// Инфо о владельце
        /// </summary>
        public IChatOwnerInfo Info { get; }
        /// <summary>
        /// Чаты, в которых участвует
        /// </summary>
        public List<Chat> Chats { get; } = new List<Chat>();

        public ParametrizedEventHandler<Chat, Entry> OnNewMessage;
        public ParametrizedEventHandler<Chat, IChatOwnerInfo> OnOwnerJoined, OnOwnerLeft;

        public ChatOwner(IChatOwnerInfo ownerInfo)
        {
            if ((Info = ownerInfo) == null) throw new ArgumentNullException(nameof(ownerInfo));
        }

        /// <summary>
        /// Войти в чат
        /// </summary>
        /// <param name="chat">Чат</param>
        public void JoinChat(Chat chat)
        {
            if (chat == null) throw new ArgumentNullException(nameof(chat));
            if (Chats.Contains(chat)) throw new ArgumentException("Already in this chat");
            Chats.Add(chat);
            chat.AddOwner(Info);

            chat.OnNewMessage += OnNewMessage;
            chat.OnOwnerJoined += OnOwnerJoined;
            chat.OnOwnerLeft += OnOwnerLeft;
        }
        /// <summary>
        /// Покинуть чат
        /// </summary>
        /// <param name="chat">Чат</param>
        public void LeftChat(Chat chat)
        {
            if (chat == null) throw new ArgumentNullException(nameof(chat));
            if (!Chats.Remove(chat)) throw new ArgumentException("Not in this chat");
            chat.DelOwner(Info);

            chat.OnNewMessage -= OnNewMessage;
            chat.OnOwnerJoined -= OnOwnerJoined;
            chat.OnOwnerLeft -= OnOwnerLeft;
        }
        /// <summary>
        /// Написать запись в чат
        /// </summary>
        /// <param name="chat">Чат</param>
        /// <param name="message">Сообщение</param>
        public void WriteEntry(Chat chat, string message)
        {
            if (chat == null) throw new ArgumentNullException(nameof(chat));
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (!Chats.Contains(chat)) throw new ArgumentException("Not my chat");
            chat.WriteMessage(new Entry(Info, message, DateTime.UtcNow));
        }
    }
}