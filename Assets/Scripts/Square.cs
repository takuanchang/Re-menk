using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{
    static private float ep = 5.0f;

    /*
    // Start is called before the first frame update
    void Start()
    {
        //ToDo
    }

    // Update is called once per frame
    void Update()
    {
        //ToDo
    }
    */

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Piece>(out var p))
        {
            if (collision.relativeVelocity.magnitude > ep)
            {
                p.GetColliders();
            }
        }
    }
}
