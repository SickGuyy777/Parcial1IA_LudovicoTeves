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
        Debug.Log("Entra Chase");
    }

    public override void Update()
    {
        if (_hunter.stamine <= 0)
            fsm.ChangeState(HunterStates.Idle);

        if (_hunter.CHECKAGENT == false && _hunter.stamine > 0)
            fsm.ChangeState(HunterStates.Patrol);
        _hunter.AddForce(PersuitSeek());
        _hunter.transform.position += _hunter.VELOCITY * Time.deltaTime;
        _hunter.transform.right = _hunter.VELOCITY;
    }

    Vector3 PersuitSeek()
    {
        Vector3 dir = _hunter.transform.position;
        dir.y = 1.124f;
        Vector3 desired = Vector3.zero;
        desired *= _hunter.MAXSPEED;
        desired.Normalize();

        _hunter.stamine -= 1 * Time.deltaTime;

        BoidAgent minBoid = null;
        float minDist = 99999;
        foreach (var agents in BoidManager.Instance.allBoids)
        {
            Vector3 distBoids = agents.transform.position - _hunter.transform.position;
            if (distBoids.magnitude <= _hunter.viewRadius && distBoids.magnitude < minDist)
            {
                minDist = distBoids.magnitude;
                minBoid = agents;
            }
        }

        return _hunter.Seek(minBoid.transform.position);
    }

    public override void OnExit()
    { }
}
