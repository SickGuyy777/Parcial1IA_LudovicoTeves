using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunter : MonoBehaviour
{
    FiniteStateMachine _fsm;

    public float stamine;
    public float fullStamine;
    public float wayRadius;
    public float viewRadius;
    public Transform[] waypoints;

    [HideInInspector] public int currentWay;

    public static Vector3 _velocity;
    bool _checkAgent;

    [SerializeField] float _maxSpeed;
    [SerializeField][Range(0, 0.15f)] float _maxForce;
    [SerializeField] LayerMask _agentLayer;
    [SerializeField] LayerMask _obstaclesLayer;

    public bool CHECKAGENT { get => _checkAgent; }
    public float MAXSPEED { get => _maxSpeed; }
    public Vector3 VELOCITY { get => _velocity; }

    private void Start()
    {
        _fsm = new FiniteStateMachine();
        _fsm.AddState(HunterStates.Idle, new IdleState(this));
        _fsm.AddState(HunterStates.Patrol, new PatrolState(this));
        _fsm.AddState(HunterStates.Chase, new ChaseState(this));

        _fsm.ChangeState(HunterStates.Idle);

        stamine = fullStamine;
    }

    private void Update()
    {
        _checkAgent = Physics.CheckSphere(transform.position, viewRadius, _agentLayer);
        _fsm.Update();
    }

    //public Vector3 ObstacleAvoidance()
    //{
    //    Vector3 desired = _velocity.normalized * _maxSpeed;
    //
    //    if (Physics.Raycast(transform.position + transform.right * 0.5f, transform.forward, out RaycastHit hit, viewRadius, _obstaclesLayer))
    //    {
    //        Vector3 avoidanceDir = Vector3.Cross(transform.up, hit.normal);
    //        desired += avoidanceDir * _maxSpeed;
    //    }
    //
    //    else if (Physics.Raycast(transform.position - transform.right * 0.5f, transform.forward, out hit, viewRadius, _obstaclesLayer))
    //    {
    //        Vector3 avoidanceDir = Vector3.Cross(transform.up, hit.normal);
    //        desired -= avoidanceDir * _maxSpeed;
    //    }
    //
    //    Vector3 steering = desired - _velocity;
    //    steering = Vector3.ClampMagnitude(steering, _maxForce);
    //    AddForce(steering);
    //
    //    return steering;
    //}

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

    public Vector3 MyVelocity()
    {
        return _velocity;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(waypoints[currentWay].position, wayRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Gizmos.color = Color.yellow;
        Vector3 origin1 = transform.position + transform.forward / 2;
        Vector3 origin2 = transform.position - transform.forward / 2;

        Gizmos.DrawLine(origin1, origin1 + transform.right * viewRadius);
        Gizmos.DrawLine(origin2, origin2 + transform.right * viewRadius);
    }
}

public enum HunterStates
{
    Idle,
    Patrol,
    Chase
}
