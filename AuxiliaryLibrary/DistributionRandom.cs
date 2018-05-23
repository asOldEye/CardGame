using System;

namespace AuxiliaryLibrary
{
    /// <summary>
    /// Генератор псевдослучайных чисел
    /// </summary>
    public class DistributionRandom : Random
    {
        /// <summary>
        /// Происходит ли событие с заданной вероятностью от нуля до ста
        /// </summary>
        /// <param name="percent">Процент [0,100]</param>
        /// <returns></returns>
        public bool NextPercent(int percent)
        {
            if (percent < 0 || percent > 100)
                throw new ArgumentException("Percent value must be in the interim [0,100]");
            return base.Next(0, 101) < percent;
        }
    }
}