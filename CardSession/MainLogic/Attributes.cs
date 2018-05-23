using System;

namespace CardSessionServer
{
    /// <summary>
    /// Размещается у методов, способных быть вызванными из Modifier
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    sealed class Modified : Attribute { }

    /// <summary>
    /// Размещается у методов, способных быть вызванными из контроллера
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    sealed class ControllerCommand : Attribute
    {
        public bool OnMyTurn { get; }
        public ControllerCommand(bool onMyTurn = true) { OnMyTurn = onMyTurn; }
    }

    /// <summary>
    /// Размещается у объектов, подлежащих интерпретации
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property,
        Inherited = false, AllowMultiple = false)]
    sealed class Interptered : Attribute
    {
        /// <summary>
        /// Имя интерпретированного объекта
        /// </summary>
        public string Name { get; }
        public Interptered(string name)
        { if ((Name = name) == null) throw new ArgumentNullException(nameof(name)); }
    }
}