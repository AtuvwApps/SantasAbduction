using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingLight : MonoBehaviour
{
    public GameObject alien;

    // Update is called once per frame
    void Update()
    {
        transform.position = alien.GetComponent<Rigidbody2D>().position;
    }
}
