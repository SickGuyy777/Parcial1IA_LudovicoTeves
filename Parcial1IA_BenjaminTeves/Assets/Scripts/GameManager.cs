using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public float boundWidth;
    public float boundHeight;

    public List<Boid> allBoids = new List<Boid>();

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void AddBoid(Boid b)
    {
        if (!allBoids.Contains(b))
            allBoids.Add(b);
    }

    public Vector3 ChangeObjPosition(Vector3 pos)
    {
        if (pos.z > boundHeight / 2) pos.z = -boundHeight / 2;
        if (pos.z < -boundHeight / 2) pos.z = boundHeight / 2;
        if (pos.x < -boundWidth / 2) pos.x = boundWidth / 2;
        if (pos.x > boundWidth / 2) pos.x = -boundWidth / 2;

        return pos;
    }



    private void OnDrawGizmos()
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
