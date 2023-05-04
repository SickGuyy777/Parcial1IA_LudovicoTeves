using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : States
{
    Hunter _hunter;
    Renderer _rend;

    public IdleState(Hunter hunter)
    {
        _hunter = hunter;
        _rend = _hunter.GetComponent<Renderer>();
    }

    public override void OnEnter()
    {
        _rend.material.color = Color.yellow;
    }

    public override void Update()
    {
        _hunter.stamine += 1f * Time.deltaTime;
        if (_hunter.CHECKAGENT == true && _hunter.stamine >= _hunter.fullStamine) fsm.ChangeState(HunterStates.Chase);
        if (_hunter.CHECKAGENT == false && _hunter.stamine >= _hunter.fullStamine) fsm.ChangeState(HunterStates.Patrol);
    }

    public override void OnExit()
    { }
}
