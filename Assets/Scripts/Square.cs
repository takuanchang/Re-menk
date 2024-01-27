using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{
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

    // マスがダメージを貰う処理
    public void OnExploded(Vector3 direction, float speed)
    {
        //Vector3 vec = transform.position - pos;
        //float distance = vec.magnitude;

        //vec.y += 5.0f;

        //rb.AddForce(CalculateForce(vec.normalized, distance), ForceMode.Impulse);
        //rb.AddTorque(CalculateTorque(new Vector3(vec.z, 0, -vec.x).normalized, distance), ForceMode.Impulse);
    }
}
