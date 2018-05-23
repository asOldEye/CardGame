using AuxiliaryLibrary;
using System;
using System.Collections.Generic;

namespace CardSessionServer
{
    [Serializable]
    public class DurableModifier : Modifier
    {
        List<FreePair<Container, int>> modified = new List<FreePair<Container, int>>();
        /// <summary>
        /// Время действия модификатора
        /// </summary>
        [Interptered("Timing")]
        public int Timing { get; }

        public DurableModifier(Type targetType, string methodName, object[] param, int timing = -1)
            : base(targetType, methodName, param)
        {
            if ((Timing = timing) == 0) throw new ArgumentException("Wrong timing");
        }

        /// <summary>
        /// Установить тайминг для модифицируемого объекта
        /// </summary>
        /// <param name="target">Цель</param>
        /// <param name="newTiming">Новый тайминг</param>
        public void SetTiming(Container target, int newTiming)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            var f = modified.Find(mod => mod.Obj1 == target);
            if (f != null) f.SetObj2(newTiming);
            else throw new ArgumentException("Does'nt contain this target");
        }
        /// <summary>
        /// Добавляет новый модифицируемый объект
        /// </summary>
        /// <param name="target"></param>
        public void AddModified(Container target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            var f = modified.Find(mod => mod.Obj1 == target);
            if (f == null) modified.Add(new FreePair<Container, int>(target, Timing));
            else throw new ArgumentException("Does'nt contain this target");
        }

        /// <summary>
        /// Воздействует на объекты
        /// </summary>
        protected virtual void Action(SessionsController player)
        {
            foreach (var f in modified)
                if (f.Obj1.Owner == player)
                {
                    if (f.Obj2 > 0) f.SetObj2(f.Obj2 - 1);
                    if (f.Obj2 == 0)
                    {
                        modified.Remove(f);
                        f.Obj1.Modifiers.Remove(this);
                    }
                    else base.Action(f.Obj1 as Container);
                }
        }

        public override void OnTurnEnds(SessionsController controller)
        {
            base.OnTurnEnds(controller);
            Action(controller);
        }
        public override void Wanish()
        {
            base.Wanish();
            foreach (var f in modified)
                f.Obj1.Modifiers.Remove(this);
            modified.Clear();
        }

    }
}