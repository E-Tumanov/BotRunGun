

namespace RBGame
{
    /// <summary>
    /// 
    /// </summary>

    public struct PauseEvent : Command
    {
        public bool isPause;
    }

    public partial class GameEventSystem
    {
        public EventHandler<PauseEvent> OnPauseEvent = new EventHandler<PauseEvent>();
    }
}