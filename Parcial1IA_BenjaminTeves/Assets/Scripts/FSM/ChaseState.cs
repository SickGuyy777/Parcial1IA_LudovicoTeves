using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : States
{
    Hunter _hunter;

    public float maxSpeed;
    [Range(0f, 0.1f)]
    public float maxForce;

    public ChaseState (Hunter hunter)
    {
        _hunter = hunter;
    }

    public override void OnEnter()
    {
        Debug.Log("Entro en Chase");
    }

    public override void Update()
    {
        if (_hunter.stamina <= 0)
            fsm.ChangeState(HunterStates.Idle);

        if (_hunter.CHECKAGENT == false && _hunter.stamina > 0)
            fsm.ChangeState(HunterStates.Patrol);

        _hunter.AddForce(Persuit());
    }

    Vector3 Persuit()
    {
        Vector3 dir = _hunter.transform.position;
        dir.y = 0f;
        Vector3 desired = Vector3.zero;
        desired.Normalize();
        desired *= maxSpeed;
        _hunter.stamina -= 1 * Time.deltaTime;
        _hunter.transform.right = desired;

        foreach (var agents in GameManager.instance.allBoids)
        {
            Vector3 distBoids = agents.transform.position - _hunter.transform.position;
            if (distBoids.magnitude <= _hunter.viewRadius)
            {
                Vector3 futurePos = agents.transform.position + agents.GetVelocity() * Time.deltaTime;
                desired = futurePos - _hunter.transform.position;
                _hunter.transform.position += desired * Time.deltaTime;
            }
        }

        return _hunter.Seek(desired);
    }

    public override void OnExit()
    {
    }
}
