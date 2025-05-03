using Core.GameControll;
using Infrastructure.DI.Container;
using Infrastructure.StateMachine;
using UnityEngine;

namespace Infrastructure.Bootstrap
{
    public class GameAppController 
    {
        private GameStateMachine _gameStateMachine;
        public void Init(Container container, Transform startPoint, LevelReloaderTrigger levelReloaderTrigger)
        {
            GameManager gameManager = new GameManager(startPoint, levelReloaderTrigger);
            _gameStateMachine = new GameStateMachine(container, gameManager);
            _gameStateMachine.Enter<InitState>();
        
        }
    }
}