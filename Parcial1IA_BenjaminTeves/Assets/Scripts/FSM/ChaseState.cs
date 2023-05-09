using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ChaseState : States
{
    Hunter _hunter;
    Renderer _rend;

    public ChaseState(Hunter hunter)
    {
        _hunter = hunter;
        _rend = _hunter.GetComponent<Renderer>();
    }

    public override void OnEnter()
    {
        _rend.material.color = Color.red;
    }

    public override void Update()
    {
        if (_hunter.stamine <= 0)
            fsm.ChangeState(HunterStates.Idle);

        if (_hunter.CHECKAGENT == false && _hunter.stamine > 0)
            fsm.ChangeState(HunterStates.Patrol);
        _hunter.AddForce(Persuit());
    }

    Vector3 Persuit()
    {
        Vector3 dir = _hunter.transform.position;
        dir.y = 1.124f;
        Vector3 desired = Vector3.zero;
        desired.Normalize();
        desired *= _hunter.MAXSPEED;

        _hunter.stamine -= 1 * Time.deltaTime;
        foreach (var agents in BoidManager.Instance.allBoids)
        {
            Vector3 distBoids = agents.transform.position - _hunter.transform.position;
            if (distBoids.magnitude <= _hunter.viewRadius)
            {
                Vector3 futurePos = agents.transform.position + agents.MyVelocity() * Time.deltaTime;
                desired = futurePos - _hunter.transform.position;
                _hunter.transform.position += desired * Time.deltaTime;
            }
        }

        return _hunter.Seek(desired);
    }

    public override void OnExit()
    { }
}
