using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitlePieceAnimator : MonoBehaviour
{
    [SerializeField] private GameObject m_Piece;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("DropPiece");
    }

    IEnumerator DropPiece()
    {
        while (true)
        {
            float randomSpan = Random.Range(0.1f, 0.3f);

            yield return new WaitForSeconds(randomSpan);

            float x = Random.Range(1.5f, 4.5f);
            var position = new Vector3(x, 3.5f, 5f);
            
            Instantiate<GameObject>(m_Piece, position, Random.rotation, transform);
        }
    }
}
