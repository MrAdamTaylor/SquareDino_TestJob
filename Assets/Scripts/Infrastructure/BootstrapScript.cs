using Infrastructure.DI.Container;
using Infrastructure.DI.Model;
using Unity.VisualScripting;
using UnityEngine;

public class BootstrapScript : MonoBehaviour
{
    [SerializeField] public PlayerConfig _playerConfig;
    [SerializeField] public EnemyConfig _enemyConfig;
    
    public void Start()
    {
        var container = new Container();

        container.RegisterSingleton<AssetLoader, AssetLoader>();
        if (_playerConfig != null)
        {
            container.CacheScriptableObject(typeof(PlayerConfig), _playerConfig);
            container.RegisterSingleton<PlayerFactory, PlayerFactory>();
        }
        if (_enemyConfig != null)
        {
            container.CacheScriptableObject(typeof(EnemyConfig), _enemyConfig);
            container.RegisterSingleton<EnemyFactory, EnemyFactory>();
        }
        container.CreateScope();
        
        GameObject gameObject = new GameObject("[GameAppController]");
        var gameAppController = gameObject.AddComponent<GameAppController>();
        gameAppController.Init(container);
    }
}