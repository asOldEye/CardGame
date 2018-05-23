using System;
using System.Collections.Generic;
using AuxiliaryLibrary;

namespace Chat
{
    [Serializable]
    /// <summary>
    /// Чат
    /// </summary>
    public class Chat
    {
        public Chat(List<IChatOwnerInfo> owners = null, Queue<Entry> messages = null)
        {
            if (owners != null)
                this.owners = new List<IChatOwnerInfo>(owners.FindAll(f => f != null));
            else this.owners = new List<IChatOwnerInfo>();
            if (this.owners.Count < 2) throw new ArgumentOutOfRangeException("Too few owners");
        }

        List<IChatOwnerInfo> owners;
        public List<Entry> Messages { get; } = new List<Entry>();

        internal void WriteMessage(Entry entry)
        {
            if (entry == null) throw new ArgumentNullException(nameof(entry));
            Messages.Add(entry);
            if (OnNewMessage != null)
                OnNewMessage.Invoke(this, entry);
        }

        public void AddOwner(IChatOwnerInfo owner)
        {
            if (owner == null) throw new ArgumentNullException(nameof(owner));
            if (owners.Contains(owner)) throw new ArgumentException("This owner already owner");

            owners.Add(owner);

            if (OnOwnerJoined != null)
                OnOwnerJoined.Invoke(this, owner);
        }
        public void DelOwner(IChatOwnerInfo owner)
        {
            if (owner == null) throw new ArgumentNullException(nameof(owner));
            if (!owners.Remove(owner)) throw new ArgumentException("Not my owner");
            if (OnOwnerLeft != null)
                OnOwnerLeft.Invoke(this, owner);
        }

        public event ParametrizedEventHandler<Chat, Entry> OnNewMessage;

        public event ParametrizedEventHandler<Chat, IChatOwnerInfo> OnOwnerJoined;
        public event ParametrizedEventHandler<Chat, IChatOwnerInfo> OnOwnerLeft;
    }
}