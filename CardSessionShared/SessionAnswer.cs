namespace CardSessionShared
{
    [System.Serializable]
    public class SessionAnswer
    {
        public SessionAnswer(long sessionID, SessionCommand command, Answer reply)
        {
            SessionID = sessionID;
            if((Command = command) == null) throw new System.ArgumentNullException(nameof(command));
            Reply = reply;
        }

        public long SessionID { get; }
        public SessionCommand Command { get; }
        public Answer Reply { get; }
        [System.Serializable]
        public enum Answer { ok, wrongSubject, wrongTarget, wrongCommand, wrongSession }
    }
}