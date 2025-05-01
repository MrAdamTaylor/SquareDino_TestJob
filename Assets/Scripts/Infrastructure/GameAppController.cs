using System.Collections;
using Infrastructure.DI.Container;
using UnityEngine;

public class GameAppController : MonoBehaviour
{
    private GameStateMachine _gameStateMachine;
    public void Init(Container container)
    {
        _gameStateMachine = new GameStateMachine(container);
        _gameStateMachine.Enter<InitState>();
        
    }
}


public interface IState
{
    void Enter();
    void Exit();
}