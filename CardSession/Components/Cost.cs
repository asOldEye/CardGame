using System;

namespace CardSessionServer
{
    /// <summary>
    /// Стоимость карты
    /// </summary>
    [Serializable]
    public class Cost : Component
    {
        /// <summary>
        /// Стоимость карты
        /// </summary>
        [Interptered("CostValue")]
        public int Value { get; }
        public Cost(int cost)
        {
            if ((Value = cost) < 0) throw new ArgumentException("Cost must be >= zero");
        }
    }
}