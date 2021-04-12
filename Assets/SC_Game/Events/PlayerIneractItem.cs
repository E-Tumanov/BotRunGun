
namespace RBGame
{
    /// <summary>
    /// 
    /// </summary>
    public struct PlayerInteractItem : Command
    {
        public IMapItem item;
    }

    public partial class GameEventSystem
    {
        public EventHandler<PlayerInteractItem> OnPlayerInteractItem = new EventHandler<PlayerInteractItem>();
    }
}