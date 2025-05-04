using System;
using Core.GameControl;
using Infrastructure.Bootstrap;
using UnityEngine;

namespace Infrastructure.StateMachine
{
    public class GameLoopState : IState
    {
        private readonly GameManager _gameManager;
        
        public GameLoopState(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public void Enter()
        {
            _gameManager.GameStart();
        }

        public void Exit()
        {
            
        }
    }
}