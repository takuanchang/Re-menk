using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{
    private static readonly float VelocityThreshold = 5.0f;

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

    // TODO:要調整
    public void TurnOn()
    {
        GetComponent<Renderer>().material.color = Color.yellow;
    }
    public void TurnOff()
    {
        GetComponent<Renderer>().material.color = Color.white;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Piece>(out var p))
        {
            if (collision.relativeVelocity.magnitude > VelocityThreshold)
            {
                p.Explode();
            }
        }
    }
}
