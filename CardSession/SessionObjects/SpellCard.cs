using System.Collections.Generic;

namespace CardSessionServer
{
    /// <summary>
    /// Карта заклинания
    /// </summary>
    [Interptered("SpellCard")]
    public class SpellCard : Card
    {
        public SpellCard(int cardCost, List<Modifier> modifiers) : base(cardCost)
        {
            try
            { AddComponent(new Spell(modifiers)); }
            catch { throw; }
        }
    }
}