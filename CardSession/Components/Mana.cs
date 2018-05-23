using AuxiliaryLibrary;
using CardSessionShared;
using System;

namespace CardSessionServer
{
    /// <summary>
    /// Мана
    /// </summary>
    [Serializable]
    public class Mana : Component
    {
        /// <summary>
        /// Максимальное количество маны
        /// </summary>
        [Interptered("ManaMaxValue")]
        public int MaxValue { get; }
        int value;
        /// <summary>
        /// Текущее количество маны
        /// </summary>
        [Interptered("ManaValue")]
        public int Value
        {
            get => value;
            protected set
            {
                if (value < 0) value = 0;
                if (value > Value) value = Value;

                int delta = value - this.value;
                this.value = value;

                if (OnManaChanged != null)
                    OnManaChanged.Invoke(new SessionChange("Mana was changed on " + delta, Container.ID));
            }
        }
        /// <summary>
        /// Изменение количества маны
        /// </summary>
        [Modified]
        public void DeltaMana(int delta)
        { Value += delta; }

        public Mana(int maxValue, int value)
        {
            MaxValue = maxValue;
            Value = value;
        }
        public override void Appear(Container container)
        {
            base.Appear(container);
            if (container.Session != null)
                OnManaChanged += container.Session.SessionChanged;
        }

        /// <summary>
        /// Событие, вызывающееся при изменении количества маны
        /// </summary>
        public event NonParametrizedEventHandler<SessionChange> OnManaChanged;
    }
}