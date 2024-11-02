using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    public class StateMachine<TStateType, TState, THost> where TState : State<TStateType, THost>
    {
        private TStateType _currentStateType;
        private TState _currentState;

        private Dictionary<TStateType, TState> _states;

        public StateMachine()
        {
            _states = new Dictionary<TStateType, TState>();
        }
        
        public void AddState(TStateType stateType, TState state)
        {
            _states.Add(stateType, state);    
        }
        
        public void ChangeState(TStateType stateType)
        {
            _currentState?.Exit();
            
            _currentStateType = stateType;
            if(_states.TryGetValue(stateType, out var state))
            {
                _currentState = state;
            }
            else
            {
                Debug.LogError($"State of type {stateType} not found");
                return;
            }
            
            _currentState.Enter();
        }

        public void Tick()
        {
            _currentState.Tick();
        }
    }
}