
namespace RBGame
{
    /// <summary>
    /// 
    /// </summary>
    public struct PlayerKilled : Command
    {
    }

    public partial class GameEventSystem
    {
        public EventHandler<PlayerKilled> OnPlayerKilled = new EventHandler<PlayerKilled>();
    }
}