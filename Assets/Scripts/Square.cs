using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{
    private static readonly float VelocityThreshold = 5.0f;

    private Material m_material;

    private float m_HitPoint;
    private const float initial_HP = 1.0f;

    public void Initialize()
    {
        m_material = GetComponent<Renderer>().material;
        m_material.EnableKeyword("_EMISSION");
        m_material.EnableKeyword("_EmissionColor");

        m_HitPoint = initial_HP;
    }

    void Start()
    {
        Initialize();
    }

    /*
    // Update is called once per frame
    void Update()
    {
        //ToDo
    }
    */

    // TODO:要調整
    public void TurnOn()
    {
        m_material.SetColor("_EmissionColor", Color.red);
    }
    public void TurnOff()
    {
        m_material.SetColor("_EmissionColor", Color.black);
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
