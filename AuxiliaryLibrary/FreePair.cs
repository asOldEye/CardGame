namespace AuxiliaryLibrary
{
    /// <summary>
    /// Пара объектов со свободно изменяемым содержимым
    /// </summary>
    /// <typeparam name="T1">Тип первого объекта</typeparam>
    /// <typeparam name="T2">Тип второго объекта</typeparam>
    [System.Serializable]
    public class FreePair<T1, T2> : Pair<T1, T2>
    {
        /// <summary>
        /// Изменить первый объект
        /// </summary>
        /// <param name="obj">Новый объект на замену</param>
        public void SetObj1(T1 obj)
        { Obj1 = obj; }
        /// <summary>
        /// Изменить второй объект
        /// </summary>
        /// <param name="obj">Новый объект на замену</param>
        public void SetObj2(T2 obj)
        { Obj2 = obj; }

        public FreePair(T1 obj1 = default(T1), T2 obj2 = default(T2)) : base(obj1, obj2)
        { }
    }
}