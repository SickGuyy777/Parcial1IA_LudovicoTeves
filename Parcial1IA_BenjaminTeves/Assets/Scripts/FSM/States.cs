using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class States
{
    public FiniteStateMachine fsm;
    public abstract void Update();
    public abstract void OnEnter();
    public abstract void OnExit();
}
