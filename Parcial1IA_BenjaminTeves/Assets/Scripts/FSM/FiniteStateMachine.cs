using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachine
{
    States _currentState;

    Dictionary<HunterStates, States> _allStates = new Dictionary<HunterStates, States>();

    public void AddState(HunterStates key, States state)
    {
        if (state == null) return;

        _allStates.Add(key, state);
        state.fsm = this;
    }

    public void ChangeState(HunterStates key)
    {
        if (!_allStates.ContainsKey(key)) return;

        if(_currentState != null)
            _currentState.OnExit();

        _currentState = _allStates[key];
        _currentState.OnEnter();
    }

    public void Update()
    {
        _currentState.Update();
    }
}
