using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jikken3 : MonoBehaviour
{
    Vector3 accel = Vector3.zero;
    Vector3 velocity = Vector3.down;
    float param = 1f;
    public bool is_Use = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (is_Use)
        {
            accel = Vector3.zero - transform.position;
            accel = param * accel.normalized;
            velocity += accel * Time.deltaTime;
        }
        Debug.Log(accel);
        //Debug.Log(velocity);
        transform.position += velocity * Time.deltaTime;
    }
}
