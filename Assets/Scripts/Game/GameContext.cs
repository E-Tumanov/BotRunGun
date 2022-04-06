using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RBGame.Factory;
using System;
using UnityEngine.Assertions;

namespace RBGame
{
    public static class BroadCastMessage
    {
        static GameEventSystem _eve;
        public static GameEventSystem Eve
        {
            get
            {
                if (_eve == null)
                    _eve = new GameEventSystem ();
                return _eve;
            }
        }
    }


    /// <summary>
    /// 
    /// Bootstrapper
    /// 
    /// </summary>
    public class GameContext : GameContextBase
    {
        public static GameContext Instance;

        [HideInInspector] public GamePad GamePad;
        [HideInInspector] public RoadManager RoadManager;
        [HideInInspector] public BattleManager BattleManager;
        [HideInInspector] public BallsManager BallsManager;
        [HideInInspector] public CoinManager CoinManager;
        [HideInInspector] public TripManager TripManager;
        [HideInInspector] public Stage CurrStage;
        [HideInInspector] public IPlayerModel PlayerModel;
        [HideInInspector] public BossInfo BossInfo;
        [HideInInspector] public BossFactory BossFactory;
        [HideInInspector] public IItemFactory ItemFactory;
        [HideInInspector] public StageGroupManager GroupManager;

        public CfgRoad RoadConfig;

        public Color32 GetColorByNum (int colorNum)
        {
            if (colorNum < 0 || colorNum >= RoadConfig.ColorStack.Count)
                return Color.white;

            return RoadConfig.ColorStack[colorNum];
        }


        [SerializeField] string roadName;

        private void Awake ()
        {
            Instance = this;
            SceneNameResolver ();

            ConfDB.LoadConfigsAndSave ();

            GamePad = FindObjectOfType<GamePad> ();

            {
                ItemFactory = FindObjectOfType<ItemFactory> ();
                CurrStage = StageManager.Create (G.stageFileName, ItemFactory);
                GroupManager = new StageGroupManager ();
                GroupManager.GroupList (CurrStage.RebuildCoinList ());
            }

            BallsManager = GetComponent<BallsManager> ();
            CoinManager = GetComponent<CoinManager> ();
            TripManager = GetComponent<TripManager> ();
            RoadManager = FindObjectOfType<RoadManager> ();

            PlayerModel = FindObjectOfType<PlayerMover> ();
            Assert.IsNotNull (PlayerModel);


            BattleManager = GetComponent<BattleManager> ();
            BossFactory = FindObjectOfType<BossFactory> ();

            BossInfo = CurrStage.bossInfo;
        }

        public float GetUpOffset (float zpos)
        {
            float mul = 1 - Mathf.Clamp01 ((TripManager.FinishDist - zpos) / 200.0f);
            return Mathf.Lerp (RoadConfig.GetUpOffset (zpos), 0, mul);
        }

        private void OnDestroy ()
        {
            Instance = null;
        }

        void SceneNameResolver ()
        {

#if (UNITY_EDITOR)
            if (roadName != null && roadName != "")
            {
                G.stageFileName = roadName + ".json";
            }
#endif

            if (G.stageFileName == string.Empty)
            {
                Debug.LogWarning ("Map name undef");
                G.stageFileName = "101.json";
            }
        }
    }


    /// <summary>
    /// Игровые системы/объекты которым нужен GameContext(DI) наследуют это
    /// </summary>
    public class GModel : MonoBehaviour
    {
        protected GameContext cx => GameContext.Instance;

        protected GameEventSystem Eve => BroadCastMessage.Eve;

        protected IPlayerModel bot => cx.PlayerModel;
        protected BossInfo bossInfo => cx.BossInfo;
        protected BallsManager ballsManager => cx.BallsManager;
        protected RoadManager roadMgr => cx.RoadManager;
        protected TripManager trip => cx.TripManager;
        protected CoinManager coin => cx.CoinManager;
        protected BattleManager battleMgr => cx.BattleManager;
        protected BossFactory factoryBoss => cx.BossFactory;
        protected Stage stage => cx.CurrStage;

        protected CfgRoad roadConfig => cx.RoadConfig;
        protected void DG (string msg) => Debug.Log (msg);
    }
}