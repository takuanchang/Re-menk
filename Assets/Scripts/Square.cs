using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{
    private Material m_material;

    private float m_HitPoint;
    private const float initialHP = 20.0f; // TODO:体力の減り方がウィンドウの大きさで変わってしまう

    [SerializeField]
    private float m_explosionParam = 0.1f;

    private Board m_Owner = null;
    private int m_Index = -1;

    public void Initialize(Board owner, int index)
    {
        m_material = GetComponent<Renderer>().material;
        m_material.EnableKeyword("_EMISSION");
        m_material.EnableKeyword("_EmissionColor");

        m_HitPoint = initialHP;

        m_Owner = owner;
        m_Index = index;
    }

    /*
    // Update is called once per frame
    void Update()
    {
        //ToDo
    }
    */

    // マスがダメージを貰う処理
    public void OnExploded(float distance, float speed)
    {
        // TODO:倍率は要調整
        m_HitPoint -= m_explosionParam * speed / distance;
        // Debug.Log(m_HitPoint);
        if (m_HitPoint < 0)
        {
            m_Owner.BreakSquare(m_Index);
        }
    }
}
