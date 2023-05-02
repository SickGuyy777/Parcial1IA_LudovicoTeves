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
        Debug.Log("Entro a Idle");
    }

    public override void Update()
    {
        _hunter.AddForce(ChargeStamine());
        if (_hunter.CHECKAGENT == true && _hunter.stamine > 0 && _hunter.fullStamine == true) fsm.ChangeState(HunterStates.Chase);
    }

    public Vector3 ChargeStamine()
    {
        _hunter.stamine += 1f * Time.deltaTime;
        Vector3 desired = default;
        if(_hunter.stamine >= 60)
        {
            _hunter.fullStamine = true;
            if (_hunter.stamine > 0 && _hunter.fullStamine == true && _hunter.CHECKAGENT == false) fsm.ChangeState(HunterStates.Patrol);
        }
        return _hunter.Seek(desired);
    }

    public override void OnExit()
    { }
}
