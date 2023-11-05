using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Rigidbody))]
public class Piece : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField]
    private float m_explosionParam = 2.0f;

    /// <summary>
    /// 表裏判定の許容誤差
    /// </summary>
    static readonly float epsilon = 0.2f;

    /// <summary>
    /// 駒の属するチーム
    /// </summary>
    public Team Team { get; private set; } = Team.None;

    /// <summary>
    /// 初期状態でどのチームに属しているかを与えて駒を初期化する
    /// </summary>
    /// <param name="initialTeam">初期状態のチーム</param>
    public void Initialize(Team initialTeam) {
        // 初期化時に無効なチームを設定しようとした場合はアサートする
        Assert.AreNotEqual(initialTeam, Team.None, $"Do NOT set invalid teams at initialization");
        // チームを設定する
        Team = initialTeam;
        // チームに応じて向きを設定する
        switch (initialTeam)
        {
            // 黒は表向き
            case Team.Black:
                transform.rotation = Quaternion.identity;
                break;

            // 白は表向き
            case Team.White:
                transform.rotation = Quaternion.Euler(180, 0, 0);
                break;

            // その他はありえない
            default:
                break;
        }
    }

    /// <summary>
    /// 駒の属するチームを更新する
    /// </summary>
    public void UpdateTeam()
    {
        Vector3 up = transform.up.normalized;
        if (Mathf.Abs(up.y) < epsilon) {
            Team = Team.None; // どちらともいえない
        } else if (up.y > 0.0f) {
            Team = Team.Black; // 表なら黒
        } else {
            Team = Team.White; // 裏なら白
        }
    }

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

    private Vector3 CalculateForce(Vector3 direction, float distance)
    {
        float magnification = m_explosionParam / distance;

        return magnification * direction;
    }

    private Vector3 CalculateTorque(Vector3 direction, float distance)
    {
        float magnification = m_explosionParam / distance;

        return magnification * direction;
    }

    // 今だけ
    // 吹っ飛ぶ処理
    public void Explosion(Vector3 pos)
    {
        Vector3 vec = transform.position - pos;
        float distance = vec.magnitude;

        vec.y += 5.0f;

        rb.AddForce(CalculateForce(vec.normalized, distance), ForceMode.Impulse);
        rb.AddTorque(CalculateTorque(new Vector3(vec.z, 0, -vec.x).normalized, distance), ForceMode.Impulse);
    }

    public void UseGravity()
    {
        rb.useGravity = true;
        rb.AddForce(new Vector3(0.0f, -10.0f, 0.0f), ForceMode.Impulse);
    }


    // 初期化処理
    private　void Start() {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    // 削除用プログラムを雑に導入
    private void Update()
    {
        if(transform.position.y < -10)
        {
            Destroy(gameObject);
        }
    }
}
