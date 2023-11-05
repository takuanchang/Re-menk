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
        StartCoroutine(DropPiece());
    }

    private void Update()
    {
        if(m_gameObjectAndSpawnedTime.Peek().spawnedTime + 1.5f < Time.time) // キューの先頭にあるコマが発生から1.5秒経っていれば削除する
        {
            (GameObject targetObject, float time) target = m_gameObjectAndSpawnedTime.Dequeue();
            Destroy(target.targetObject);
        }
    }

    IEnumerator DropPiece()
    {
        while (true)
        {
            float randomSpan = Random.Range(0.1f, 0.5f); // コマの発生間隔はランダムな方が見た目良さそう。0.1秒～0.5秒くらいがうるさすぎず静かすぎずかな

            float x = Random.Range(1.5f, 4.5f); // メニューの右側にランダムな位置に落とす
            var position = new Vector3(x, 4.5f, 5f); // コマの生成時にコライダーに押されて挙動が変わるのでちょっと上の方から落とす
            
            var spawnedPiece = Instantiate(m_Piece, position, Random.rotation, transform);
            spawnedPiece.GetComponent<Rigidbody>().useGravity = true;
            m_gameObjectAndSpawnedTime.Enqueue((spawnedPiece, Time.time)); // キューにコマのGameObjectと発生時刻を保存しておく。

            yield return new WaitForSeconds(randomSpan);
        }
    }
}
