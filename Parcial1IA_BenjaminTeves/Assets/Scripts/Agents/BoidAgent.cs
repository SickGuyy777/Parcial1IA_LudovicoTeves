using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoidAgent : SteeringAgent
{
    [SerializeField] SpatialGrid _targetGrid;
    [SerializeField] float _radius;

    public float separationRadius;
    public float viewRadius;

    public float arriveRadius;

    [SerializeField] LayerMask _obsLayer;

    [Range(0f, 10f)][SerializeField] float _separationWeight;

    public List<GameObject> foodList;

    public FoodSpawner foodManager;
    public Transform fleeTarget;

    public float killDistance;
    
    public float respawnTime = 3;

    MeshRenderer _renderer;

    private void Start()
    {
        Vector3 randomDir = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
        AddForce(randomDir.normalized * _maxSpeed);
        BoidManager.Instance.AddBoid(this);
        foodList = foodManager.foodList;
        _renderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        transform.position = BoidManager.Instance.BoundChecks(transform.position);
        //Flocking();
        NewFlocking();

        Vector3 obstacleForce = ObstacleAvoidance();
        Vector3 force = obstacleForce == Vector3.zero ? CalculateSteering(transform.forward * _maxSpeed) : obstacleForce;
        AddForce(force);

        Move();

        float distanciaHunter = Vector3.Distance(transform.position, fleeTarget.position);

        if (distanciaHunter <= killDistance)
        {
            BoidManager.Instance.RemoveBoid(this);
        }

        if (distanciaHunter <= viewRadius) Evade();

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
            AddForce(Arrive(closestFood.transform.position));

        if (closestFood != null && Vector3.Distance(transform.position, closestFood.transform.position) < 0.5)
        {
            foodList.Remove(closestFood);
            Destroy(closestFood);
        }
    }

    void Flocking()
    {
        AddForce(Separation(BoidManager.Instance.allBoids) * _separationWeight);
        AddForce(Alignment(BoidManager.Instance.allBoids));
        AddForce(Cohesion(BoidManager.Instance.allBoids));
    }

    void NewFlocking()
    {
        var entities = Query().ToList();
        AddForce(SeparationNew(entities) * _separationWeight);
        AddForce(AlignmentNew(entities));
        AddForce(CohesionNew(entities));
    }

    Vector3 Arrive(Vector3 targetPos)
    {
        float dist = Vector3.Distance(targetPos, transform.position);
        if (dist > arriveRadius) return Seek(targetPos);

        Vector3 desired = (targetPos - transform.position).normalized;
        desired *= ((dist / arriveRadius) * _maxSpeed);

        Vector3 steering = Vector3.ClampMagnitude(desired - _velocity, _maxForce * 1.5f);
        return steering;
    }

    Vector3 ObstacleAvoidance()
    {
        Vector3 desired = default;

        if (Physics.Raycast(transform.position + transform.right / 4, _velocity, viewRadius, _obsLayer))
            desired = -transform.right;
        else if (Physics.Raycast(transform.position - transform.right / 4, _velocity, viewRadius, _obsLayer))
            desired = transform.right;
        else return desired;

        return CalculateSteering(desired.normalized * _maxSpeed);
    }

    public void Death()
    {
        gameObject.SetActive(false);
        gameObject.transform.position = new Vector3(0, 0, 0);
        _maxSpeed = 0;
        BoidManager.Instance.StartCoroutine(ResetAfterTime(respawnTime));
    }
    
    #region old flocking
    Vector3 Alignment(HashSet<BoidAgent> boids)
    {
        Vector3 desired = Vector3.zero;
        int count = 0;
        foreach (BoidAgent item in boids)
        {
            if (Vector3.Distance(item.transform.position, transform.position) <= viewRadius)
            {
                desired += item._velocity;
                count++;
            }
        }
        desired /= count;

        return CalculateSteering(desired.normalized * _maxSpeed);
    }

    Vector3 Cohesion(HashSet<BoidAgent> boids)
    {
        Vector3 desiredPos = Vector3.zero;
        int count = 0;
        foreach (var item in boids)
        {
            if (item != this) continue;

            if (Vector3.Distance(item.transform.position, transform.position) <= viewRadius)
            {
                desiredPos += item.transform.position;
                count++;
            }
        }
        if (count == 0) return Vector3.zero;
        desiredPos /= count;

        return Seek(desiredPos);
    }

    Vector3 Separation(HashSet<BoidAgent> boids)
    {
        Vector3 desired = Vector3.zero;
        foreach (var item in boids)
        {
            if (item == this || Vector3.Distance(item.transform.position, transform.position) > separationRadius) continue;
            desired += item.transform.position - transform.position;
        }
        if (desired == Vector3.zero) return desired;
        desired = -desired.normalized * _maxSpeed;

        return CalculateSteering(desired);
    }
    #endregion
    
    #region new flocking

    Vector3 SeparationNew(IEnumerable<GridEntity> entities)
    {
        Vector3 desired = Vector3.zero;
        foreach (var item in entities)
        {
            if (item == this || Vector3.Distance(item.transform.position, transform.position) > separationRadius) continue;
            desired += item.transform.position - transform.position;
        }
        if (desired == Vector3.zero) return desired;
        desired = -desired.normalized * _maxSpeed;

        return CalculateSteering(desired);
    }

    Vector3 AlignmentNew(IEnumerable<GridEntity> entities)
    {
        Vector3 desired = Vector3.zero;
        int count = 0;
        foreach (var item in entities)
        {
            if (Vector3.Distance(item.transform.position, transform.position) <= viewRadius)
            {
                desired += _velocity;
                count++;
            }
        }
        desired /= count;

        return CalculateSteering(desired.normalized * _maxSpeed);
    }

    Vector3 CohesionNew(IEnumerable<GridEntity> entities)
    {
        Vector3 desiredPos = Vector3.zero;
        int count = 0;
        foreach (var item in entities)
        {
            if (item != this) continue;

            if (Vector3.Distance(item.transform.position, transform.position) <= viewRadius)
            {
                desiredPos += item.transform.position;
                count++;
            }
        }
        if (count == 0) return Vector3.zero;
        desiredPos /= count;

        return Seek(desiredPos);
    }
    #endregion

    void Evade()
    {
        if (fleeTarget != null)
        {
            Vector3 hunterVelocity = Hunter._velocity;
            Vector3 toTarget = fleeTarget.position - transform.position;
            float distance = toTarget.magnitude;
            float time = distance / _maxSpeed;
            Vector3 targetPos = fleeTarget.position + hunterVelocity * time;
            Vector3 desired = (transform.position - targetPos).normalized * _maxSpeed;
            AddForce(desired - _velocity * 2);
        }
    }

    public Vector3 MyVelocity() { return _velocity; }

    private IEnumerator ResetAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(true);
        _maxSpeed = initialSpeed;
        BoidManager.Instance.AddBoid(this);
    }

    public IEnumerable<GridEntity> Query()
    {
        //creo una "caja" con las dimensiones deseadas, y luego filtro segun distancia para formar el círculo
        return _targetGrid.Query(
            transform.position + new Vector3(-_radius, 0, -_radius),
            transform.position + new Vector3(_radius, 0, _radius),
            x => {
                var position2d = x - transform.position;
                position2d.y = 0;
                return position2d.sqrMagnitude < _radius * _radius;
            });
    }

    private void OnDrawGizmos()
    {
        Vector3 origin1 = transform.position + transform.right / 4;
        Vector3 origin2 = transform.position - transform.right / 4;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, separationRadius);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, viewRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(origin1, origin1 + transform.forward * viewRadius);
        Gizmos.DrawLine(origin2, origin2 + transform.forward * viewRadius);
    }
}
