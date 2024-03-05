using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Board : MonoBehaviour
{
    [SerializeField]
    private Square m_SquarePrefab;
    [SerializeField]
    private ParticleSystem m_BrokenEffectOriginal;

    [SerializeField]
    private Transform m_BrokenEffectCollector;

    int m_Length = 8;
    private Vector3 m_CenterOffset = new Vector3(-3.5f, 0, -3.5f);

    private List<Square> m_Squares = null;
    private List<ParticleSystem> m_BrokenEffects = null;

    private List<int> m_ValidIndices = null;
    public IReadOnlyList<int> ValidIndices => m_ValidIndices;

    public void InitializeBoard()
    {
        int count = m_Length * m_Length;
        m_Squares = new List<Square>(count);
        m_BrokenEffects = new List<ParticleSystem>(count);
        m_ValidIndices = new List<int>(count);

        for (int squareIndex = 0; squareIndex < count; squareIndex++)
        {
            var row = squareIndex % m_Length;
            var column = squareIndex / m_Length;
            var position = new Vector3(row, 0, column) + m_CenterOffset;
            var square = Instantiate(m_SquarePrefab, position, Quaternion.identity, transform);
            square.Initialize(this, squareIndex);
            var effect = Instantiate(m_BrokenEffectOriginal, position, Quaternion.identity, m_BrokenEffectCollector);

            m_Squares.Add(square);
            m_BrokenEffects.Add(effect);
            m_ValidIndices.Add(squareIndex);
        }
    }

    public int GetCenterIndex()
    {
        return m_Length / 2 * m_Length + m_Length / 2;
    }

    public int GetNextSquareIndex(int currentIndex, Vector2 inputDirection)
    {
        Vector2Int direction = new Vector2Int();
        inputDirection.Normalize();
        // 右:1, 左:-1
        if (inputDirection.x >= 0.5f)
        {
            direction.x = 1;
        }
        else if (inputDirection.x > -0.5f)
        {
            direction.x = 0;
        }
        else
        {
            direction.x = -1;
        }

        // 上:1, 下:-1
        if (inputDirection.y >= 0.5f)
        {
            direction.y = 1;
        }
        else if (inputDirection.x > -0.5f)
        {
            direction.y = 0;
        }
        else
        {
            direction.y = -1;
        }

        var nextRow = Mathf.Clamp(currentIndex % m_Length + direction.x, 0, m_Length - 1);
        var nextColumn = Mathf.Clamp(currentIndex / m_Length + direction.y, 0, m_Length - 1);

        return nextRow + m_Length * nextColumn;
    }

    public Square GetSquare(int squareIndex)
    {
        return m_Squares[squareIndex];
    }


    public bool IsSelectable(int squareIndex)
    {
        return m_ValidIndices.Contains(squareIndex);
    }

    public void BreakSquare(int squareIndex)
    {
        m_BrokenEffects[squareIndex].Play();
        m_Squares[squareIndex].gameObject.SetActive(false);
        m_ValidIndices.Remove(squareIndex);
        //TODO:全破壊された場合の処理
    }

    public Vector3 GetSquarePosition(int squareIndex)
    {
        return m_Squares[squareIndex].transform.position;
    }

#if UNITY_EDITOR
    [ContextMenu("ボードのマスを配置")]
    public void InitializeBoardInEditor()
    {
        InitializeBoard();
    }
#endif

    // Start is called before the first frame update
    void Start()
    {
        // InitializeBoard();
    }
}
