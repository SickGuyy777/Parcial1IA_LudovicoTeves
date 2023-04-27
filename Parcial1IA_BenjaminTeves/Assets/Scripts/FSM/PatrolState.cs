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
        Patrol();
        
        if(_hunter.stamine <= 0)
        {
            _hunter.fullStamine = false;
            fsm.ChangeState(HunterStates.Idle);
        }

        if (_hunter.CHECKAGENT == true && _hunter.stamine >= 0 && _hunter.fullStamine == true) fsm.ChangeState(HunterStates.Chase);
    }

    public void Patrol()
    {
        _hunter.AddForce(_hunter.Seek(_hunter.waypoints[_hunter.currentWay].position));

        if (Vector3.Distance(_hunter.waypoints[_hunter.currentWay].position, _hunter.transform.position) <= _hunter.wayRadius)
            _hunter.currentWay++;

        if (_hunter.currentWay >= _hunter.waypoints.Length)
            _hunter.currentWay = 0;

        _hunter.transform.position += _hunter.VELOCITY * Time.deltaTime;
        _hunter.transform.right = _hunter.VELOCITY;
        _hunter.stamine -= 1 * Time.deltaTime;
    }

    public override void OnExit()
    { }
}
