using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{
    private Material m_Material;

    private float m_HitPoint;
    private const float InitialHP = 20.0f; // TODO:体力の減り方がウィンドウの大きさで変わってしまう

    [SerializeField]
    private float m_ExplosionParam = 0.1f;

    private Board m_Owner = null;
    private int m_Index = -1;

    public void Initialize(Board owner, int index)
    {
        m_Material = GetComponent<Renderer>().material;
        m_Material.EnableKeyword("_EMISSION");
        m_Material.EnableKeyword("_EmissionColor");

        m_HitPoint = InitialHP;

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
        m_HitPoint -= m_ExplosionParam * speed / distance;
        // Debug.Log(m_HitPoint);
        if (m_HitPoint < 0)
        {
            m_Owner.BreakSquare(m_Index);
        }
    }
}
