using System;
using Core.GameControl;
using Infrastructure.Bootstrap;
using Infrastructure.DI.Container;

namespace Infrastructure.StateMachine
{
    public class OnStartState : IState
    {
        private GameManager _gameManager;

        public OnStartState( GameManager gameManager)
        {
            _gameManager = gameManager;
            
        }
    
        public void Enter()
        {
            _gameManager.StartConfigure();
        }

        public void Exit()
        {
            
        }
    }
}