using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityController : MonoBehaviour
{
    public Vector3 localGravity;

    private Rigidbody m_Rigidbody;
    // Start is called before the first frame update
    public void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Rigidbody.useGravity = false;
    }

    public void UseGravity()
    {
        m_Rigidbody.useGravity = true;
        // add force?
    }
}
