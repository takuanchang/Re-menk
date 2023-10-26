using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject m_Square;

    int m_Length = 8;

    public void InitializeBoard()
    {
        int count = m_Length * m_Length;
        for (int i = 0; i < count; i++)
        {
            var row = i % m_Length;
            var column = i / m_Length;
            var position = new Vector3(row * 10, 0, column * 10);
            var square = Instantiate(m_Square, position, Quaternion.identity, transform);
            //square.Initialize(new Vector2Int(row, column));

            // チェック柄作成 とりあえず色変えで誤魔化し
            //if ((row + (column % 2)) % 2 == 1)
            //{
            //    square.GetComponent<Renderer>().material.color = Color.black;
            //}
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
