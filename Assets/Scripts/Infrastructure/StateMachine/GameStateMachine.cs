using System;
using System.Collections.Generic;
using Core.GameControl;
using Infrastructure.Bootstrap;
using Infrastructure.DI.Container;
using UnityEngine;

namespace Infrastructure.StateMachine
{
    public class GameStateMachine
    {
        private Dictionary<Type, IState> _states;
        private IState _activeState;
        public GameStateMachine(Container container, Transform startPoint, LevelReloaderTrigger levelReloaderTrigger)
        {
            GameManager gameManager = new GameManager(this, startPoint, levelReloaderTrigger);
            
            _states = new Dictionary<Type, IState>
            {
                [typeof(InitState)] = new InitState(this, container, gameManager,startPoint, levelReloaderTrigger.gameObject.transform),
                [typeof(OnStartState)] = new OnStartState( gameManager),
                [typeof(GameLoopState)] = new GameLoopState( gameManager)
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