using System.Collections.Generic;
using Core.ObjectPool;
using Infrastructure.DI.Injector;
using UnityEngine;

namespace Core.Enemy
{
    public class EnemyManager
    {
        [Inject] private EnemyPool _enemyPool;

        public IReadOnlyList<Enemy> ActiveEnemies => _activeEnemies;
        
        private List<GameObject> _ragdollEnemiesList = new();
        private List<Enemy> _activeEnemies = new();
        private List<GameObject> _oldRagdollEnemiesList = new();

        public void SpawnEnemies(Transform[] enemiesPoints)
        {
            var enemies = _enemyPool.SpawnAtPositions(enemiesPoints.Length, enemiesPoints);
            
            for (int i = 0; i < enemies.Count; i++)
            {
                SubscribeToDeath(enemies[i]);
            }
        }

        public void OnTaskCompleted(int stepNumber)
        {
            if (stepNumber < 2)
            {
                return; 
            }
            
            for (int i = 0; i < _oldRagdollEnemiesList.Count; i++)
            {
                ReturnEnemyInPool(_oldRagdollEnemiesList[i]);
            }
            _oldRagdollEnemiesList.Clear();
            
            _oldRagdollEnemiesList = new List<GameObject>(_ragdollEnemiesList);
            
            _ragdollEnemiesList.Clear();
        }

        public void ReloadAllEnemies()
        {
            if (_oldRagdollEnemiesList.Count > 0)
            {
                for (int i = 0; i < _oldRagdollEnemiesList.Count; i++) 
                    ReturnEnemyInPool(_oldRagdollEnemiesList[i]);
                _oldRagdollEnemiesList.Clear();
            }

            if (_activeEnemies.Count > 0)
            {
                for (int i = 0; i < _activeEnemies.Count; i++) 
                    ReturnEnemyInPool(_activeEnemies[i].gameObject);
                _activeEnemies.Clear();
            }


            if ( _ragdollEnemiesList.Count > 0)
            {
                for (int i = 0; i < _ragdollEnemiesList.Count; i++) 
                    ReturnEnemyInPool(_ragdollEnemiesList[i]);
                _ragdollEnemiesList.Clear();
            }
        }

        private void ReturnEnemyInPool(GameObject enemyObject)
        {
            if (enemyObject.TryGetComponent(out Enemy enemy))
            {
                enemy.OnDeath -= HandleEnemyDeath;
                enemy.Recovery();
            }

            _enemyPool.Return(enemyObject);
        }

        private void SubscribeToDeath(GameObject enemyGO)
        {
            if (enemyGO.TryGetComponent(out Enemy enemy))
            {
                enemy.OnDeath += HandleEnemyDeath;
                _activeEnemies.Add(enemy);
            }
        }
        
        private void HandleEnemyDeath(Enemy enemy)
        {
            _ragdollEnemiesList.Add(enemy.gameObject);
            _activeEnemies.Remove(enemy);
        }

    }
}