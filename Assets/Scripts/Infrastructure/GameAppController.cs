using Infrastructure.DI.Container;
using Infrastructure.StateMachine;
using UnityEngine;

namespace Infrastructure
{
    public class GameAppController : MonoBehaviour
    {
        private GameStateMachine _gameStateMachine;
        public void Init(Container container)
        {
            GameManager gameManager = new GameManager();
            
            _gameStateMachine = new GameStateMachine(container, new GameManager());
            _gameStateMachine.Enter<InitState>();
        
        }
    }


    public interface IState
    {
        void Enter();
        void Exit();
    }
}