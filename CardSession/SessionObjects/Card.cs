namespace CardSessionServer
{
    /// <summary>
    /// Карта
    /// </summary>
    public abstract class Card : Container
    {
        public Card(int cardCost)
        {
            try
            { AddComponent(new Cost(cardCost)); }
            catch { throw; }
        }
    }
}