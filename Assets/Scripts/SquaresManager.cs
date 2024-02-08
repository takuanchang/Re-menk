using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SquaresManager : MonoBehaviour
{
    [SerializeField]
    private Square m_Square;

    [SerializeField]
    private BrokenEffectCollector m_BrokenEffectCollector;

    int m_Length = 8;
    private Vector3 m_CenterOffset = new Vector3(-3.5f, 0, -3.5f);

    private List<Square> m_Squares = null;
    private SortedSet<int> m_ValidIndices = null;
    public IEnumerable<int> ValidIndices => m_ValidIndices;

    public void InitializeBoard()
    {
        int count = m_Length * m_Length;
        m_Squares = new List<Square> (count);
        m_ValidIndices = new();

        for (int squareIndex = 0; squareIndex < count; squareIndex++)
        {
            var row = squareIndex % m_Length;
            var column = squareIndex / m_Length;
            var position = new Vector3(row, 0, column) + m_CenterOffset;
            var square = Instantiate(m_Square, position, Quaternion.identity, transform);
            square.SetEffectCollector(m_BrokenEffectCollector);
            m_Squares.Add(square);
            m_ValidIndices.Add(squareIndex);
        }
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
