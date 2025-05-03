using System.Collections.Generic;
using System.Linq;
using Core.Enemy;
using Core.ObjectPool;
using Core.Player;
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
        private GameManager _gameManager;
        private GameStateMachine _gameStateMachine;
    
        public InitState(GameStateMachine gameStateMachine, Container container, GameManager gameManager)
        {
            _gameStateMachine = gameStateMachine;
            _gameManager = gameManager;
            _gameManager.Construct(_gameStateMachine);
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
            container.CacheType(_gameManager.GetType(), _gameManager);
        }

        public void Enter()
        {
            GameTaskCreator creator = new();
            _container.Construct(creator);
            creator.GenerateGameTask();

            GameObject parent = GameObject.Find("[GameObjects]");
        
            GameObject startPoint = GameObject.Find("StartWaypoint");
            _playerFactory.Create(startPoint.transform.position, parent.transform);
            
            EnemyManager enemyManager = new EnemyManager();
            _container.CacheType(enemyManager.GetType(),enemyManager);
            
            
            PoolBuilder poolBuilder = new PoolBuilder((BulletFactory)_bulletFactory,(EnemyFactory)_enemyFactory, parent.transform);
            _container.Construct(poolBuilder);
            int enemyCount = creator.CalculateOptimalEnemyPool();
            poolBuilder.ConfigurePools(enemyCount);
        
            _container.Construct(enemyManager);
            
            _container.Construct(_gameManager);
            
            _gameStateMachine.Enter<OnStartState>();
            /*GameObject enemiesSpawnPointParent = GameObject.Find("[EnemiesSpawnPoints]");
        
            var spawnPoints = new List<Transform>();
            if (enemiesSpawnPointParent != null)
                spawnPoints = enemiesSpawnPointParent.transform.GetAllChildren();
        
            for (int i = 0; i < spawnPoints.Count; i++)
            {
                _enemyFactory.Create(spawnPoints[i].position, parent.transform);
            }*/
        }

        public void Exit()
        {
        
        }
    }


    public class EnemyManager
    {
        [Inject] private EnemyPool _enemyPool;

        public IReadOnlyList<Enemy> ActiveEnemies => _activeEnemies;
        
        private List<GameObject> _ragdollEnemiesList = new();
        private List<Enemy> _activeEnemies = new();
        private List<GameObject> _oldRagdollEnemiesList = new();

        public void SpawnEnemies(Transform[] enemiesPoints)
        {
            var enemies = _enemyPool.SpawnAtPositions(enemiesPoints.Length, enemiesPoints);
            
            for (int i = 0; i < enemies.Count; i++)
            {
                SubscribeToDeath(enemies[i]);
            }
        }

        public void OnTaskCompleted(int stepNumber)
        {
            if (stepNumber < 2)
            {
                Debug.Log("<color=orange>Just collecting ragdolls</color>");
                return; // Ничего не делаем
            }

            Debug.Log("<color=orange>Rotating ragdolls, cleaning up</color>");

            // 1. Деспавним старых
            for (int i = 0; i < _oldRagdollEnemiesList.Count; i++)
            {
                ReturnEnemyInPool(_oldRagdollEnemiesList[i]);
            }
            _oldRagdollEnemiesList.Clear();

            // 2. Переносим новых в старые
            _oldRagdollEnemiesList = new List<GameObject>(_ragdollEnemiesList);

            // 3. Очищаем текущий
            _ragdollEnemiesList.Clear();
        }

        private void ReturnEnemyInPool(GameObject enemyObject)
        {
            if (enemyObject.TryGetComponent(out Enemy enemy))
            {
                enemy.OnDeath -= HandleEnemyDeath;
                enemy.Recovery();
            }

            _enemyPool.Return(enemyObject);
        }

        public void DespawnKilledEnemies()
        {
            if(_ragdollEnemiesList.Count <= 0)
                return;
            
            
            for (int i = 0; i < _ragdollEnemiesList.Count; i++)
            {
                ReturnEnemyInPool(_ragdollEnemiesList[i]);
                
                _enemyPool.Return(_ragdollEnemiesList[i]);
            }
        }

        private void SubscribeToDeath(GameObject enemyGO)
        {
            if (enemyGO.TryGetComponent(out Enemy enemy))
            {
                enemy.OnDeath += HandleEnemyDeath;
                _activeEnemies.Add(enemy);
            }

        }
        
        private void HandleEnemyDeath(Enemy enemy)
        {
            _ragdollEnemiesList.Add(enemy.gameObject);
            _activeEnemies.Remove(enemy);
        }

    }

    public class GameManager
    {
        private int _completedSteps = 0;
        
        [Inject] private EnemyManager _enemyManager;
        [Inject] private List<(Transform,GameTask)> _gameTasks;
        [Inject] private MouseInputSystem _mouseInputSystem;
        [Inject] private Player _player;
        
        private GameStateMachine _gameStateMachine;
        private Queue<Transform> _waypoints;
        private Queue<GameTask> _tasks;
        private GameTask _currentTask;


        public void Construct(GameStateMachine gameStateMachine)
        {
            _gameStateMachine = gameStateMachine;
        }

        public void GameStart()
        {
            _tasks = new Queue<GameTask>(_gameTasks.Select(t => t.Item2));
            _waypoints = new Queue<Transform>(_gameTasks.Select(t => t.Item1));
            
            AssignNextTask();
        }

        
        private void AssignNextTask()
        {
            if (_tasks.Count == 0)
            {
                Debug.Log("<color=green>All tasks completed</color>");
                return;
            }

            _currentTask = _tasks.Dequeue();
            _currentTask.OnCompleted += NextStep;
            _enemyManager.SpawnEnemies(_currentTask.GetPositions());

            Transform waypoint = _waypoints.Dequeue();
            _player.TryMoveToNextWaypoint(waypoint);

            var spawnPoints = _currentTask.GetPositions();
            
            var activeEnemies = _enemyManager.ActiveEnemies;
            for (int i = 0; i < activeEnemies.Count; i++)
            {
                _currentTask.AttachEnemy(activeEnemies[i]);
            }

            Debug.Log("<color=cyan>Task assigned</color>");
        }
        
        
        private void NextStep()
        {
            _currentTask.OnCompleted -= NextStep;

            _completedSteps++;
            
            if(_gameTasks.Count - _tasks.Count > 1)
                _enemyManager.OnTaskCompleted(_completedSteps);
            AssignNextTask();
            
        }

        public void StartConfigure()
        {
            _completedSteps = 0;
            _enemyManager.DespawnKilledEnemies();
            //_enemyManager.SpawnEnemies(_gameTasks[0].Item2.GetPositions());

            _mouseInputSystem.Enable();
            _mouseInputSystem.StartConfigure();
            _mouseInputSystem.OnFirstClick += EnterGameLoop;
        }

        private void EnterGameLoop()
        {
            _mouseInputSystem.OnFirstClick -= EnterGameLoop;
            _gameStateMachine.Enter<GameLoopState>();
        }

    }
}