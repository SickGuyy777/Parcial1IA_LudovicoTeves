using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : States
{
    Hunter _hunter;

    public PatrolState(Hunter hunter)
    {
        _hunter = hunter;
    }

    public override void OnEnter()
    {
        Debug.Log("Entro en Patrol");
    }

    public override void Update()
    {
        _hunter.FollowWays();
        _hunter.stamina -= 1 * Time.deltaTime;

        if(_hunter.stamina <= 0)
            fsm.ChangeState(HunterStates.Idle);
    }

    public override void OnExit()
    {

    }
}
