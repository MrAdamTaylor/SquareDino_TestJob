using System;
using Core.Configs;
using Core.UI;
using Infrastructure.DI.Injector;
using UnityEngine;

namespace Core.Enemy
{
    public class Enemy : MonoBehaviour, IDamageable
    {
        [SerializeField] private HealthView _healthView;
        [SerializeField] private Rigidbody _mainBone;
        [SerializeField] private Animator _animator;
        
        public bool IsDeath => _isDeath;
        
        public event Action<Enemy> OnDeath; 
        
        private RagdollHandler _ragdollHandler;
        private HealthController _healthController;
        private Transform _playerTransform;
        
        private bool _isDeath;


        [Inject]
        public void Construct(HealthController healthController,RagdollHandler ragdollHandler, EnemyConfig enemyConfig, Player.Player player)
        {
            _playerTransform = player.transform;
            _healthController = healthController;
            _healthController.Construct(_healthView, enemyConfig);
            _healthController.SubscribeToDeath(Kill);
            _ragdollHandler = ragdollHandler;
            _ragdollHandler.Initialize(GetComponentsInChildren<Rigidbody>(), _mainBone, transform, enemyConfig.ThrowForce);
            _isDeath = false;
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

        private void Kill()
        {
            _animator.enabled = false;
            _ragdollHandler.Enable();
            _ragdollHandler.ThrowRagdoll(_playerTransform);
            _isDeath = true;
            OnDeath?.Invoke(this);
        }
    }
}