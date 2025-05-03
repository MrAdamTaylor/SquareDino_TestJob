using UnityEngine;

namespace Core.Configs
{
    [CreateAssetMenu(fileName = "Enemy", menuName = "Enemy")]
    public class EnemyConfig : ScriptableObject
    {
        public int Health;
        
        [Header("The force with which the ragdoll flies off")]
        public float ThrowForce;
    }
}