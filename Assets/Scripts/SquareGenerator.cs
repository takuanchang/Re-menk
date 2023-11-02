using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject m_Square;

    [SerializeField]
    private GameObject m_Piece;

    int m_Length = 8;

    public void InitializeBoard()
    {
        int count = m_Length * m_Length;
        for (int i = 0; i < count; i++)
        {
            var row = i % m_Length;
            var column = i / m_Length;
            var position = new Vector3(row, 0, column);
            var square = Instantiate(m_Square, position, Quaternion.identity, transform);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeBoard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
