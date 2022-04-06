
namespace RBGame
{
    public struct PlayerShot
    {
        public bool isMissShot;
        public bool isFirstMissShot;
    }

    public struct PlayerInteractItem
    {
        public IMapItem item;
    }

    public partial class GameEventSystem
    {
        public MEventHandler<PlayerInteractItem> OnPlayerInteractItem = new MEventHandler<PlayerInteractItem> ();
        public MEventHandler<bool> OnPlayerChangeMove = new MEventHandler<bool> ();
        public MEventHandler<EmtyData> OnPlayerKilled = new MEventHandler<EmtyData> ();
        public MEventHandler<EmtyData> OnPlayerImpact = new MEventHandler<EmtyData> ();
        public MEventHandler<EmtyData> OnPlayerFinished = new MEventHandler<EmtyData> ();
        public MEventHandler<EmtyData> OnPlayerStopped2Fight = new MEventHandler<EmtyData> ();
        public MEventHandler<PlayerShot> OnPlayerShot = new MEventHandler<PlayerShot> ();
    }
}