using System;

namespace Chat
{
    /// <summary>
    /// Инфо о участнике чата
    /// </summary>
    public interface IChatOwnerInfo
    {
        /// <summary>
        /// Имя участника чата
        /// </summary>
        string Name { get; }
    }
}