using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringAgent : MonoBehaviour
{
    protected Vector3 _velocity;

    public float initialSpeed;
    [SerializeField] protected float _maxSpeed;
    [SerializeField] protected float _maxForce;

    protected void Move()
    {
        transform.position += _velocity * Time.deltaTime;
        //Vector3 dir = transform.position;
        transform.forward = _velocity;
    }

    protected void AddForce (Vector3 force)
    {
        _velocity = Vector3.ClampMagnitude(_velocity + force, _maxSpeed);
    }

    protected Vector3 Seek (Vector3 t)
    {
        return CalculateSteering(t - transform.position);
    }

    protected Vector3 CalculateSteering(Vector3 desired)
    {
        return Vector3.ClampMagnitude(desired - _velocity, _maxForce);
    }
}
