using System;
using UnityEngine;

namespace Infrastructure.StateMachine
{
    public class GameLoopState : IState
    {
        private GameManager _gameManager;
        private GameStateMachine _gameStateMachine;
        
        public GameLoopState(GameStateMachine gameStateMachine, GameManager gameManager)
        {
            _gameManager = gameManager;
            _gameStateMachine = gameStateMachine;
        }

        public void Enter()
        {
            Debug.Log($"<color=green>Game loop entered</color>");
            _gameManager.GameStart();
            //_gameManagerLaunchProcessforPlayer
            
        }

        public void Exit()
        {
            throw new NotImplementedException();
        }
    }
}