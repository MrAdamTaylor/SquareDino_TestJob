using System.Collections.Generic;
using Cinemachine;
using Core.Configs;
using Core.Enemy;
using Core.GameControl;
using Infrastructure.DI.Container;
using Infrastructure.DI.Model;
using Infrastructure.Factory;
using UnityEngine;

namespace Infrastructure.Bootstrap
{
    public class BootstrapScript : MonoBehaviour
    {
        [SerializeField] private PlayerConfig _playerConfig;
        [SerializeField] private EnemyConfig _enemyConfig;
        [SerializeField] private BulletConfig _bulletConfig;
        [SerializeField] private CinemachineFreeLook _playerFollowCamera;
        [SerializeField] private Camera _camera;
        [SerializeField] private List<Transform> _waypoints;
    
        [Header("StartPoint")]
        [SerializeField] private Transform _startPoint;
        [Header("FinishPoint")]
        [SerializeField] private LevelReloaderTrigger _levelReloaderTrigger;
        
        [SerializeField] List<EdgeConfig> _edges;
        
        public void Start()
        {
            var container = new Container();
        
            container.CacheType(container.GetType(), container);
            container.CacheType(_waypoints.GetType(), _waypoints);
            container.CacheComponent(_camera.GetType(), _camera);
            container.CacheMono(_playerFollowCamera.GetType(),_playerFollowCamera);
            container.RegisterSingleton<AssetLoader, AssetLoader>();
        
            container.RegisterTransient<Health,Health>();
            container.RegisterTransient<RagdollHandler, RagdollHandler>();
        
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
            if (_bulletConfig != null)
            {
                container.CacheScriptableObject(typeof(BulletConfig), _bulletConfig);
                container.RegisterSingleton<BulletFactory, BulletFactory>();
            }
            container.CreateScope();
        
            container.CacheType(_edges.GetType(), _edges);
            GameAppController gameAppController = new();
            gameAppController.Init(container, _startPoint, _levelReloaderTrigger);
        }
        
        
    }
}