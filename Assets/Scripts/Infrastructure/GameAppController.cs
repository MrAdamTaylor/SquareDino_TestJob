using Infrastructure.DI.Container;
using Infrastructure.StateMachine;
using UnityEngine;

namespace Infrastructure
{
    public class GameAppController : MonoBehaviour
    {
        private GameStateMachine _gameStateMachine;
        public void Init(Container container, Transform startPoint, LevelReloaderTrigger levelReloaderTrigger)
        {
            GameManager gameManager = new GameManager(startPoint, levelReloaderTrigger);
            _gameStateMachine = new GameStateMachine(container, gameManager);
            _gameStateMachine.Enter<InitState>();
        
        }
    }


    public interface IState
    {
        void Enter();
        void Exit();
    }
}