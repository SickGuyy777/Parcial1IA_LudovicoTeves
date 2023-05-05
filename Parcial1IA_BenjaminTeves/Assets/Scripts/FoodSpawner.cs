using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public GameObject foodPrefab;

    public GameObject agents;

    public List<GameObject> foodList;



    public int cantidad = 15;

    // Update is called once per frame
    void Start()
    {
        Spawn();
    }

    void Spawn()
    {

        do
        {
            Vector3 randomSpawnPosition = new Vector3(Random.Range(-14, 16), 0, Random.Range(-9, 10));

            GameObject newFood = Instantiate(foodPrefab, randomSpawnPosition, Quaternion.identity);
            foodList.Add(newFood);

            cantidad--;

        }

        while (cantidad >= 1);


    }
}
