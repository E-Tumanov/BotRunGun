
namespace RBGame
{
    /// <summary>
    /// 
    /// </summary>
    /// 

    public struct PlayerImpact : Command
    {
    }

    public partial class GameEventSystem
    {
        public EventHandler<PlayerImpact> OnPlayerImpact = new EventHandler<PlayerImpact>();
    }
}