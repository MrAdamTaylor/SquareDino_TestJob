using UnityEngine;

namespace Core.Configs
{
    [CreateAssetMenu(fileName = "Bullet", menuName = "Bullet")]
    public class BulletConfig : ScriptableObject
    {
    
        public int bulletCountsInPool;
    
        public float bulletSpeed;
        public int bulletDamage;
        public int bulletLifeTime;
    }
}
