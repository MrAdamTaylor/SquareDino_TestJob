using Core.Configs;
using Infrastructure.DI.Injector;
using Infrastructure.Factory;
using UnityEngine;

namespace Core.ObjectPool
{
    public class PoolBuilder
    {
        [Inject] private BulletPool _bulletPool;
        [Inject] private EnemyPool _enemyPool;
        
        private BulletFactory _bulletFactory;
        private EnemyFactory _enemyFactory;
        private Transform _globalParent;
        private BulletConfig _bulletConfig;
    
        public PoolBuilder(BulletFactory bulletFactory, EnemyFactory enemyFactory, Transform globalParent)
        {
            _enemyFactory = enemyFactory;
            _bulletFactory = bulletFactory;
            _globalParent = globalParent;
        }

        [Inject]
        public void Construct(BulletConfig bulletConfig)
        {
            _bulletConfig = bulletConfig;
        }

        public void ConfigurePools(int enemiesCount)
        {
            _bulletPool.Construct(_bulletConfig.bulletCountsInPool, ()=> _bulletFactory.Create(Vector3.zero, _globalParent));
            _enemyPool.Construct(enemiesCount, ()=> _enemyFactory.Create(Vector3.zero, _globalParent));
        }
    }
}