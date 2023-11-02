using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Piece : MonoBehaviour
{
    // 表裏判定の許容誤差
    static float epsilon = 0.2f;
    // 駒の状態の種類
    public enum Status {Stand, Front, Back};
    private Rigidbody rb;

    float explosion_param = 2.0f;

    // 表裏の判定
    public Status GetStatus()
    {
        Vector3 up = transform.up.normalized;
        if (Mathf.Abs(up.y) < epsilon) return Status.Stand;
        else if (up.y > 0.0f) return Status.Front;
        else return Status.Back;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    // Update is called once per frame

    // 本番はイベント実行時のみ呼ぶ
    public void GetColliders()
    {
        // 近くにあるコライダーを取得
        Collider[] hitColliders = new Collider[1000];
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, 4.01f, hitColliders);
        for (int i = 0; i < numColliders; i++)
        {
            // 自分以外の駒の時
            if(hitColliders[i].TryGetComponent<Piece>(out var p))
            {
                if (p == this)
                {
                    continue;
                }
                p.Explosion(transform.position);
            }
            // マスの時
            else if (hitColliders[i].TryGetComponent<Square>(out var m))
            {

            }
            else
            {
                continue;
            }
        }
    }

    // 今だけ
    // 吹っ飛ぶ処理
    public void Explosion(Vector3 pos)
    {
        Vector3 vec = transform.position - pos;
        float r = vec.magnitude;
        vec.y += 5.0f;
        rb.AddForce(explosion_param / (r * r * r) * vec, ForceMode.Impulse);
        rb.AddTorque(explosion_param / (r * r * r) * new Vector3(vec.z, 0.0f, -vec.x), ForceMode.Impulse);
    }

    public void UseGravity()
    {
        rb.useGravity = true;
        rb.AddForce(new Vector3(0.0f, -10.0f, 0.0f), ForceMode.Impulse);
    }
}
