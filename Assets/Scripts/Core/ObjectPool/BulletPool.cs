using System;
using UnityEngine;

namespace Core.ObjectPool
{
    public class BulletPool 
    {
        private GameObjectPool _pool;

        public void Construct(int bulletCount, Func<GameObject> factoryFunction)
        {
            _pool = new GameObjectPool(factoryFunction, bulletCount);
        }

        public void Spawn(Vector3 position, Quaternion rotation)
        {
            if (_pool.PoolCount <= 0)
            {
                Debug.LogWarning("Bullet Pool is empty!");
                return;
            }

            GameObject bullet = _pool.Get();

            bullet.transform.position = position;
            bullet.transform.rotation = rotation;
        }

        public void Return(GameObject bullet)
        {
            bullet.SetActive(false);
            _pool.Return(bullet);
        }

        public int GetUnpooledCount() => _pool.GetUnpooledCount();
    }

    public class EnemyPool
    {
        private GameObjectPool _pool;

        public void Construct(int totalCount, Func<GameObject> factory)
        {
            _pool = new GameObjectPool(factory, totalCount);
        }

        public void SpawnAtPositions(int count, Transform[] spawnPoints)
        {
            for (int i = 0; i < count; i++)
            {
                if (_pool.PoolCount <= 0)
                {
                    Debug.Log("<color=red>Pool exhausted</color>");
                    return;
                }

                GameObject enemy = _pool.Get();
                Transform point = spawnPoints[i % spawnPoints.Length];

                enemy.transform.position = point.position;
                enemy.transform.rotation = point.rotation;
            }
        }

        public void Return(GameObject enemy)
        {
            enemy.SetActive(false);
            _pool.Return(enemy);
        }
    }
}