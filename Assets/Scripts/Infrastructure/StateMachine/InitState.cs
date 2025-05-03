using Core.ObjectPool;
using Infrastructure.DI.Container;
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
        }

        public void Exit()
        {
        
        }
    }
}