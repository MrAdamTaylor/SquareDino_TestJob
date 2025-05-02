using Infrastructure.DI.Container;
using Infrastructure.DI.Injector;
using UnityEngine;

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

        var instance = GameObject.Instantiate(prefab, position, Quaternion.identity, parent);
        var playerComponent = instance.GetComponent<Player>();
        _container.Construct(playerComponent);
        return instance;
    }
}