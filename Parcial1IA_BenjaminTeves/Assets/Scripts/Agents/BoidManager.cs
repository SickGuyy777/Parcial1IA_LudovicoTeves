using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    public float boundHeight;
    public float boundWidth;

    public HashSet<BoidAgent> allBoids = new HashSet<BoidAgent>();

    public static BoidManager Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public Vector3 BoundChecks(Vector3 pos)
    {
        if (pos.z > boundHeight / 2) pos.z = -boundHeight / 2;
        if (pos.z < -boundHeight / 2) pos.z = boundHeight / 2;
        if (pos.x < -boundWidth / 2) pos.x = boundWidth / 2;
        if (pos.x > boundWidth / 2) pos.x = -boundWidth / 2;

        return pos;
    }

    public void AddBoid(BoidAgent b)
    {
        if (!allBoids.Contains(b))
            allBoids.Add(b);
    }

    void OnDrawGizmos()
    {
        Vector3 topLeft = new Vector3(-boundWidth / 2, 0, boundHeight / 2);
        Vector3 topRight = new Vector3(boundWidth / 2, 0, boundHeight / 2);
        Vector3 botRight = new Vector3(boundWidth / 2, 0, -boundHeight / 2);
        Vector3 botLeft = new Vector3(-boundWidth / 2, 0, -boundHeight / 2);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, botRight);
        Gizmos.DrawLine(botRight, botLeft);
        Gizmos.DrawLine(botLeft, topLeft);
    }
}
