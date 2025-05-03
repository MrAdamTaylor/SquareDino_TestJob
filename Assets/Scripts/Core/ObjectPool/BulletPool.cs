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
    }
}