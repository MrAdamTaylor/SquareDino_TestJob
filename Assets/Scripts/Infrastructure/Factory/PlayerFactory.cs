using Core.Configs;
using Core.Player;
using Infrastructure.DI.Container;
using Infrastructure.DI.Injector;
using UnityEngine;

namespace Infrastructure.Factory
{
    public class PlayerFactory : IFactory
    {
        private const string PLAYER_PREFAB_PATH = "Prefabs/Player/Player";
    
        [Inject] private PlayerConfig _playerConfig;
        [Inject] private AssetLoader _assetLoader;
        [Inject] private Container _container;
    
        public GameObject Create(Vector3 position, Transform parent)
        {
            var prefab = _assetLoader.LoadPrefab(PLAYER_PREFAB_PATH);
            if (prefab == null)
                return null;

            var instance = GameObject.Instantiate(prefab, position, Quaternion.Euler(0,180,0), parent);
            var playerComponent = instance.GetComponent<Player>();
            MouseInputSystem mouseInputSystem = new();
            _container.Construct(mouseInputSystem);
            _container.CacheType(mouseInputSystem.GetType(), mouseInputSystem);
            _container.Construct(playerComponent);
            return instance;
        }
    }
}