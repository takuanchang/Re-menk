using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(Rigidbody))]
public class Piece : MonoBehaviour
{
    private PiecesManager m_PiecesManager = null;

    private bool m_isDead = false;
    private bool m_WillBeKilled = false;
    private Rigidbody rb;

    private IPlayer m_Thrower;

    public IPlayer Thrower
    {
        get => m_Thrower;

        set
        {
            m_Thrower = value;
        }
    }

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

    private Team m_UpTeam, m_BottomTeam;

    /// <summary>
    /// 爆発出来る速さの最小値
    /// </summary>
    private static readonly float ExplodableSpeedMin = 5.0f;
    [SerializeField]
    private GameObject m_SpeedEffect;
    [SerializeField]
    private ParticleSystem m_ExplosionEffect;
    [SerializeField]
    private Material m_UpMaterial;
    [SerializeField]
    private Material m_BottomMaterial;

    private GameObject m_Kiraan;


    /// <summary>
    /// 爆発時インパルスで追加する上方向ベクトル
    /// </summary>
    private static readonly float AddedYOnExploded = 5.0f;

    /// <summary>
    /// 駒を利用不可にする高さ(床)
    /// </summary>
    private static readonly float YMinLimit = -10;

    /// <summary>
    ///  駒を利用不可にする高さ(天井)
    /// </summary>
    private static readonly float YMaxLimit = 30;

    /// <summary>
    /// 各チームの色
    /// </summary>
    private static readonly Color[] TeamColors = { Color.black, Color.white, Color.blue };

