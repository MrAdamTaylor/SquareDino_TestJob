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
            enemy.OnDeath += OnEnemyKilled;
        }

        private void OnEnemyKilled(Enemy enemy)
        {
            enemy.OnDeath -= OnEnemyKilled; 

            _currentKills++;
            if (_currentKills >= _purposeCount)
            {
                OnCompleted?.Invoke(); 
                OnCompleted = null;    
            }
        }

        public void OutputInfo()
        {
            Debug.Log($"<color=magenta>Camera Look Target: {_cameraLookTarget.gameObject.name} and SpawnPoints is {_enemySpawnPoints.Count} </color>");
        }
    
        public int EnemyCount => _enemySpawnPoints.Count;

        public Transform[] GetPositions() => _enemySpawnPoints.ToArray();
    }
}