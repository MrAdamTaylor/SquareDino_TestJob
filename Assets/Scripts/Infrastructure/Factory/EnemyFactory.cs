using Core.Configs;
using Core.Enemy;
using Infrastructure.DI.Container;
using Infrastructure.DI.Injector;
using UnityEngine;

namespace Infrastructure.Factory
{
    public class EnemyFactory : IFactory
    {
        private const string ENEMY_PREFAB_PATH = "Prefabs/Enemy/Enemy";
    
        [Inject] private EnemyConfig _enemyConfig;
        [Inject] private AssetLoader _assetLoader;
        [Inject] private Container _container;

        public GameObject Create(Vector3 position, Transform parent)
        {
            var prefab = _assetLoader.LoadPrefab(ENEMY_PREFAB_PATH);
            if (prefab == null)
                return null;

            var instance = GameObject.Instantiate(prefab, position, Quaternion.identity, parent);
            var enemyComponent = instance.GetComponent<Enemy>();
            _container.Construct(enemyComponent);
            return instance;
        }
    }
}