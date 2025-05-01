using System;
using Infrastructure.DI.Container;

public class OnStartState : IState
{
    public OnStartState(GameStateMachine gameStateMachine, Container container)
    {
        
    }
    
    public void Enter()
    {
        throw new NotImplementedException();
    }

    public void Exit()
    {
        throw new NotImplementedException();
    }
}