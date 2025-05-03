using System;
using Core.Configs;
using Core.UI;
using Infrastructure.DI.Injector;
using UnityEngine;

namespace Core.Enemy
{
    public class Enemy : MonoBehaviour, IDamagable
    {
        [SerializeField] private HealthView _healthView;
        [SerializeField] Rigidbody _mainBone;
        
        public bool IsDeath => _isDeath;
        
        public event Action<Enemy> OnDeath; 
        private RagdollHandler _ragdollHandler;

        private Animator _animator;
        private HealthController _healthController;
        private Transform _playerTransform;
        private bool _isDeath;


        [Inject]
        public void Construct(Health health,RagdollHandler ragdollHandler, EnemyConfig enemyConfig, Player.Player player)
        {
            _playerTransform = player.transform;
            _animator = GetComponent<Animator>();
            health.Construct(enemyConfig.Health);
            _healthController = new HealthController(health, _healthView, Kill);
            _ragdollHandler = ragdollHandler;
            _ragdollHandler.Initialize(GetComponentsInChildren<Rigidbody>(), _mainBone, transform, enemyConfig.ThrowForce);
            _isDeath = false;
        }

        private void Kill()
        {
            _animator.enabled = false;
            _ragdollHandler.Enable();
            _ragdollHandler.ThrowRagdoll(_playerTransform);
            _isDeath = true;
            OnDeath?.Invoke(this);
        }

        public void TakeDamage(int damage)
        {
            _healthController.TakeDamage(damage);
        }

        public void Recovery()
        {
            _healthController.RestoreHealth();
            _ragdollHandler.Disable();
            _animator.enabled = true;
            _isDeath = false;
        }
    }
}