
namespace RBGame
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    public struct ButtonPress : Command
    {
        public bool isPressed;
        public GAME_BUTTON btn;
    }

    public partial class GameEventSystem
    {
        public EventHandler<ButtonPress> OnButtonPress = new EventHandler<ButtonPress>();
    }
}