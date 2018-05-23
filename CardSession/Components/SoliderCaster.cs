using AuxiliaryLibrary;
using CardSessionShared;
using System;
using System.Collections.Generic;

namespace CardSessionServer
{
    [Serializable]
    public class SoliderCaster : Component
    {
        public SoliderCaster(List<SoliderCard> soliders)
        {
            if ((Soliders = soliders) == null) throw new ArgumentNullException("Null soliders list");
        }

        /// <summary>
        /// Колода солдат игрока
        /// </summary>
        [Interptered("Soliders")]
        public List<SoliderCard> Soliders { get; } = new List<SoliderCard>();

        /// <summary>
        /// Вытаскивает карту солдата из колоды на стол
        /// </summary>
        [ControllerCommand]
        [Modified]
        public void CastSolider(SoliderCard solider, Position position)
        {
            if (Container.Session == null) throw new ArgumentException("Not in session");
            if (position.CompareTo(Container.Session.Map.Size) >= 0) throw new ArgumentException("Too big position");
            var f = Container.Session.Map.FindByPosition(position);
            if (f.Positioned != null) throw new ArgumentException("Another target already in this position");
            var Mana = Container.GetComponent<Mana>();
            if (Mana == null) throw new ArgumentException("No mana in container");
            if (solider == null) throw new ArgumentNullException(nameof(solider));
            if (!Soliders.Remove(solider)) throw new ArgumentException("It is'not my spell");

            if (Mana.Value < solider.GetComponent<Cost>().Value) throw new ArgumentException("Low mana");
            Mana.DeltaMana(-solider.GetComponent<Cost>().Value);

            solider.SetOwner(Container.Owner);
            solider.GetComponent<Positionable>().Position = position;
            solider.GetComponent<Positionable>().Positioned = true;

            if (OnSoliderCasted != null)
                OnSoliderCasted.Invoke(new SessionChange("Solider was casted ", Container.ID));
        }

        protected override void OnContainerAppears(Container container)
        {
            base.OnContainerAppears(container);
            OnSoliderCasted += container.Session.SessionChanged;
            foreach (var f in Soliders) container.Session.AddObject(f);
        }
        protected override void OnContainerVanished(Container container)
        {
            base.OnContainerVanished(container);
            OnSoliderCasted -= container.Session.SessionChanged;
            foreach (var f in Soliders) f.Session.DelObject(f);
        }

        public override void Wanish()
        {
            base.Wanish();
            if (Container.Session != null)
                OnContainerVanished(Container);
        }
        public override void Appear(Container container)
        {
            base.Appear(container);
            if (container.Session != null)
                OnContainerAppears(Container);
        }

        /// <summary>
        /// Вызывается при вызове карты из колоды на стол
        /// </summary>
        public event NonParametrizedEventHandler<SessionChange> OnSoliderCasted;
    }
}