
namespace RBGame
{
    /// <summary>
    /// 
    /// </summary>
    public struct PlayerShot : Command
    {
        public bool isMissShot;
        public bool isFirstMissShot;
    }

    public partial class GameEventSystem
    {
        public EventHandler<PlayerShot> OnPlayerShot = new EventHandler<PlayerShot>();
    }
}