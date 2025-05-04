using UnityEngine;

namespace Core.Configs
{
    [CreateAssetMenu(fileName = "Bullet", menuName = "Bullet")]
    public class BulletConfig : ScriptableObject
    {
    
        public int BulletCountsInPool;
    
        public float BulletSpeed;
        public int BulletDamage;
        public int BulletLifeTime;
    }
}
