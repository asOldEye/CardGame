using AuxiliaryLibrary;
using CardSessionShared;
using System;

namespace CardSessionServer
{
    [Serializable]
    public class Walkable : Positionable
    {
        public Walkable(Position position, bool positioned, int moveRadius) : base(position, positioned)
        { MoveRadius = moveRadius; }

        int moveRadius;
        /// <summary>
        /// Радиус передвижения за 1 ход
        /// </summary>
        [Interptered("MoveRadius")]
        public int MoveRadius
        {
            get => moveRadius;
            protected set
            {
                if (value < 0) value = 0;
                int delta = moveRadius - (moveRadius = value);
                if (OnMoveRadiusChanged != null)
                    OnMoveRadiusChanged.Invoke(new SessionChange("Move radius has changed on " + delta, Container.ID));
            }
        }
        [Modified]
        public virtual void DeltaMoveRadius(int delta)
        { MoveRadius += delta; }

        /// <summary>
        /// Передвинуть на новую позицию
        /// </summary>
        /// <param name="position"></param>
        [Modified]
        [ControllerCommand(false)]
        public void Move(Position position)
        {
            if (Position.Distance(position, Position) > MoveRadius)
                throw new ArgumentException("Can't move because too far");
            Container.Session.Map.FindByPosition(Position).Positioned = null;
            Container.Session.Map.FindByPosition(position).Positioned = Container;
            if (OnPositionChanged != null)
                OnPositionChanged.Invoke(new SessionChange("Position was changed", Container.ID));
        }

        public override void Appear(Container container)
        {
            base.Appear(container);
            if (container.Session != null) OnContainerAppears(container);
        }
        public override void Wanish()
        {
            base.Wanish();
            if (Container.Session != null) OnContainerVanished(Container);
        }
        protected override void OnContainerAppears(Container container)
        {
            base.OnContainerAppears(container);
            OnMoveRadiusChanged += container.Session.SessionChanged;
            OnPositionChanged += container.Session.SessionChanged;
        }
        protected override void OnContainerVanished(Container container)
        {
            base.OnContainerVanished(container);
            OnMoveRadiusChanged -= container.Session.SessionChanged;
            OnPositionChanged -= container.Session.SessionChanged;
        }

        public event NonParametrizedEventHandler<SessionChange> OnMoveRadiusChanged;
        public event NonParametrizedEventHandler<SessionChange> OnPositionChanged;
    }
}