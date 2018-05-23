namespace AuxiliaryLibrary
{
    /// <summary>
    /// Основа событийных аргументов с параметром
    /// </summary>
    /// <typeparam name="TSender">Тип передающего объекта</typeparam>
    /// <typeparam name="TParam">Тип передаваемого параметра</typeparam>
    /// <param name="sender">Передающий объект</param>
    /// <param name="param">Передаваемый параметр</param>
    public delegate void ParametrizedEventHandler<TSender, TParam>(TSender sender, TParam param);
}