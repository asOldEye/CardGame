using System;

namespace CardEnvironmentShared
{
    /// <summary>
    /// Описание объекта
    /// </summary>
    [Serializable]
    public class Info
    {
        public Info(string name, string descr)
        {
            Name = name; Description = descr;
        }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}