using Core.GameControl;
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
            _gameStateMachine = new GameStateMachine(container, startPoint, levelReloaderTrigger);
            _gameStateMachine.Enter<InitState>();
        
        }
    }
}