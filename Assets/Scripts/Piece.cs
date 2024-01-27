using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Rigidbody))]
public class Piece : MonoBehaviour
{
    private bool m_isDead = false;
    private Rigidbody rb;

    [SerializeField]
    private float m_explosionParam = 2.0f;

    /// <summary>
    /// 表裏判定の許容誤差
    /// </summary>
    static readonly float Epsilon = 0.2f;

    /// <summary>
    /// 駒の属するチーム
    /// </summary>
    public Team Team { get; private set; } = Team.None;

    // 爆発出来る速さの最小値
    private static readonly float ExplodableSpeedMin = 5.0f;


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

        // 駒のRigidbodyを取得して重力を切る
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;

        // フラグを切る
        m_isDead = false;
    }

    /// <summary>
    /// 駒の属するチームを更新する
    /// </summary>
    public void UpdateTeam()
    {
        if (m_isDead)
        {
            return;
        }

        Vector3 up = transform.up.normalized;
        if (Mathf.Abs(up.y) < Epsilon) {
            Team = Team.None; // どちらともいえない
        } else if (up.y > 0.0f) {
            Team = Team.Black; // 表なら黒
        } else {
            Team = Team.White; // 裏なら白
        }
    }

    public void Explode(float speedOnCollision)
    {
        // 近くにあるコライダーを取得
        const float radius = 2.0f;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach(var collider in hitColliders)
        {
            if(collider.TryGetComponent<Piece>(out var p))
            {
                if (p == this)
                {
                    continue;
                }
                p.OnExploded(transform.position);
            }
            // マスの時
            else if (collider.TryGetComponent<Square>(out var m))
            {
                // マスのダメージ計算等を用意する
            }
            else
            {
                continue;
            }
        }
    }

    private Vector3 CalculateForce(Vector3 direction, float distance)
    {
        float magnification = m_explosionParam / (distance * distance);

        return magnification * direction;
    }

    private Vector3 CalculateTorque(Vector3 direction, float distance)
    {
        float magnification = m_explosionParam / (distance * distance);

        return magnification * direction;
    }

    // 吹っ飛ぶ処理
    public void OnExploded(Vector3 pos)
    {
        Vector3 vec = transform.position - pos;
        float distance = vec.magnitude;

        vec.y += 5.0f;

        rb.AddForce(CalculateForce(vec.normalized, distance), ForceMode.Impulse);
        rb.AddTorque(CalculateTorque(new Vector3(vec.z, 0, -vec.x).normalized, distance), ForceMode.Impulse);
    }


    public void Shoot(Vector3 dir)
    {
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;
        rb.AddForce(dir, ForceMode.Impulse);
    }

    public bool IsStable()
    {
        return rb.IsSleeping() || m_isDead;
    }

    private bool ShouldBeDisabled()
    {
        return transform.position.y < -10;
    }

    // とりあえずprivateにする。今後の実装によってはpublicの方がいいので注意
    private void Kill()
    {
        m_isDead = true;
        Team = Team.None;
        gameObject.SetActive(false);
    }

    // 削除用プログラムを雑に導入
    private void Update()
    {
        if(ShouldBeDisabled())
        {
            Kill();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Piece>(out var p))
        {
            // TODO:当たったのが駒の時、何か考えてもいいかも
        }
        if (collision.gameObject.TryGetComponent<Square>(out _))
        {
            var relativeSpeed = collision.relativeVelocity.magnitude;
            if (relativeSpeed > ExplodableSpeedMin)
            {
                this.Explode(relativeSpeed);
            }
        }
    }
}
