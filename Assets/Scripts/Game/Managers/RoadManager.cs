using UnityEngine;

namespace RBGame
{
    /// <summary>
    /// Система генерации дорожных секций + облака
    /// </summary>
    public class RoadManager : GModel
    {
        [SerializeField] Transform wallLine;
        [SerializeField] Transform startStageSplitter;
        [SerializeField] Transform finishStageSplitter;
        [SerializeField] Transform[] cloud;

        int nextPos = 0;
        float prevPlayerPos = 0;


        private void Start()
        {
            for (int i = 0; i < 3; i++)
                Gen();

            //  стартовый бокс
            {
                var sb = Instantiate(startStageSplitter);
                sb.position = -17 * Vector3.forward;
                sb.gameObject.AddComponent<DestroyInvis>().dist = 40;
            }

            //  финишный бокс
            {
                var sb = Instantiate(finishStageSplitter);
                sb.position = Vector3.forward * trip.FinishDist + Vector3.up * cx.GetUpOffset(trip.FinishDist);
                sb.gameObject.AddComponent<DestroyInvis>().dist = 40;
            }
        }


        private void GenCloudLeft(float nextPos)
        {
            float xOffset = -80;
            var r = cloud[(int)(Random.value * cloud.Length)];
            Vector3 pos = Vector3.zero;
            
            pos.z = nextPos + Random.value * 100;
            pos.y = -5 - Random.value * 10 + cx.GetUpOffset(pos.z);
            pos.x = xOffset + Random.value * 60;

            var cl = Instantiate(r, pos, Quaternion.identity);
            cl.gameObject.AddComponent<DestroyInvis>().dist = 40;
        }

        private void GenCloudRight (float nextPos)
        {
            float xOffset = 10;
            var r = cloud[(int)(Random.value * cloud.Length)];
            Vector3 pos = Vector3.zero;

            pos.z = nextPos + Random.value * 100;
            pos.y = -5 - Random.value * 10 + cx.GetUpOffset (pos.z);
            pos.x = xOffset + Random.value * 40;

            var cl = Instantiate (r, pos, Quaternion.identity);
            cl.gameObject.AddComponent<DestroyInvis> ().dist = 40;

        }

        private void Gen()
        {
            //  После босса облака не генерить. А то вид портит
            
            if (!trip.IsFinished)
            {
                for (int i = 0; i < 3; i++)
                    GenCloudLeft(nextPos);
                
                for (int i = 0; i < 2; i++)
                    GenCloudRight (nextPos);
            }
            

            var wall = Instantiate(wallLine);
            wall.position = Vector3.forward * nextPos;
            wall.gameObject.AddComponent<DestroyInvis>().dist = 120;
            nextPos += 100;
        }


        private void Update()
        {
            if (bot.position.z - prevPlayerPos > 100)
            {
                Gen();
                prevPlayerPos = bot.position.z;
            }
        }
    }
}