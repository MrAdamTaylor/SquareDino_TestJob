using Core.Enemy;
using Core.GameControl;
using Core.ObjectPool;
using Infrastructure.Bootstrap;
using Infrastructure.DI.Container;
using Infrastructure.Factory;
using UnityEngine;

namespace Infrastructure.StateMachine
{
    public class InitState : IState
    {
        private const string PARENT_OBJECT_NAME = "[GameObjects]";
        
        private readonly IFactory _enemyFactory;
        private readonly IFactory _playerFactory;
        private readonly IFactory _bulletFactory;
        private readonly Container _container;
        private readonly GameManager _gameManager;
        private readonly GameStateMachine _gameStateMachine;
    
        public InitState(GameStateMachine gameStateMachine, Container container, GameManager gameManager)
        {
            _gameStateMachine = gameStateMachine;
            _gameManager = gameManager;
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

            GameObject parent = GameObject.Find(PARENT_OBJECT_NAME);
            
            _playerFactory.Create(_gameManager.StartPoint.position, parent.transform);
            
            EnemyManager enemyManager = new EnemyManager();
            _container.CacheType(enemyManager.GetType(),enemyManager);
            
            
            PoolBuilder poolBuilder = new PoolBuilder((BulletFactory)_bulletFactory,(EnemyFactory)_enemyFactory, parent.transform);
            _container.Construct(poolBuilder);
            int enemyCount = creator.CalculateOptimalEnemyPool();
            poolBuilder.ConfigurePools(enemyCount);
        
            _container.Construct(enemyManager);
            
            _container.Construct(_gameManager);
            
            _gameStateMachine.Enter<OnStartState>();
        }

        public void Exit()
        {
        
        }
    }
}