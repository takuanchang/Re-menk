using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityController : MonoBehaviour
{
    public Vector3 localGravity;

    private Rigidbody rb;
    // Start is called before the first frame update
    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    public void UseGravity()
    {
        rb.useGravity = true;
        // add force?
    }
}
