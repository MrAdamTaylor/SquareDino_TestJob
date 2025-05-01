using Infrastructure.DI.Injector;
using UnityEngine;

public class EnemyFactory : IFactory
{
    private const string ENEMY_PREFAB_PATH = "Prefabs/Enemy/Enemy";
    
    [Inject] private EnemyConfig _enemyConfig;
    [Inject] private AssetLoader _assetLoader;

    public GameObject Create(Vector3 position, Transform parent)
    {
        var prefab = _assetLoader.LoadPrefab(ENEMY_PREFAB_PATH);
        if (prefab == null)
            return null;

        var instance = GameObject.Instantiate(prefab, position, Quaternion.identity, parent);
        return instance;
    }
}