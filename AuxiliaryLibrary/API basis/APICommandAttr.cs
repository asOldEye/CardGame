using System;

namespace AuxiliaryLibrary
{
    /// <summary>
    /// Атрибут, указывающий на то, что метод является АПИ командой
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class APICommandAttr : Attribute
    {
        /// <param name="inputParams">Параметры, которые должны быть реализованы во входящем запросе</param>
        public APICommandAttr(Type[] inputParams = null)
        {
            if ((InputParams = inputParams) == null) InputParams = new Type[] { };
        }
        /// <summary>
        /// Параметры, которые должны быть реализованы во входящем запросе и их порядок
        /// </summary>
        public Type[] InputParams { get; }
    }
}