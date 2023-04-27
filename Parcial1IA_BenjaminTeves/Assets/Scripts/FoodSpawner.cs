using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public GameObject foodPrefab;

    public GameObject agents;

    public float time = 5;

    public int a = 10;

    // Update is called once per frame
    void Start()
    {
        Invoke("Spawn", time);
    }

    void Spawn()
    {

        do
        {
            Vector3 randomSpawnPosition = new Vector3(Random.Range(-14, 16), 0, Random.Range(-9, 10));

            Instantiate(foodPrefab, randomSpawnPosition, Quaternion.identity);
            a--;
        }

        while (a >= 1);


    }
}
