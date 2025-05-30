using Core;
using Core.Other;
using Infrastructure.DI.Container;
using Infrastructure.DI.Injector;
using UnityEngine;

namespace Infrastructure.Factory
{
    public class BulletFactory : IFactory
    {
        private const string BULLET_PREFAB_PATH = "Prefabs/Bullet/Bullet";
        
        [Inject] private AssetLoader _assetLoader;
        [Inject] private Container _container;

        public GameObject Create(Vector3 position, Transform parent)
        {
            var prefab = _assetLoader.LoadPrefab(BULLET_PREFAB_PATH);
            if (prefab == null)
                return null;

            var instance = GameObject.Instantiate(prefab, position, Quaternion.identity, parent);
            var bulletComponent = instance.GetComponent<Bullet>();
            _container.Construct(bulletComponent);
            return instance;
        }
    }
}