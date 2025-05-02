using System;
using UnityEngine;

namespace Core.Enemy
{
    public class Health 
    {
        private int _current;
        private int _max;

        public void Construct(int max)
        {
            _max = max;
            _current = max;
        }

        public void TakeDamage(int damage)
        {
            _current -= damage;
            _current = Mathf.Max(_current, 0);
        }

        public float Normalized => (float)_current / _max;
    }
}