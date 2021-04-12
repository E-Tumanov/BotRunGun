namespace RBGame
{

    /// <summary>
    /// 
    /// </summary>
    public struct BuyBullet : Command
    {
        public bool isStarted;
        public bool isCanceled;
        public int value;
    }

    public partial class GameEventSystem
    {
        public EventHandler<BuyBullet> OnBuyBullet = new EventHandler<BuyBullet>();
    }
}