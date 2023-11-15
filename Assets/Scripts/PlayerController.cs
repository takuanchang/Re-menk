using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using static UnityEditor.PlayerSettings;

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

    [SerializeField]
    private UnityEvent OnPieceThrown;

    /// <summary>
    /// 次の駒を用意してから操作可能にする
    /// </summary>
    public bool PrepareNextPiece()
    {
        // チーム未設定のまま駒を用意しようとした場合はアサートする
        Assert.AreNotEqual(Team, Team.None, $"No Team has been set for this PlayerController.");

        // 駒がないなら何もしない
        if (m_RemainingPieces <= 0) {
            return false;
        }

        // 駒を用意する
        var position = new Vector3(3.5f, 5.0f, 3.5f);
        m_Target = Instantiate(m_OriginPiece, position, Quaternion.identity, m_PiecesCollector.transform);
        m_Target.Initialize(Team);

        m_RemainingPieces--;

        // 操作可能にする
        IsPlayable = true;
        phase = Phase.SquareSelect;
        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_SquareLayerMask = LayerMask.GetMask("Square");
    }


    enum Phase {
        SquareSelect,
        //MoveCamera,
        ButtonUpWait,
        PieceThrow
    }

    private Phase phase = Phase.SquareSelect;
    private Vector3 targetPosition = Vector3.zero;
    private Queue<Tuple<float, Vector3>> mouseHistory = new Queue<Tuple<float, Vector3>>();
    private float sumTime = 0.0f;
    // 閾値
    static readonly float threshold = 0.3f;


    // Update is called once per frame
    void Update()
    {
        if (!IsPlayable)
        {
            return;
        }
        switch (phase)
        {
            // マス選択フェーズ
            case Phase.SquareSelect:
                // マウスからレイを飛ばす
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit, 10.0f, m_SquareLayerMask, QueryTriggerInteraction.Ignore))
                {
                    Vector3 pos = hit.collider.transform.position;
                    pos.y = 3.0f;
                    m_Target.transform.position = pos;
                }
                if (Input.GetMouseButtonDown(0))
                {
                    phase = Phase.ButtonUpWait;
                }
                break;
            // マウス押し直し待ち
            case Phase.ButtonUpWait:
                if (Input.GetMouseButtonDown(1))
                {
                    phase = Phase.SquareSelect;
                    break;
                }
                if (Input.GetMouseButtonDown(0))
                {
                    sumTime = 0.0f;
                    mouseHistory.Clear();
                    targetPosition = Input.mousePosition;
                    phase = Phase.PieceThrow;
                }
                break;
            // 投げるフェーズ
            case Phase.PieceThrow:
                if (Input.GetMouseButtonDown(1))
                {
                    phase = Phase.SquareSelect;
                    break;
                }

                float dt = Time.deltaTime;
                sumTime += dt;
                mouseHistory.Enqueue(Tuple.Create(dt, Input.mousePosition));
                while(sumTime - mouseHistory.Peek().Item1 > threshold)
                {
                    var p = mouseHistory.Dequeue();
                    sumTime -= p.Item1;
                }
                if (Input.GetMouseButtonUp(0))
                {
                    Debug.Log(CalculateSpeed(ref mouseHistory));
                    //Throw(pos);
                    //OnPieceThrown.Invoke();
                }
                break;
        }

        //// 選択したマスに落とす
        //if (Input.GetMouseButtonDown(0))
        //{
        //    // マウスからレイを飛ばす
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    if(Physics.Raycast(ray, out var hit, 10.0f, m_SquareLayerMask, QueryTriggerInteraction.Ignore))
        //    {
        //        Vector3 pos = hit.collider.transform.position;
        //        pos.y = 5.0f;
        //        Drop(pos);
        //        OnPieceThrown.Invoke();
        //    }
        //}
    }

    //public void Drop(Vector3 pos)
    //{
    //    if (m_Target == null)
    //    {
    //        Debug.Log("Error");
    //        return;
    //    }
    //    m_Target.transform.position = pos;
    //    m_Target.Shoot();
    //    m_Target = null;
    //    IsPlayable = false;
    //    return;
    //}


    private float speedParam = 1.0f;
    private float CalculateSpeed(ref Queue<Tuple<float, Vector3>> history)
    {
        float speed = 0.0f;
        var preHistory = history.Peek();
        bool isFirst = true;
        foreach(var p in history)
        {
            if (isFirst)
            {
                isFirst = false;
                continue;
            }
            speed += (p.Item2 - preHistory.Item2).magnitude / p.Item1;
            preHistory = p;
        }
        speed /= (float)history.Count - 1.0f;
        return speed * speedParam;
    }

    public void Throw(Vector3 pos)
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
