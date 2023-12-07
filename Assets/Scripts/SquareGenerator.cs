using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SquareGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject m_Square;

    [SerializeField]
    private GameObject m_Piece;

    int m_Length = 8;
    private Vector3 m_CenterOffset = new Vector3(-3.5f, 0, -3.5f);

    public void InitializeBoard()
    {
        int count = m_Length * m_Length;
        for (int i = 0; i < count; i++)
        {
            var row = i % m_Length;
            var column = i / m_Length;
            var position = new Vector3(row, 0, column) + m_CenterOffset;
            var square = Instantiate(m_Square, position, Quaternion.identity, transform);
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
