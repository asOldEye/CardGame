using System;

namespace AuxiliaryLibrary
{
    /// <summary>
    /// Команда обращения к АПИ сервера
    /// </summary>
    [Serializable]
    public class APICommand
    {
        /// <summary>
        /// Имя комманды сервера
        /// </summary>
        public string Command { get; }
        /// <summary>
        /// Параметры, передаваемые на сервер
        /// </summary>
        public object[] Params { get; }
        /// <param name="command">Имя команды на сервере</param>
        /// <param name="param">Параметры, передаваемые на сервер</param>
        public APICommand(string command, object[] param)
        {
            if ((Command = command) == null) throw new ArgumentNullException(nameof(command));
            if ((Params = param) == null) throw new ArgumentNullException(nameof(param));
        }
    }
}