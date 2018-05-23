using System;

namespace CardSessionShared
{
    [Serializable]
    public class SessionCommand
    {
        public SessionCommand(long sessionID, string command, int subjectID, Position targetPosition)
        {
            if ((Command = command) == null) throw new ArgumentNullException(nameof(command));
            SessionID = sessionID;
            SubjectID = subjectID;
            TargetPosition = targetPosition;
        }
        public SessionCommand(long sessionID, string command, int subjectID, int targetID)
        {
            if ((Command = command) == null) throw new ArgumentNullException(nameof(command));
            SessionID = sessionID;
            SubjectID = subjectID;
            TargetID = targetID;
        }
        public long SessionID { get; }
        public string Command { get; }
        public int SubjectID { get; } = -1;
        public Position? TargetPosition { get; } = null;
        public int? TargetID { get; } = null;
    }
}