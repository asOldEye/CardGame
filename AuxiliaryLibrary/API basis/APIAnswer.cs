using System;

namespace AuxiliaryLibrary
{
    /// <summary>
    /// Ответ клиенту от сервера
    /// </summary>
    [Serializable]
    public class APIAnswer
    {
        public APIAnswer(APICommand command, object answer, Exception exception = null)
        {
            if ((Command = command) == null) throw new ArgumentNullException(nameof(command));
            Answer = answer;
            Exception = exception;
        }

        /// <summary>
        /// Команда, ответом на которую является этот ответ
        /// </summary>
        public APICommand Command { get; }
        /// <summary>
        /// Ответный объект сервера
        /// </summary>
        public object Answer { get; }

        /// <summary>
        /// Исключение, отправленное клиенту сервером
        /// </summary>
        public Exception Exception { get; }
    }
}