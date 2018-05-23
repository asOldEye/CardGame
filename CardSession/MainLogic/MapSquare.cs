using System;
using System.Collections.Generic;

namespace CardSessionServer
{
    /// <summary>
    /// Квадрат карты
    /// </summary>
    [Serializable]
    class MapSquare
    {
        internal MapSquare(List<Modifier> modifiers, Container positioned)
        {
            if ((this.modifiers = modifiers) == null) this.modifiers = new List<Modifier>();
            Positioned = positioned;
        }

        List<Modifier> modifiers;
        /// <summary>
        /// Модификаторы на данной клетке
        /// </summary>
        public virtual Modifier[] Modifiers { get => modifiers.ToArray(); }
        /// <summary>
        /// Добавить модификатор
        /// </summary>
        public void AddModifier(Modifier mod)
        {
            if (mod == null) throw new ArgumentNullException(nameof(mod));
            if (modifiers.Contains(mod)) throw new ArgumentException("Already have this component");
            modifiers.Add(mod);
            if (positioned != null)
                if (mod is DurableModifier)
                    (mod as DurableModifier).SetTiming(positioned, 0);
        }
        /// <summary>
        /// Удалить модификатор
        /// </summary>
        public bool DelModifier(Modifier mod)
        {
            if (mod == null) throw new ArgumentNullException(nameof(mod));
            if (!modifiers.Remove(mod)) return false;
            if (positioned != null)
                if (mod is DurableModifier)
                    (mod as DurableModifier).SetTiming(positioned, 0);
            return true;
        }

        Container positioned;
        /// <summary>
        /// Объект, расположенный на этой клетке
        /// </summary>
        public virtual Container Positioned
        {
            get => positioned;
            set
            {
                if (value != null)
                {
                    if (value.GetComponent<Positionable>() == null)
                        throw new ArgumentException("Positioned container must contain 'positionable' component to be positioned on map");
                    if (positioned != null)
                        throw new ArgumentException("In this square already positioned another container");
                    foreach (var f in Modifiers) f.Action(value);
                }
                else if (positioned != null)
                    foreach (var f in Modifiers)
                        if (f is DurableModifier)
                            (f as DurableModifier).SetTiming(positioned, 0);
                positioned = value;
            }
        }
    }
}