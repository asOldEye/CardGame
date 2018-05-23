using System.Collections.Generic;

namespace CardSessionServer
{
    /// <summary>
    /// Солдат, способный создавать заклинания
    /// </summary>
    [Interptered("CasterSoliderCard")]
    public class CasterSoliderCard : SoliderCard
    {
        public CasterSoliderCard(int cardCost, int healthMax, int health, int attackPower, int moveRadius, int manaMax, int mana, List<SpellCard> spells)
            : base(cardCost, healthMax, health, attackPower, moveRadius)
        {
            try
            {
                AddComponent(new Mana(manaMax, mana));
                AddComponent(new SpellCaster(spells));
            }
            catch { throw; }
        }
    }
}