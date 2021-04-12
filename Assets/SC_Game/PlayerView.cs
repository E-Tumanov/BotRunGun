using UnityEngine;

namespace RBGame
{
    /// <summary>
    /// Вид. Player.
    /// </summary>
    public class PlayerView : GModel
    {
        [SerializeField] Transform pups;
        [SerializeField] Transform wheelGroup;
        [SerializeField] Transform wheel;
        [SerializeField] Transform forwardArrow;
        [SerializeField] Transform dbScatterModel;
        [SerializeField] Transform FXTrail;


        private void Start()
        {
            // Под вопросом. Воможно для модели должен быть интерфейс MVP

            eve.OnPlayerImpact.AddListener(this, x => OnImpact());
            eve.OnPlayerKilled.AddListener(this, x => OnImpact());
            eve.OnBossKilled.AddListener(this, x => OnBossKilled());
        }

        void OnImpact()
        {
            //  Можно воскреснуть
            gameObject.SetActive(false);
            Instantiate(dbScatterModel, transform.position, Quaternion.identity);
        }

        void OnBossKilled()
        {
            FXTrail.GetComponent<ParticleSystem>().Stop();
        }


        void Update()
        {
            //   if (G.isPause)
            //     return;

            //forwardArrow.forward = G.playerMoveDir.normalized;
            /*
            pups.rotation = Quaternion.AngleAxis(bot.bodyRoll, Vector3.forward); // ROLL
            wheelGroup.rotation = Quaternion.AngleAxis(0.5f * bot.wheelYaw, Vector3.up);  // YAW
            wheel.transform.Rotate(new Vector3(45 * bot.levelSpeed / (0.28f * 3.1415f) * Time.deltaTime, 0, 0), Space.Self);
            transform.position = bot.position;
            */

            pups.rotation = Quaternion.AngleAxis (- 0.1f * bot.bodyRoll, Vector3.forward); // ROLL
            //wheelGroup.rotation = Quaternion.AngleAxis (15.5f * bot.wheelYaw, Vector3.up);  // YAW
            //wheel.transform.Rotate (new Vector3 (45 * bot.levelSpeed / (0.28f * 3.1415f) * Time.deltaTime, 0, 0), Space.Self);
            transform.position = bot.position;
        }
    }
}