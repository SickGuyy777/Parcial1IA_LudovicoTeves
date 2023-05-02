using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    
    private Vector3 _velocity;
    public float maxSpeed;
    [Range(0f, 0.5f)]
    public float maxForce;

    public float viewRadius;

    //Weights
    [Range(0f, 2f)]
    public float separationWeight;
    [Range(0f, 2f)]
    public float alignmentWeight;
    [Range(0f, 2f)]
    public float cohesionWeight;

    [Header("Arrive")]
    public float arriveRadius;
    public bool isArriving;

    public Transform seekTarget;

    public Transform fleeTarget;


    
    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.AddBoid(this);

        Vector3 randomDir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        randomDir.Normalize();
        randomDir *= maxSpeed;
        AddForce(randomDir);
    }

    // Update is called once per frame
    void Update()
    {
        AddForce(Separation() * separationWeight);
        AddForce(Alignment() * alignmentWeight);
        AddForce(Cohesion() * cohesionWeight);

        transform.position += _velocity * Time.deltaTime;
        transform.right = _velocity;
        CheckBounds();

       if (isArriving)
        {
            AddForce(Arrive(seekTarget.position));
            transform.position += _velocity * Time.deltaTime;
            transform.right = _velocity;
            return;
        }

        Vector3 desired = seekTarget.position - transform.position;
        float dist = desired.magnitude;
        if (dist <= viewRadius)
        {
            AddForce(Seek(seekTarget.position));
        }

        if ((fleeTarget.position - transform.position).magnitude <= viewRadius)
        {
         AddForce(Flee(fleeTarget.position));
        }
    }

    Vector3 Separation()
    {
        Vector3 desired = Vector3.zero;
        foreach (Boid boid in GameManager.instance.allBoids)
        {
            Vector3 dist = boid.transform.position - transform.position;
            if (dist.magnitude <= viewRadius)
            {
                desired += dist;
            }
        }
        if (desired == Vector3.zero) return desired; //si no hay nadie adentro
        desired *= -1;
        return CalculateSteering(desired);
    }

    Vector3 Alignment()
    {
        Vector3 desired = Vector3.zero;
        int count = 0;
        foreach (var item in GameManager.instance.allBoids)
        {
            if (item == this) continue;
            if (Vector3.Distance(transform.position, item.transform.position) <= viewRadius)
            {
                desired += item._velocity;
                count++;
            }
        }
        if (count == 0) return desired;
        desired /= (float)count;
        return CalculateSteering(desired);
    }

    Vector3 Cohesion()
    {
        Vector3 desired = Vector3.zero;
        int count = 0;
        foreach (var item in GameManager.instance.allBoids)
        {
            if (item == this) continue;

            if (Vector3.Distance(transform.position, item.transform.position) <= viewRadius)
            {
                desired += item.transform.position;
                count++;
            }
        }
        if (count == 0) return desired;

        desired /= (float)count;
        desired -= transform.position;

        return CalculateSteering(desired);
    }

    Vector3 CalculateSteering(Vector3 desired)
    {
        return Vector3.ClampMagnitude((desired.normalized * maxSpeed) - _velocity, maxForce);
    }

    void CheckBounds()
    {
        transform.position = GameManager.instance.ChangeObjPosition(transform.position);
    }

    protected void AddForce(Vector3 force)
    {
        _velocity = Vector3.ClampMagnitude(_velocity + force, maxSpeed);
    }

    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, viewRadius);
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.position, separationRadius);
        
    }

    public Vector3 GetVelocity()
    {
        return _velocity;
    }

    Vector3 Arrive(Vector3 targetPos)
    {
        Vector3 desired = targetPos - transform.position;
        float dist = desired.magnitude;
        if (dist <= arriveRadius)
        {
            desired.Normalize();
            desired *= maxSpeed * (dist / arriveRadius);

            GameObject.Destroy(seekTarget.gameObject);

            isArriving = false;

        }
        else
        {
            desired.Normalize();
            desired *= maxSpeed;
        }

        //Steering
        Vector3 steering = desired - _velocity;
        steering = Vector3.ClampMagnitude(steering, maxForce);

        return steering;
    }

    Vector3 Seek(Vector3 targetPos)
    {
        //Action-Selection
        Vector3 desired = targetPos - transform.position;
        desired.Normalize();
        desired *= maxSpeed;


        float dist = desired.magnitude;
        if (dist <= viewRadius){
            isArriving = true;
        }

        //Steering
        Vector3 steering = desired - _velocity;

        steering = Vector3.ClampMagnitude(steering, maxForce);

        return steering;
    }

    Vector3 Flee(Vector3 targetPos)
    {
        return -Seek(targetPos);
    }


}
