using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RBGame.Factory
{
    public class WallFactory : MonoBehaviour
    {
        [SerializeField] Transform wall_v1;
        [SerializeField] Transform[] cloud;

        public virtual Transform CreateWall() 
        {
            return Instantiate(wall_v1);
        }

        public virtual Transform CreateCloud(float nextPos)
        {
            var r = cloud[(int)(Random.value * cloud.Length)];
            Vector3 pos = Vector3.zero;
            pos.z = Random.value * 20;
            pos.y = 4 + Random.value * 4.5f;
            pos.x = 4 - Random.value * 10;
            var cl = Instantiate(r, Vector3.forward * nextPos + pos, Quaternion.identity);
            cl.gameObject.AddComponent<DestroyInvis>().dist = 40;

            return cl;
        }
    }
}