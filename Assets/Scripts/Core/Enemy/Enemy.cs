using Core.Configs;
using Core.UI;
using Infrastructure.DI.Injector;
using UnityEngine;

namespace Core.Enemy
{
    public class Enemy : MonoBehaviour, IDamagable
    {
        [SerializeField] private HealthView _healthView;
    
        private Animator _animator;
        private HealthController _healthController;
        private RagdollHandler _ragdollHandler;
    
        [Inject]
        public void Construct(Health health,RagdollHandler ragdollHandler, EnemyConfig enemyConfig)
        {
            _animator = GetComponent<Animator>();
            health.Construct(enemyConfig.Health);
            _healthController = new HealthController(health, _healthView, Kill);
            _ragdollHandler = ragdollHandler;
            _ragdollHandler.Initialize(GetComponentsInChildren<Rigidbody>());
        }

        private void Kill()
        {
            _animator.enabled = false;
            _ragdollHandler.Enable();
        }

        public void TakeDamage(int damage)
        {
            _healthController.TakeDamage(damage);
        }
    }
}