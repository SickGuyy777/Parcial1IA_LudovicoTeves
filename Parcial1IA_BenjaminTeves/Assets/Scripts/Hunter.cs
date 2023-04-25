using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunter : MonoBehaviour
{
    FiniteStateMachine _fsm;

    public float stamina;

    Vector3 _velocity;
    int _currentWay;

    [SerializeField] float _maxSpeed;
    [SerializeField] [Range(0, 0.15f)] float _maxForce;

    public float wayRadius;
    public Transform[] waypoints;

    private void Start()
    {
        _fsm = new FiniteStateMachine();
        _fsm.AddState(HunterStates.Idle, new IdleState(this));
        _fsm.AddState(HunterStates.Patrol, new PatrolState(this));

        _fsm.ChangeState(HunterStates.Idle);
    }

    private void Update()
    {
        _fsm.Update();
    }

    public void FollowWays()
    {
        AddForce(Seek(waypoints[_currentWay].position));

        if (Vector3.Distance(waypoints[_currentWay].position, transform.position) <= wayRadius)
            _currentWay++;

        if (_currentWay >= waypoints.Length)
            _currentWay = 0;

        transform.position += _velocity * Time.deltaTime;
        transform.right = _velocity;
    }

    Vector3 Seek(Vector3 targetPos)
    {
        Vector3 desired = (targetPos - transform.position).normalized * _maxSpeed;
        Vector3 steering = Vector3.ClampMagnitude(desired - _velocity, _maxForce);
        return steering;
    }

    void AddForce(Vector3 force)
    {
        _velocity = Vector3.ClampMagnitude(_velocity + force, _maxSpeed);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(waypoints[_currentWay].position, wayRadius);
    }
}

public enum HunterStates
{
    Idle,
    Patrol,
    Chase
}
