using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.ObjectPool
{
    public class EnemyPool
    {
        private GameObjectPool _pool;

        public void Construct(int totalCount, Func<GameObject> factory)
        {
            _pool = new GameObjectPool(factory, totalCount);
        }

        public List<GameObject> SpawnAtPositions(int count, Transform[] spawnPoints)
        {
            List<GameObject> spawnedEnemies = new();

            for (int i = 0; i < count; i++)
            {
                if (_pool.PoolCount <= 0)
                {
                    break; 
                }

                GameObject enemy = _pool.Get();
                Transform point = spawnPoints[i % spawnPoints.Length];

                enemy.transform.position = point.position;
                enemy.transform.rotation = point.rotation;

                spawnedEnemies.Add(enemy);
            }

            return spawnedEnemies;
        }

        public void Return(GameObject enemy)
        {
            enemy.SetActive(false);
            _pool.Return(enemy);
        }
    }
}