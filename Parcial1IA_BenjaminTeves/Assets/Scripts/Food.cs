using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{


    public float distanciaMinima = 0.1f;

    private void FixedUpdate()
    {
        GameObject agente = GameObject.FindGameObjectWithTag("Boid");

        if (agente != null)
        {
            Vector3 direccion = agente.transform.position - transform.position;
            float distancia = direccion.magnitude;

            if (distancia < distanciaMinima)
            {
                Destroy(gameObject);
            }
        }
    }

}