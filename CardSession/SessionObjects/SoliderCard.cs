using CardSessionShared;

namespace CardSessionServer
{
    /// <summary>
    /// Карта слодата
    /// </summary>
    [Interptered("SoliderCard")]
    public class SoliderCard : Card
    {
        public SoliderCard(int cardCost, int healthMax, int health, int attackPower, int moveRadius) 
            : base(cardCost)
        {
            try
            {
                AddComponent(new Destroyable(healthMax, health));
                AddComponent(new Attacker(attackPower, health));
                AddComponent(new Walkable(new Position(0,0), false, moveRadius));
            }
            catch { throw; }
        }
    }
}