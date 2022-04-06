
using UnityEngine;

namespace RBGame
{
    public class BossRecieveDamage 
    {
        public IBoss boss;
        public  Color32 BallColor { get; set; }
    }

    public partial class GameEventSystem
    {
        public MEventHandler<EmtyData> OnBossKilled = new MEventHandler<EmtyData> ();
        public MEventHandler<BossRecieveDamage> OnBossRecieveDamage = new MEventHandler<BossRecieveDamage> ();
        public MEventHandler<EmtyData> OnBossShot = new MEventHandler<EmtyData> ();
    }
}