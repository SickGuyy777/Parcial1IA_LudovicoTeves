using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : States
{
    Hunter _hunter;

    public IdleState(Hunter hunter)
    {
        _hunter = hunter;
    }

    public override void OnEnter()
    {
        Debug.Log("Entro a Idle");
    }

    public override void Update()
    {
        _hunter.stamina += 1 * Time.deltaTime;

        if (_hunter.stamina >= 60)
            fsm.ChangeState(HunterStates.Patrol);

    }

    public override void OnExit()
    {

    }
}