    /// <summary>
    /// 初期状態でどのチームに属しているかを与えて駒を初期化する
    /// </summary>
    /// <param name="initialTeam">初期状態のチーム</param>
    public void Initialize(Team initialTeam, PiecesManager piecesManager, int teamNum) {
        // 初期化時に無効なチームを設定しようとした場合はアサートする
        Assert.AreNotEqual(initialTeam, Team.None, $"Do NOT set invalid teams at initialization");
        // チームを設定する
        Team = initialTeam;
        // 管理者を設定
        m_PiecesManager= piecesManager;

        Material[] pieceMaterials = GetComponent<MeshRenderer>().materials;
        Shader upShader = pieceMaterials[0].shader;
        Shader bottomShader = pieceMaterials[1].shader;
        GetComponent<MeshRenderer>().materials[0] = new Material(upShader);
        GetComponent<MeshRenderer>().materials[1] = new Material(bottomShader);

        // チームに応じて向きを設定する
        switch (initialTeam)
        {
            // 黒は表向き
            case Team.Black:
                transform.rotation = Quaternion.identity;
                pieceMaterials[0].color = TeamColors[((int)Team.Black)];
                pieceMaterials[1].color = TeamColors[((int)Team.Black + 1) % teamNum];
                m_UpTeam = Team.Black;
                m_BottomTeam = (Team)(((int)Team.Black + 1) % teamNum);
                //m_UpMaterial.color = new Color(TeamColors[((int)Team.Black)].r, TeamColors[((int)Team.Black)].g, TeamColors[((int)Team.Black)].b);
                //m_UpMaterial.color = new Color(TeamColors[((int)Team.Black + 1) % teamNum].r, TeamColors[((int)Team.Black + 1) % teamNum].g, TeamColors[((int)Team.Black + 1) % teamNum].b);
                break;

            // 白は表向き
            case Team.White:
                Debug.Log(initialTeam);
                transform.rotation = Quaternion.identity;
                //m_UpMaterial.color = new Color(TeamColors[((int)Team.White)].r, TeamColors[((int)Team.White)].g, TeamColors[((int)Team.White)].b);
                //m_UpMaterial.color = new Color(TeamColors[((int)Team.White + 1) % teamNum].r, TeamColors[((int)Team.White + 1) % teamNum].g, TeamColors[((int)Team.White + 1) % teamNum].b);
                pieceMaterials[0].color = TeamColors[((int)Team.White)];
                pieceMaterials[1].color = TeamColors[((int)Team.White + 1) % teamNum];
                m_UpTeam = Team.White;
                m_BottomTeam = (Team)(((int)Team.White + 1) % teamNum);
                break;

            case Team.Blue:
                transform.rotation = Quaternion.identity;
                //m_UpMaterial.color = new Color(TeamColors[((int)Team.Blue)].r, TeamColors[((int)Team.Blue)].g, TeamColors[((int)Team.Blue)].b);
                //m_UpMaterial.color = new Color(TeamColors[((int)Team.Blue + 1) % teamNum].r, TeamColors[((int)Team.Blue + 1) % teamNum].g, TeamColors[((int)Team.Blue + 1) % teamNum].b);
                pieceMaterials[0].color = TeamColors[((int)Team.Blue)];
                pieceMaterials[1].color = TeamColors[((int)Team.Blue + 1) % teamNum];
                m_UpTeam = Team.Blue;
                m_BottomTeam = (Team)(((int)Team.Blue + 1) % teamNum);
                break;

            // TODO:他のチームをどうするか
            default:
                break;
        }

        // 駒のRigidbodyを取得して重力を切る
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;

        // Shoot時に表示するエフェクトを非表示にする
        m_SpeedEffect.SetActive(false);

        m_ExplosionEffect.Stop();
        m_ExplosionEffect.Clear();

        m_Kiraan = GameObject.Find("Kiraan");
        m_Kiraan.GetComponent<MeshRenderer>().enabled = false;

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
            Team = m_UpTeam;
        } else {
            Team = m_BottomTeam;
        }
    }

    public void Explode(float speedOnCollision)
    {
        m_ExplosionEffect.Play();

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
                Vector3 difference = collider.transform.position - transform.position;
                // difference.y = 0.1f;
                m.OnExploded(difference.magnitude, speedOnCollision);
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

        vec.y += AddedYOnExploded;

        rb.AddForce(CalculateForce(vec.normalized, distance), ForceMode.Impulse);
        rb.AddTorque(CalculateTorque(new Vector3(vec.z, 0, -vec.x).normalized, distance), ForceMode.Impulse);
        // EndTurnの時間を再設定してもらう
        m_PiecesManager.RequestResetEndTurn();
    }


    public void Shoot(Vector3 dir)
    {
        m_SpeedEffect.SetActive(true);

        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;
        rb.AddForce(dir, ForceMode.Impulse);
    }

    public void Stop()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public bool IsStable()
    {
        return rb.IsSleeping() || m_isDead;
    }

    private bool IsUnderGround()
    {
        return transform.position.y < YMinLimit;
    }

    private bool IsUpperCeil()
    {
        return transform.position.y > YMaxLimit;
    }

    private bool ShouldBeDisabled()
    {
        return IsUnderGround() || IsUpperCeil();
    }

    // とりあえずprivateにする。今後の実装によってはpublicの方がいいので注意
    private void Kill()
    {
        m_isDead = true;
        Team = Team.None;
        gameObject.SetActive(false);
    }

    // 削除用プログラムを雑に導入
    private async void Update()
    {
        if (ShouldBeDisabled())
        {
            if (IsUpperCeil() && !m_WillBeKilled)
            {
                // TODO: カメラを上に向けてキラーン✨
                // Throwerにメッセージを送ってカメラを操作してもらう？
                // FreeLookCameraのLookAtを消滅したポイントに変更する
                // なんか色々おかしいので周辺の処理を考え直す
                m_WillBeKilled = true;

                // 指定の速度で上に飛ばす
                rb.useGravity = false;
                rb.velocity = Vector3.zero;
                rb.AddForce(new Vector3(0, 20.0f, 0), ForceMode.Impulse);

                Thrower.LookUpSky(); // ParticleEffectのtransformって使えたっけ？
                await UniTask.Delay(1000);
                m_Kiraan.GetComponent<MeshRenderer>().enabled = true;
                m_Kiraan.GetComponent<Transform>().position = transform.position;
                m_Kiraan.GetComponent<Animator>().Play("KiraanAnime", 0, 0.0f);
                this.GetComponent<MeshRenderer>().enabled = false;
                // m_Kiraan.GetComponent<Transform>().rotation = Quaternion.Euler(0, 0, 0);
                await UniTask.Delay(1000);
                m_Kiraan.GetComponent<MeshRenderer>().enabled = false;

                Kill();
            }
            else if(!m_WillBeKilled)
            {
                m_WillBeKilled = true;
                Kill();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        m_SpeedEffect.SetActive(false);

        if (collision.gameObject.TryGetComponent<Piece>(out var p))
        {
            // TODO:当たったのが駒の時、何か考えてもいいかも
        }
        if (collision.gameObject.TryGetComponent<Square>(out _))
        {
            var relativeSpeed = collision.relativeVelocity.magnitude;
            // Debug.Log(relativeSpeed);
            if (relativeSpeed > ExplodableSpeedMin)
            {
                this.Explode(relativeSpeed);
            }
        }
    }
}
