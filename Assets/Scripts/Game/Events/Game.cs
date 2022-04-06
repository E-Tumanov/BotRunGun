
namespace RBGame
{
    public class EmtyData
    {
    }


    public struct ButtonPress
    {
        public bool isPressed;
        public GAME_BUTTON btn;
    }

    public struct BuyBullet
    {
        public bool isStarted;
        public bool isCanceled;
        public int value;
    }

    public struct CollectGroup
    {
        public StageGroupInfo grp;
    }

    public struct PauseEvent
    {
        public bool isPause;
    }

    public partial class GameEventSystem
    {
        public MEventHandler<CollectGroup> OnCollectGroup = new MEventHandler<CollectGroup> ();
        public MEventHandler<PauseEvent> OnPauseEvent = new MEventHandler<PauseEvent> ();
        public MEventHandler<BuyBullet> OnBuyBullet = new MEventHandler<BuyBullet> ();
        public MEventHandler<ButtonPress> OnButtonPress = new MEventHandler<ButtonPress> ();


        public MEventHandler<bool> OnGameOver = new MEventHandler<bool> ();
    }
}