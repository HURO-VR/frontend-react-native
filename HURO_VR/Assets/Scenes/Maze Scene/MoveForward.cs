using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward : MonoBehaviour
{

    public float speed = 5f; // Speed of movement
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Move the GameObject forward in the Z-direction
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        transform.Rotate(Vector3.up, 50f * Time.deltaTime); // Rotate around the Y-axis
    }
}
