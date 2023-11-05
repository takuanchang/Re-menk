using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// このプレイヤーのチーム
    /// </summary>
    public Team Team { get; set; } = Team.None;

    /// <summary>
    /// このプレイヤーが操作可能かどうか
    /// </summary>
    public bool IsPlayable { get; private set; } = false;

    private int m_SquareLayerMask;

    public int m_RemainingPieces { get; private set; } = 32;

    [SerializeField]
    private Piece m_OriginPiece;
    private Piece m_Target;

    [SerializeField]
    private GameObject m_PiecesCollector;

    //[SerializeField]
    //private SteadyState m_State;

    /// <summary>
    /// 次の駒を用意してから操作可能にする
    /// </summary>
    public void PrepareNextPiece()
    {
        // チーム未設定のまま駒を用意しようとした場合はアサートする
        Assert.AreNotEqual(Team, Team.None, $"No Team has been set for this PlayerController.");

        // 駒がないなら何もしない
        if (m_RemainingPieces <= 0) {
            return;
        }

        // 駒を用意する
        var position = new Vector3(3.5f, 5.0f, 3.5f);
        m_Target = Instantiate(m_OriginPiece, position, Quaternion.identity, m_PiecesCollector.transform);
        m_Target.Initialize(Team);
        // 参照先を保存
        // m_State.SetRigidbody(m_Target.GetComponent<Rigidbody>());

        m_RemainingPieces--;

        // 操作可能にする
        IsPlayable = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_SquareLayerMask = LayerMask.GetMask("Square");
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsPlayable)
        {
            return;
        }

        // 選択したマスに落とす
        if (Input.GetMouseButtonDown(0))
        {
            // マウスからレイを飛ばす
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out var hit, 10.0f, m_SquareLayerMask, QueryTriggerInteraction.Ignore))
            {
                Vector3 pos = hit.collider.transform.position;
                pos.y = 5.0f;
                Drop(pos);
            }
        }
    }

    public void Drop(Vector3 pos)
    {
        if (m_Target == null)
        {
            Debug.Log("Error");
            return;
        }
        m_Target.transform.position = pos;
        m_Target.Shoot();
        m_Target = null;
        IsPlayable = false;
        return;
    }
}
