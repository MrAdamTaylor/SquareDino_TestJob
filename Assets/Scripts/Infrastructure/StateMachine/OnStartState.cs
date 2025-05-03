using System;
using Infrastructure.DI.Container;

namespace Infrastructure.StateMachine
{
    public class OnStartState : IState
    {
        private GameManager _gameManager;

        public OnStartState(GameStateMachine gameStateMachine, Container container, GameManager gameManager)
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