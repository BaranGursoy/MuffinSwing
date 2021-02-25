using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boost : MonoBehaviour // This class is controlling boost objects' destroyement
{
    // Variables and constants
    Camera cam;
    float height;
    float width;
    // Variables and constants are over

    void Start()
    {
        // Setting the variables
        cam = FindObjectOfType<Camera>();
        height = 2f * cam.orthographicSize;
        width = height * cam.aspect;
    }

    void Update()
    {
        float x = cam.transform.position.x - (width / 2);

        if (x > (transform.position.x + (transform.localScale.x / 2))) // If a boost is out of screen, then destroy it
        {
            Destroy(gameObject);
        }
    }
}
