using System;

namespace AuxiliaryLibrary
{
    /// <summary>
    /// Пара объектов
    /// </summary>
    /// <typeparam name="T1">Тип первого объекта</typeparam>
    /// <typeparam name="T2">Тип второго объекта</typeparam>
    [Serializable]
    public class Pair<T1, T2>
    {
        public Pair(T1 obj1 = default(T1), T2 obj2 = default(T2))
        {
            Obj1 = obj1;
            Obj2 = obj2;
        }

        /// <summary>
        /// Первый объект пары
        /// </summary>
        public T1 Obj1 { get; protected set; }
        /// <summary>
        /// Второй объект пары
        /// </summary>
        public T2 Obj2 { get; protected set; }
    }
}