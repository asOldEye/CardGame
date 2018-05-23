using AuxiliaryLibrary;
using CardSessionShared;
using System;

namespace CardSessionServer
{
    [Serializable]
    public class Attacker : Component
    {
        public Attacker(int attackPowerMax, int attackPower)
        {
            if ((AttackPowerMax = attackPowerMax) < 0)
                throw new ArgumentException("Wrong attack power max, it must be more not less than zero");
            if ((AttackPower = attackPower) < 0 || AttackPower > AttackPowerMax)
                throw new ArgumentException("Wrong attack power max, it must be more not less than zero and not bigger than attack power max");
        }

        public override void Appear(Container container)
        {
            base.Appear(container);
            if (container.Session != null)
            {
                OnPowerChanged += container.Session.SessionChanged;
                OnAttack += container.Session.SessionChanged;
            }
        }

        /// <summary>
        /// Максимальная сила атаки
        /// </summary>
        [Interptered("AttackPowerMax")]
        public int AttackPowerMax { get; private set; }

        private int attackPower;
        /// <summary>
        /// Сила атаки
        /// </summary>
        [Interptered("AttackPower")]
        public int AttackPower
        {
            get { return attackPower; }
            protected set
            {
                if (value < 1) value = 1;
                if (value > AttackPowerMax) value = AttackPowerMax;

                int delta = attackPower - value;
                attackPower = value;

                if (OnPowerChanged != null)
                    OnPowerChanged.Invoke(new SessionChange("Attack power changed to " + delta, Container.ID));
            }
        }

        /// <summary>
        /// Атака уничтожаемого объекта
        /// </summary>
        /// <param name="target">Атакуемый объект</param>
        [Modified]
        [ControllerCommand]
        public void Attack(Container target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (target.Owner == Container.Owner) throw new ArgumentException("One team players");
            Destroyable destr;
            if ((destr = target.GetComponent<Destroyable>()) == null)
                throw new ArgumentException("target have'nt Destroyable component");
            if (OnAttack != null)
                OnAttack.Invoke(new SessionChange("Attacks ", Container.ID));
            destr.DeltaHealth(-attackPower);
        }

        /// <summary>
        /// Изменение атаки объекта
        /// </summary>
        [Modified]
        public virtual void DeltaPower(int delta)
        { AttackPower += delta; }

        public event NonParametrizedEventHandler<SessionChange> OnPowerChanged;
        public event NonParametrizedEventHandler<SessionChange> OnAttack;
    }
}