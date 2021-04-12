
namespace RBGame
{
    /// <summary>
    /// 
    /// </summary>

    public struct CollectGroup : Command
    {
        public StageGroupInfo grp;
    }

    public partial class GameEventSystem
    {
        public EventHandler<CollectGroup> OnCollectGroup = new EventHandler<CollectGroup>();
    }
}