using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{
    private Material m_material;

    private float m_HitPoint;
    private const float initial_HP = 25.0f; // TODO:体力の減り方がウィンドウの大きさで変わってしまう

    [SerializeField]
    private float m_explosionParam = 0.1f;

    private Board m_Board = null;
    private int m_Index = -1;

    public void RegisterBoard(Board board)
    {
        m_Board = board;
    }
    public void RegisterIndex(int index)
    {
        m_Index = index;
    }

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
    public void OnExploded(float distance, float speed)
    {
        // TODO:倍率は要調整
        m_HitPoint -= m_explosionParam * speed / distance;
        Debug.Log(m_HitPoint);
        if (m_HitPoint < 0)
        {
            m_Board.BreakSquare(m_Index);
        }
    }
}
