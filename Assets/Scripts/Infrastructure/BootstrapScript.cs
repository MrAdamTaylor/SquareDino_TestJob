using System.Collections.Generic;
using Cinemachine;
using Infrastructure.DI.Container;
using Infrastructure.DI.Model;
using UnityEngine;

public class BootstrapScript : MonoBehaviour
{
    [SerializeField] private PlayerConfig _playerConfig;
    [SerializeField] private EnemyConfig _enemyConfig;
    [SerializeField] private CinemachineFreeLook _virtualCamera;

    [SerializeField] private List<Transform> _waypoints;
    
    public void Start()
    {
        var container = new Container();
        
        container.CacheType(container.GetType(), container);
        container.CacheType(_waypoints.GetType(), _waypoints);
        container.CacheMono(_virtualCamera.GetType(),_virtualCamera);
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