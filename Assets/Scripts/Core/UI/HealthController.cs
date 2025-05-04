using System;
using Core.Configs;
using Core.Enemy;
using Infrastructure.DI.Injector;
using UnityEngine;

namespace Core.UI
{
    public class HealthController
    {
        [Inject] private Camera _camera;
        [Inject] private  Health _health;
        
        private HealthView _view;
        private Action _onDeath;
        
        public void Construct(HealthView healthView, EnemyConfig enemyConfig)
        {
            _health.Construct(enemyConfig.Health);
            _view = healthView;
            _view.Construct(_camera);
            _view.SetValue(_health.Normalized);
        }

        public void SubscribeToDeath(Action onDeath)
        {
            _onDeath = onDeath;
        }

        public void TakeDamage(int damage)
        {
            _health.TakeDamage(damage);
            _view.SetValue(_health.Normalized);
            
            if (_health.Normalized <= 0f)
                _onDeath?.Invoke();
        }

        public void RestoreHealth()
        {
            _health.RestoreHealth();
            _view.SetValue(_health.Normalized);
        }
    }
}