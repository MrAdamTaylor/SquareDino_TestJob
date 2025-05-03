using System;
using Core.Enemy;
using UnityEngine;

namespace Core.UI
{
    public class HealthController
    {
        private readonly Health _health;
        private readonly HealthView _view;
        private readonly Action _onDeath;

        public HealthController(Health health, HealthView view, Action onDeath = null)
        {
            _health = health;
            _view = view;
            _onDeath = onDeath;
            
            _view.SetValue(_health.Normalized);
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