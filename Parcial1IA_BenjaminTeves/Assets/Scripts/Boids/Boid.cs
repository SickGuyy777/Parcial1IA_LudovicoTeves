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

    public float separationRadius;

    [Header("Weights")]
    [Range(0f, 3f)] public float separationWeight;
    [Range(0f, 3f)] public float alignmentWeight;
    [Range(0f, 3f)] public float cohesionWeight;

    [Header("Arrive")]
    public float arriveRadius;


    public Transform seekTarget;

    public Transform fleeTarget;

    public float killDistance = 1f;

    private Vector3 velocidad;

    public FoodSpawner foodManager;
    public List<GameObject> foodList;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.AddBoid(this);

        foodList = foodManager.foodList;

        Vector3 randomDir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        randomDir.Normalize();
        randomDir *= maxSpeed;
        AddForce(randomDir);
    }

    // Update is called once per frame
    void Update()
    {
        float distanciaHunter = Vector3.Distance(transform.position, fleeTarget.position);

        if (distanciaHunter <= killDistance)
        {
            gameObject.SetActive(false);
        }

        GameObject closestFood = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject food in foodList)
        {
            float distance = Vector3.Distance(transform.position, food.transform.position);
            if (distance <= viewRadius && distance < closestDistance)
            {
                closestFood = food;
                closestDistance = distance;
            }
        }

        if (closestFood != null)
        {
            AddForce(Arrive(closestFood.transform.position));
        }

        if (closestFood != null && Vector3.Distance(transform.position, closestFood.transform.position) < 0.5)
        {
            foodList.Remove(closestFood);
            Destroy(closestFood);
        }

        transform.position = new Vector3(transform.position.x, 0f, transform.position.z);


        //Lo ideal seria utilizar OverlapSphere y conseguir los componentes
        AddForce(Separation(GameManager.instance.allBoids, separationRadius) * separationWeight);
        AddForce(Alignment() * alignmentWeight);
        AddForce(Cohesion() * cohesionWeight);

        transform.position += _velocity * Time.deltaTime;
        transform.right = _velocity;
        CheckBounds();

        if ((fleeTarget.position - transform.position).magnitude <= viewRadius)
        {
            AddForce(Flee(fleeTarget.position));
        }
    }
    Vector3 Separation(List<Boid> boids)
    {
        return Separation(boids, viewRadius);
    }

    Vector3 Separation(List<Boid> boids, float radius)
    {
        Vector3 desired = Vector3.zero;
        foreach (var item in boids)
        {
            if (item == this || Vector3.Distance(item.transform.position, transform.position) > radius) continue;

            desired += item.transform.position - transform.position;
        }
        if (desired == Vector3.zero) return desired;
        desired = -desired.normalized * maxSpeed;

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
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, separationWeight);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, alignmentWeight);
    }

    public Vector3 GetVelocity()
    {
        return _velocity;
    }

    Vector3 Arrive(Vector3 targetPos)
    {
        float dist = Vector3.Distance(targetPos, transform.position);
        if (dist > arriveRadius)
            return Seek(targetPos);

        Vector3 desired = (targetPos - transform.position).normalized;
        desired *= ((dist / arriveRadius) * maxSpeed); //arriveRadius = 100% / dist

        Vector3 steering = Vector3.ClampMagnitude(desired - _velocity, maxForce);

        return steering;
    }

    Vector3 Seek(Vector3 targetPos)
    {
        //Action-Selection
        Vector3 desired = targetPos - transform.position;
        desired.Normalize();
        desired *= maxSpeed;


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
