using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.GameControll
{
    public class GameTask
    {
        public event Action OnCompleted;
        
        public int EnemyCount => _enemySpawnPoints.Count;
        public Transform[] GetPositions() => _enemySpawnPoints.ToArray();
        
        private List<Transform> _enemySpawnPoints;
        private readonly int _purposeCount;
        private int _currentKills;

        public GameTask( List<Transform> enemySpawnPoints)
        {
            _enemySpawnPoints = enemySpawnPoints;
            _purposeCount = enemySpawnPoints.Count;
            _currentKills = 0;
        }
        public void Reset()
        {
            _currentKills = 0;
        }
        public void AttachEnemy(Enemy.Enemy enemy)
        {

            if (enemy.IsDeath)
            {
                enemy.OnDeath -= OnEnemyKilled;
                enemy.Recovery();
            }

            enemy.OnDeath += OnEnemyKilled;
        }
        private void OnEnemyKilled(Enemy.Enemy enemy)
        {
            enemy.OnDeath -= OnEnemyKilled; 

            _currentKills++;
            if (_currentKills >= _purposeCount)
            {
                OnCompleted?.Invoke(); 
            }
        }
    }
}