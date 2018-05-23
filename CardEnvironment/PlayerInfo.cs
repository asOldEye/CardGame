using System;

namespace CardEnvironmentServer
{
    /// <summary>
    /// Информация о игроке(только для сервера)
    /// </summary>
    [Serializable]
    public class PlayerInfo
    {
        public PlayerInfo(CardEnvironmentShared.PlayerInfo sharedPlayerInfo, string password)
        {
            if ((Password = password) == null) throw new ArgumentNullException(nameof(password));
            if ((SharedPlayerInfo = sharedPlayerInfo) == null) throw new ArgumentNullException(nameof(sharedPlayerInfo));
        }

        /// <summary>
        /// Пароль игрока
        /// </summary>
        public string Password { get; }
        /// <summary>
        /// Внешняя информация о игроке
        /// </summary>
        public CardEnvironmentShared.PlayerInfo SharedPlayerInfo { get; }
    }
}