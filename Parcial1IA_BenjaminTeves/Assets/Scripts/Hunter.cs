using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunter : MonoBehaviour
{
    FiniteStateMachine _fsm;

    public float stamine;
    public float wayRadius;
    public float viewRadius;
    public Transform[] waypoints;
    public bool fullStamine;
    [HideInInspector] public int currentWay;

    Vector3 _velocity;
    bool _checkAgent;

    public bool CHECKAGENT { get => _checkAgent; }
    public Vector3 VELOCITY { get => _velocity; }

    [SerializeField] float _maxSpeed;
    [SerializeField] [Range(0, 0.15f)] float _maxForce;
    [SerializeField] LayerMask _agentLayer;

    private void Start()
    {
        _fsm = new FiniteStateMachine();
        _fsm.AddState(HunterStates.Idle, new IdleState(this));
        _fsm.AddState(HunterStates.Patrol, new PatrolState(this));
        _fsm.AddState(HunterStates.Chase, new ChaseState(this));

        _fsm.ChangeState(HunterStates.Idle);
    }

    private void Update()
    {
        _checkAgent = Physics.CheckSphere(transform.position, viewRadius, _agentLayer);
        _fsm.Update();       
    }

    public Vector3 Seek(Vector3 targetPos)
    {
        Vector3 desired = (targetPos - transform.position).normalized * _maxSpeed;
        Vector3 steering = Vector3.ClampMagnitude(desired - _velocity, _maxForce);
        return steering;
    }

    public void AddForce(Vector3 force)
    {
        _velocity = Vector3.ClampMagnitude(_velocity + force, _maxSpeed);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(waypoints[currentWay].position, wayRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, viewRadius);
    }
}

public enum HunterStates
{
    Idle,
    Patrol,
    Chase
}
