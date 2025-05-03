using System;
using System.Collections.Generic;
using Infrastructure.DI.Container;

namespace Infrastructure.StateMachine
{
    public class GameStateMachine
    {
        private Dictionary<Type, IState> _states;
        private IState _activeState;
        public GameStateMachine(Container container, GameManager gameManager)
        {
            _states = new Dictionary<Type, IState>
            {
                [typeof(InitState)] = new InitState(this, container, gameManager),
                [typeof(OnStartState)] = new OnStartState(this, container, gameManager),
                [typeof(GameLoopState)] = new GameLoopState(this, gameManager)
            };
        }
    
        public void Enter<TState>() where TState : class, IState
        {
            IState state = ChangeState<TState>();
            state.Enter();
        }
    
    
        private TState ChangeState<TState>() where TState : class, IState
        {
            _activeState?.Exit();
      
            TState state = GetState<TState>();
            _activeState = state;
      
            return state;
        }
    
        private TState GetState<TState>() where TState : class, IState => 
            _states[typeof(TState)] as TState;
    }
}