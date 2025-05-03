using System;
using System.Collections.Generic;
using Core.Enemy;
using UnityEngine;

namespace Infrastructure.StateMachine
{
    public class GameTask
    {
        private Transform _cameraLookTarget;
        private List<Transform> _enemySpawnPoints;
        private int _purposeCount;
        private int _currentKills;
        
        public event Action OnCompleted;

        public GameTask(Transform cameraLookTarget, List<Transform> enemySpawnPoints)
        {
            _cameraLookTarget = cameraLookTarget;
            _enemySpawnPoints = enemySpawnPoints;
            _purposeCount = enemySpawnPoints.Count;
            _currentKills = 0;
        }
        
        public void AttachEnemy(Enemy enemy)
        {

            if (enemy.IsDeath)
            {
                enemy.OnDeath -= OnEnemyKilled;
                enemy.Recovery();
            }

            enemy.OnDeath += OnEnemyKilled;
        }

        private void OnEnemyKilled(Enemy enemy)
        {
            enemy.OnDeath -= OnEnemyKilled; 

            _currentKills++;
            if (_currentKills >= _purposeCount)
            {
                OnCompleted?.Invoke(); 
                //OnCompleted = null;    
            }
        }
    
        public int EnemyCount => _enemySpawnPoints.Count;

        public Transform[] GetPositions() => _enemySpawnPoints.ToArray();

        public void Reset()
        {
            _currentKills = 0;
        }
    }
}