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
    [SerializeField] LayerMask _obstaclesLayer;

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

    public Vector3 ObstacleAvoidance()
    {
        Vector3 desired = default;

        if (Physics.Raycast(transform.position + transform.right / 2, _velocity, viewRadius, _obstaclesLayer))
        {
            Debug.Log("A");
            desired = -transform.right;
        }
        else if(Physics.Raycast(transform.position - transform.right / 2, _velocity, viewRadius, _obstaclesLayer))
        {
            Debug.Log("B");
            desired = transform.right;
        }
        else return desired;

        return Seek(desired.normalized * _maxSpeed);
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
