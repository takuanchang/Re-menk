using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitlePieceAnimator : MonoBehaviour
{
    [SerializeField] private GameObject m_Piece;
    private Queue<(GameObject gameObject, float spawnedTime)> m_gameObjectAndSpawnedTime = new Queue<(GameObject gameObject, float spawnedTime)>();

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("DropPiece");
    }

    private void Update()
    {
        if(m_gameObjectAndSpawnedTime.Peek().spawnedTime + 1.5f < Time.time)
        {
            (GameObject targetObject, float time) target = m_gameObjectAndSpawnedTime.Dequeue();
            Destroy(target.targetObject);
        }
    }

    IEnumerator DropPiece()
    {
        while (true)
        {
            float randomSpan = Random.Range(0.1f, 0.5f);

            float x = Random.Range(1.5f, 4.5f);
            var position = new Vector3(x, 4.5f, 5f);
            
            var spawnedPiece = Instantiate<GameObject>(m_Piece, position, Random.rotation, transform);
            m_gameObjectAndSpawnedTime.Enqueue((spawnedPiece, Time.time));

            yield return new WaitForSeconds(randomSpan);
        }
    }
}
