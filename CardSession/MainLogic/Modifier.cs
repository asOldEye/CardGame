using System;
using System.Collections.Generic;
using System.Reflection;
using AuxiliaryLibrary;

namespace CardSessionServer
{
    /// <summary>
    /// Модификатор
    /// </summary>
    [Interptered("Modifier")]
    [Serializable]
    public class Modifier : Component
    {
        [Interptered("MethodName")]
        public string MethodName { get; }
        [Interptered("TargetType")]
        public Type TargetType { get; }
        MethodInfo Method { get; set; }
        [Interptered("Params")]
        public object[] Params { get; protected set; }

        public Modifier(Type targetType, string methodName, object[] param = null)
        {
            if ((MethodName = methodName) == null) throw new ArgumentNullException(nameof(methodName));
            if ((TargetType = targetType) == null) throw new ArgumentNullException(nameof(targetType));
            if ((Params = param) == null) Params = new object[] { };
            List<Type> prms = new List<Type>();
            while (targetType.BaseType != typeof(Object))
                if ((targetType = targetType.BaseType) == typeof(Component))
                {
                    foreach (var f in Params) prms.Add(f.GetType());
                    if ((Method = TargetType.GetMethod(methodName, prms.ToArray())) == null)
                        throw new ArgumentException("Target type have'nt method, named " + methodName);
                    if (Method.GetCustomAttribute(typeof(Modified)) == null)
                        throw new ArgumentException("Method have'nt modified attribute");
                    return;
                }
            throw new ArgumentException("Wrong target type, it must be inheritor of Component");
        }

        /// <summary>
        /// Воздействие на модифицируемые объекты
        /// </summary>
        public virtual void Action(List<Container> modified)
        {
            if (modified == null) throw new ArgumentNullException(nameof(modified));
            foreach (var f in modified)
            {
                var comp = f.GetComponent(TargetType);
                if (comp != null) Method.Invoke(comp, Params);
            }
        }
        /// <summary>
        /// Воздействие на модифицируемый объект
        /// </summary>
        public virtual void Action(Container target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            try
            { Action(new List<Container>() { target }); }
            catch { throw; }
        }
    }
}