
using UnityEngine;

namespace RBGame
{

    public class BossKilled : Command
    {
    }


    public class BossRecieveDamage : Command
    {
        public IBoss boss;
        public  Color32 BallColor { get; set; }
    }

    
    public class BossShot : Command
    {
    }

    public partial class GameEventSystem
    {
        public EventHandler<BossKilled> OnBossKilled = new EventHandler<BossKilled>();
        public EventHandler<BossRecieveDamage> OnBossRecieveDamage = new EventHandler<BossRecieveDamage> ();
        public EventHandler<BossShot> OnBossShot = new EventHandler<BossShot> ();
    }
}