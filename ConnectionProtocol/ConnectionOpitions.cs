using System;

namespace ConnectionProtocol
{
    /// <summary>
    /// Опции соединения
    /// </summary>
    [Serializable]
    public sealed class ConnectionOpitions
    {
        public ConnectionOpitions(bool eventOriented,
            TimeSpan averageDisconnectAvait, TimeSpan maxDisconnectAvait, int bufferSize = 2048)
        {
            EventOriented = eventOriented;
            try
            {
                BufferSize = bufferSize;
                SetDisconnectAvait(maxDisconnectAvait, averageDisconnectAvait);
            }
            catch { throw; }
        }

        [NonSerialized]
        int bufferSize;
        /// <summary>
        /// Размер буферов приема и отправки
        /// </summary>
        public int BufferSize
        {
            get { return bufferSize; }
            set
            {
                if ((bufferSize = value) <= 10) throw new ArgumentException("Too small packet size");
            }
        }

        /// <summary>
        /// Время ожидания отправки пустой записи в молчащее подключение
        /// </summary>
        public TimeSpan AverageDisconnectAvait
        { get; private set; }
        /// <summary>
        /// Время ожидания молчащего подключения до дисконнекта
        /// </summary>
        public TimeSpan MaxDisconnectAvait
        { get; private set; }

        internal void SetDisconnectAvait(TimeSpan maxDisconnectAvait, TimeSpan averageDisconnectAvait)
        {
            if (maxDisconnectAvait < new TimeSpan(0, 0, 1) || averageDisconnectAvait < new TimeSpan(0, 0, 1))
                throw new ArgumentException("Too small avait");
            MaxDisconnectAvait = maxDisconnectAvait;
            if ((AverageDisconnectAvait = averageDisconnectAvait) >= MaxDisconnectAvait) throw new ArgumentException("Average disconnect avait must be lower than max disconnect await");
        }

        /// <summary>
        /// Дефолтные настройки соединения
        /// </summary>
        public static ConnectionOpitions Default
        {
            get { return new ConnectionOpitions(false, new TimeSpan(0, 0, 10), new TimeSpan(0, 0, 30)); }
        }

        /// <summary>
        /// Ориентировать на событийный разбор входящих сообщений
        /// </summary>
        public bool EventOriented { get; set; }
    }
}