using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
   
    

    // Update is called once per frame

    void OnCollisionEnter(Collision collision) 
    {
        

        if  (collision.gameObject.tag == "Boid")
        {
            Destroy(gameObject);

            
        }

        
    }
    
}