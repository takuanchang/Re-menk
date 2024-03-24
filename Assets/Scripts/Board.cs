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
    private Square m_Square;
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

    private bool m_IsBrokenAll = false;
    public bool IsBrokenAll => m_IsBrokenAll;


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
            var square = Instantiate(m_Square, position, Quaternion.identity, transform);
            square.Initialize(this, squareIndex);
            var effect = Instantiate(m_BrokenEffectOriginal, position, Quaternion.identity, m_BrokenEffectCollector);

            m_Squares.Add(square);
            m_BrokenEffects.Add(effect);
            m_ValidIndices.Add(squareIndex);
        }
    }

    public void BreakSquare(int squareIndex)
    {
        m_BrokenEffects[squareIndex].Play();
        m_Squares[squareIndex].gameObject.SetActive(false);
        m_ValidIndices.Remove(squareIndex);
        if (m_ValidIndices.Count == 0)
        {
            m_IsBrokenAll = true;
        }
    }

    public Vector3 GetSquarePosition(int squareIndex)
    {
        return m_Squares[squareIndex].transform.position;
    }

    public int GetRemainingSquaresNum()
    {
        return m_ValidIndices.Count;
    }

    public int GetBoardSize()
    {
        return m_Length * m_Length;
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
