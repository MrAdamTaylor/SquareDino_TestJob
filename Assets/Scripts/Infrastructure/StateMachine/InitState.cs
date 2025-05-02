using System.Collections.Generic;
using Core;
using Core.Configs;
using Core.ObjectPool;
using Extensions;
using Infrastructure.DI.Container;
using Infrastructure.DI.Injector;
using Infrastructure.Factory;
using UnityEngine;

namespace Infrastructure.StateMachine
{
    public class InitState : IState
    {
        private IFactory _enemyFactory;
        private IFactory _playerFactory;
        private IFactory _bulletFactory;
        private Container _container;
    
        public InitState(GameStateMachine gameStateMachine, Container container)
        {
            _container = container;
            var scope = container.CreateScope();
            _enemyFactory = (IFactory)scope.Resolve(typeof(EnemyFactory));
            _playerFactory = (IFactory)scope.Resolve(typeof(PlayerFactory));
            _bulletFactory = (IFactory)scope.Resolve(typeof(BulletFactory));
            container.Construct(_enemyFactory);
            container.Construct(_playerFactory);
            var bulletPool = new BulletPool();
            var enemyPool = new EnemyPool();
            container.Construct(_bulletFactory);
            container.CacheType(bulletPool.GetType(), bulletPool);
            container.CacheType(enemyPool.GetType(), enemyPool);
        }

        public void Enter()
        {
            GameTaskCreator creator = new();
            _container.Construct(creator);
            creator.GenerateGameTask();

            GameObject parent = GameObject.Find("[GameObjects]");
        
            PoolBuilder poolBuilder = new PoolBuilder((BulletFactory)_bulletFactory,(EnemyFactory)_enemyFactory, parent.transform);
            _container.Construct(poolBuilder);
            int enemyCount = creator.CalculateOptimalEnemyPool();
            poolBuilder.ConfigurePools(enemyCount);
        
            GameObject enemiesSpawnPointParent = GameObject.Find("[EnemiesSpawnPoints]");
        
            var spawnPoints = new List<Transform>();
            if (enemiesSpawnPointParent != null)
                spawnPoints = enemiesSpawnPointParent.transform.GetAllChildren();

            GameObject startPoint = GameObject.Find("StartWaypoint");

            _playerFactory.Create(startPoint.transform.position, parent.transform);
        
            for (int i = 0; i < spawnPoints.Count; i++)
            {
                _enemyFactory.Create(spawnPoints[i].position, parent.transform);
            }
        }

        public void Exit()
        {
        
        }
    }

    public class PoolBuilder
    {
        [Inject] private BulletPool _bulletPool;
        [Inject] private EnemyPool _enemyPool;
        private BulletFactory _bulletFactory;
        private EnemyFactory _enemyFactory;
        private Transform _globalParent;
        private BulletConfig _bulletConfig;
        private EnemyConfig _enemyConfig;
    
        public PoolBuilder(BulletFactory bulletFactory, EnemyFactory enemyFactory, Transform globalParent)
        {
            _enemyFactory = enemyFactory;
            _bulletFactory = bulletFactory;
            _globalParent = globalParent;
        }

        [Inject]
        public void Construct(BulletConfig bulletConfig, EnemyConfig enemyConfig)
        {
            _bulletConfig = bulletConfig;
            _enemyConfig = enemyConfig;
        }

        public void ConfigurePools(int enemiesCount)
        {
            _bulletPool.Construct(_bulletConfig.bulletsInPool, ()=> _bulletFactory.Create(Vector3.zero, _globalParent));
            _enemyPool.Construct(enemiesCount, ()=> _enemyFactory.Create(Vector3.zero, _globalParent));
        }
    }



    public class GameTaskCreator
    {
        private const int ACTIVE_TASK_COUNT = 3;
    
        private List<Transform> _waypoints = new();
        private Container _container;
        private List<(Transform, GameTask)> _gameTasks = new();
    
        [Inject]
        public void Construct(List<Transform> waypoints, Container container)
        {
            _container = container;
            _waypoints = waypoints;
        }

        public void GenerateGameTask()
        {
            List<(Transform,GameTask)> gameTasks = new();
        
            for (int i = 0; i < _waypoints.Count; i++)
            {
                TagRadiusDetector detector;
                if (!_waypoints[i].gameObject.TryGetComponent(out detector))
                {
                    var spawnPointComponent = _waypoints[i].gameObject.GetComponent<TagRadiusDetector>();
                    UnityEngine.Object.Destroy(spawnPointComponent);
                    continue;
                }
            
                var waypointInfo = detector.Scan();
                Transform cameraLookTransform = null;
                List<Transform> enemySpawnerTransforms = new();

                for (int j = 0; j < waypointInfo.Count; j++)
                {
                    string tag = waypointInfo[j].tag;
                    Transform transform = waypointInfo[j].transform;

                    if (tag == "CameraLook" && cameraLookTransform == null)
                    {
                        cameraLookTransform = transform; 
                    }
                    else if (tag == "EnemySpawner")
                    {
                        enemySpawnerTransforms.Add(transform);
                    }
                }
            
                if (enemySpawnerTransforms.Count > 0)
                {
                    GameTask task = new GameTask(cameraLookTransform, enemySpawnerTransforms);
                
                    _gameTasks.Add((_waypoints[i],task));
                }

                var detectorComponent = _waypoints[i].gameObject.GetComponent<TagRadiusDetector>();
                UnityEngine.Object.Destroy(detectorComponent);
            
            }

            for (int i = 0; i < gameTasks.Count; i++)
            {
                gameTasks[i].Item2.OutputInfo();
            }
        }

        public int CalculateOptimalEnemyPool()
        {
            int taskCount = _gameTasks.Count;
        
            if (taskCount <= ACTIVE_TASK_COUNT)
            {
                int total = 0;
                for (int i = 0; i < taskCount; i++)
                {
                    total += _gameTasks[i].Item2.EnemyCount; 
                }
                return total;
            }
        
            int maxSum = 0;

            for (int start = 0; start <= taskCount - ACTIVE_TASK_COUNT; start++)
            {
                int windowSum = 0;
                for (int offset = 0; offset < ACTIVE_TASK_COUNT; offset++)
                {
                    int index = start + offset;
                    windowSum += _gameTasks[index].Item2.EnemyCount;  
                }

                if (windowSum > maxSum)
                    maxSum = windowSum;
            }

            return maxSum;
        }
    }

    public class GameTask
    {
        private Transform _cameraLookTarget;
        private List<Transform> _enemySpawnPoints;

        public GameTask(Transform cameraLookTarget, List<Transform> enemySpawnPoints)
        {
            _cameraLookTarget = cameraLookTarget;
            _enemySpawnPoints = enemySpawnPoints;
        }

        public void OutputInfo()
        {
            Debug.Log($"<color=magenta>Camera Look Target: {_cameraLookTarget.gameObject.name} and SpawnPoints is {_enemySpawnPoints.Count} </color>");
        }
    
        public int EnemyCount => _enemySpawnPoints.Count;
    }
}