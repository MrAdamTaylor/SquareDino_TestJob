using System.Collections.Generic;
using System.Linq;
using Core.Enemy;
using Core.Player;
using Infrastructure.DI.Injector;
using Infrastructure.StateMachine;
using UnityEngine;

namespace Core.GameControll
{
    public class GameManager
    {
        [Inject] private EnemyManager _enemyManager;
        [Inject] private List<(Transform,GameTask)> _gameTasks;
        [Inject] private MouseInputSystem _mouseInputSystem;
        [Inject] private Player.Player _player;

        public Transform StartPoint { get; }
        
        private bool _isFinished;

        private GameStateMachine _gameStateMachine;
        private Queue<Transform> _waypoints;
        private Queue<GameTask> _tasks;
        private GameTask _currentTask;
        private LevelReloaderTrigger _levelReloaderTrigger;

        public GameManager(Transform startPoint, LevelReloaderTrigger levelReloaderTrigger)
        {
            StartPoint = startPoint;
            _levelReloaderTrigger = levelReloaderTrigger;
            _levelReloaderTrigger.Construct(this);
        }

        public void Construct(GameStateMachine gameStateMachine)
        {
            _gameStateMachine = gameStateMachine;
        }

        public void StartConfigure()
        {
            _enemyManager.ReloadAllEnemies();
            _mouseInputSystem.Enable();
            _mouseInputSystem.StartConfigure();
            _mouseInputSystem.OnFirstClick += EnterGameLoop;
            _player.ConfigureBeforeStart();
            _isFinished = false;
        }

        public void GameStart()
        {
            _tasks = new Queue<GameTask>(_gameTasks.Select(t => t.Item2));
            _waypoints = new Queue<Transform>(_gameTasks.Select(t => t.Item1));
            
            AssignNextTask();
        }

        public void ReloadGame()
        {
            _player.PlayerStop();
            _player.gameObject.transform.position = StartPoint.position;
            for (int i = 0; i < _gameTasks.Count; i++)
            {
                _gameTasks[i].Item2.Reset();
            }

            _gameStateMachine.Enter<OnStartState>();
        }

        private void AssignNextTask()
        {
            if (_tasks.Count == 0)
            {
                _currentTask.OnCompleted -= NextStep;
                _player.TryMoveToNextWaypoint(_levelReloaderTrigger.gameObject.transform);
                return;
            }

            _currentTask = _tasks.Dequeue();
            _currentTask.OnCompleted += NextStep;
            _enemyManager.SpawnEnemies(_currentTask.GetPositions());

            Transform waypoint = _waypoints.Dequeue();
            _player.TryMoveToNextWaypoint(waypoint);
            
            
            var activeEnemies = _enemyManager.ActiveEnemies;
            for (int i = 0; i < activeEnemies.Count; i++)
            {
                _currentTask.AttachEnemy(activeEnemies[i]);
            }
        }
        
        private void NextStep()
        {
            _currentTask.OnCompleted -= NextStep;
            
            if(_tasks.Count == 0)
                _isFinished = true;
            
            if(_gameTasks.Count - _tasks.Count > 1)
                _enemyManager.OnTaskCompleted(_isFinished);
            AssignNextTask();
        }

        private void EnterGameLoop()
        {
            _mouseInputSystem.OnFirstClick -= EnterGameLoop;
            _gameStateMachine.Enter<GameLoopState>();
        }
    }
}