using UnityEngine;

namespace Core.Configs
{
    [CreateAssetMenu(fileName = "Enemy", menuName = "Enemy")]
    public class EnemyConfig : ScriptableObject
    {
        public int Health;
        
        
        public float ThrowForce;
    }
}