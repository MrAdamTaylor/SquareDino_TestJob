using UnityEngine;

namespace Core.Configs
{
    [CreateAssetMenu(fileName = "Bullet", menuName = "Bullet")]
    public class BulletConfig : ScriptableObject
    {
    
        public int bulletsInPool;
    
        public float bulletSpeed;
        public int bulletDamage;
        public int bulletLifeTime;
    }
}
