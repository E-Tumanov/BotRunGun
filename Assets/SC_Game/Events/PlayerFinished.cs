
namespace RBGame
{
    /// <summary>
    /// 
    /// </summary>

    public struct PlayerFinished : Command
    {
    }

    public partial class GameEventSystem
    {
        public EventHandler<PlayerFinished> OnPlayerFinished = new EventHandler<PlayerFinished>();
    }
}