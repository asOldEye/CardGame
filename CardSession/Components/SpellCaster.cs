using AuxiliaryLibrary;
using CardSessionShared;
using System;
using System.Collections.Generic;

namespace CardSessionServer
{
    [Serializable]
    public class SpellCaster : Component
    {
        /// <summary>
        /// Колода заклинаний
        /// </summary>
        [Interptered("SpellCards")]
        public List<SpellCard> Spells { get; }
        public SpellCaster(List<SpellCard> toCast)
        {
            if ((Spells = toCast) == null) throw new ArgumentNullException(nameof(toCast));
        }

        /// <summary>
        /// Создать заклинание
        /// </summary>
        /// <param name="spell">Заклинание</param>
        /// <param name="target">Цель</param>
        [Modified]
        [ControllerCommand]
        public void Cast(SpellCard spell, Position target)
        {
            if (target.CompareTo(Container.Session.Map.Size) >= 0) throw new ArgumentException("Too big position");
            var f = Container.Session.Map.FindByPosition(target).Positioned;
            if (f == null) throw new ArgumentException("No target in this position");
            try
            { Cast(spell, f); }
            catch { throw; }
        }
        /// <summary>
        /// Создать заклинание
        /// </summary>
        /// <param name="spell">Заклинание</param>
        /// <param name="target">Цель</param>
        [Modified]
        [ControllerCommand]
        public void Cast(SpellCard spell, Container target)
        {
            var Mana = Container.GetComponent<Mana>();
            if (Mana == null) throw new ArgumentException("No mana in container");
            if (spell == null) throw new ArgumentNullException(nameof(spell));
            if (!Spells.Contains(spell)) throw new ArgumentException("It is'not my spell");

            if (Mana.Value < spell.GetComponent<Cost>().Value) throw new ArgumentException("Low mana");

            Mana.DeltaMana(-spell.GetComponent<Cost>().Value);
            spell.GetComponent<Spell>().Use(target);
            Container.Session.DelObject(spell);
            if (OnSpellCast != null)
                OnSpellCast.Invoke(new SessionChange("Spell was casted", Container.ID));
        }

        public override void Appear(Container container)
        {
            base.Appear(container);
            if (Container.Session != null) OnContainerAppears(container);
        }
        public override void Wanish()
        {
            base.Wanish();
            if (Container.Session != null) OnContainerVanished(Container);
        }
        protected override void OnContainerAppears(Container container)
        {
            base.OnContainerAppears(container);
            OnSpellCast += container.Session.SessionChanged;
            foreach (var f in Spells) container.Session.AddObject(f);
        }
        protected override void OnContainerVanished(Container container)
        {
            base.OnContainerVanished(container);
            OnSpellCast -= container.Session.SessionChanged;
            foreach (var f in Spells) f.SetSession(container.Session);
        }

        public event NonParametrizedEventHandler<SessionChange> OnSpellCast;
    }
}