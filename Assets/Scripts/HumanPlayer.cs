using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

// 人間側の実装とする

[Serializable]
public class HumanPlayer : MonoBehaviour , IPlayer
{
    /// <summary>
    /// このプレイヤーのチーム
    /// </summary>
    public Team Team { get; private set; } = Team.None;

    private GameObject m_TurnManager = null;

    /// <summary>
    /// このプレイヤーが操作可能かどうか
    /// </summary>
    public bool IsPlayable { get; private set; } = false;

    /// <summary>
    /// このプレイヤーに残っている駒の数
    /// 但し操作中の駒はカウントされない
    /// </summary>
    public int RemainingPieces { get; private set; } = 32;

    private int m_SquareLayerMask;

    [SerializeField]
    private Piece m_OriginalPiece;
    private Piece m_Target;

    private Transform m_PiecesCollector;

    [SerializeField]
    private Camera m_MainCamera;

    // オンライン・NPC対戦の場合は待機中にFreeLookCameraを使う
    // オフライン対戦の場合はDollyCameraを使う
    // Cinemachine.CinemachineVirtualCameraBaseにどちらかを代入して使う
    [SerializeField]
    private Cinemachine.CinemachineVirtualCameraBase m_FreeLookCamera;

    [SerializeField] private Cinemachine.CinemachineVirtualCamera m_PieceCamera;

    public enum Phase {
        SquareSelect,
        //MoveCamera,
        ButtonUpWait,
        PieceThrow
    }

    private Phase m_Phase = Phase.SquareSelect;

    private Vector3 targetPosition = Vector3.zero;
    private Queue<MouseLog> m_MouseHistory = new();
    private float sumTime = 0.0f;
    // 閾値
    static readonly float threshold = 0.1f;

    float directionParam = 3.0f;

    [SerializeField]
    private float speedParam = 1.0f;

    readonly struct MouseLog {
        public readonly float deltaTime;
        public readonly Vector3 position;

        public MouseLog(float deltaTime, Vector3 position) {
            this.deltaTime = deltaTime;
            this.position = position;
        }

        public void Deconstruct(out float deltaTime, out Vector3 position) {
            deltaTime = this.deltaTime;
            position = this.position;
        }
    }

    // ------------------------------------------------------------------------------------------

    public void Initialize(Team team, GameObject turnManager, Transform piecesCollector)
    {
        Team = team;
        m_TurnManager = turnManager;
        m_PiecesCollector = piecesCollector;
    }

    public void SetupCameras(Camera main, Cinemachine.CinemachineVirtualCameraBase freeLook, Cinemachine.CinemachineVirtualCamera piece)
    {
        m_MainCamera = main;
        m_FreeLookCamera = freeLook;
        m_PieceCamera = piece;
    }

    /// <summary>
    /// 次の駒を用意してから操作可能にする
    /// </summary>
    public bool PrepareNextPiece()
    {
        // チーム未設定のまま駒を用意しようとした場合はアサートする
        Assert.AreNotEqual(Team, Team.None, "No Team has been set for this PlayerController.");

        // 駒がないなら何もしない
        if (RemainingPieces <= 0) {
            return false;
        }

        // 駒を用意する
        var position = new Vector3(3.5f, 5.0f, 3.5f);
        m_Target = Instantiate(m_OriginalPiece, position, Quaternion.identity, m_PiecesCollector.transform);
        m_Target.Initialize(Team);

        RemainingPieces--;

        // 操作可能にする
        IsPlayable = true;
        m_Phase = Phase.SquareSelect;

        // カメラを俯瞰視点にする
        m_PieceCamera.Priority = 9; // fixme : 相手のカメラのプライオリティが上がったままなので切り替わらない。修正する
        m_FreeLookCamera.Priority = 8;

        return true;
    }

    public string CurrentPhaseString()
    {
        return m_Phase.ToString();
    }

    private Vector3 CalcurateDirection(Vector3 mousePos) {
        var gap = mousePos - targetPosition;
        gap /= MathF.Min(Screen.width, Screen.height);
        gap *= directionParam;
        gap.z = gap.y;
        gap.y = 0.0f;
        return gap;
    }

    private float CalculateSpeed(Queue<MouseLog> history) {
        float speed = 0.0f;
        var length = MathF.Min(Screen.width, Screen.height);
        var (_, prePos) = history.Dequeue();
        foreach (var (dt, pos) in history) {
            var deltaPos = (pos - prePos) / length;
            speed += MathF.Sign(deltaPos.y) * deltaPos.magnitude / dt;
            (_, prePos) = (dt, pos);
        }
        speed /= history.Count;
        return speed * speedParam;
    }

    public void Throw(Vector3 dir) {
        Assert.IsNotNull(m_Target, "The piece trying to be thrown doesn't exist.");
        m_Target.Shoot(dir);
        m_Target = null;
        IsPlayable = false;
        return;
    }

    // ------------------------------------------------------------------------------------------

    void Start() {
        m_SquareLayerMask = LayerMask.GetMask("Square");
    }

    void Update()
    {
        if (!IsPlayable)
        {
            return;
        }

        switch (m_Phase)
        {
            // マス選択フェーズ
            case Phase.SquareSelect:
                // マウスからレイを飛ばす

                Ray ray = m_MainCamera.ScreenPointToRay(Input.mousePosition); // 人間依存
                if (Physics.Raycast(ray, out var hit, 10.0f, m_SquareLayerMask, QueryTriggerInteraction.Ignore)) // 人間依存
                {
                    Vector3 pos = hit.collider.transform.position;
                    pos.y = 3.0f;
                    m_Target.transform.position = pos;
                }

                if (Input.GetMouseButtonDown(0)) // 人間依存
                {
                    m_Phase = Phase.ButtonUpWait;

                    m_PieceCamera.Follow = m_Target.transform;
                    m_PieceCamera.LookAt = m_Target.transform;
                    m_PieceCamera.Priority = 11;
                }

                break;
            // 人間：マウス押し直し待ち
            // CPU : カメラ移動待ち
            case Phase.ButtonUpWait: // フェーズの名前が人間依存なので変えたほうがgood
                if (Input.GetMouseButtonDown(1))
                {
                    m_Phase = Phase.SquareSelect;
                    m_PieceCamera.Priority = 9;
                    break;
                }
                if (Input.GetMouseButtonDown(0))
                {
                    sumTime = 0.0f;
                    m_MouseHistory.Clear();
                    targetPosition = Input.mousePosition;
                    m_Phase = Phase.PieceThrow;
                }
                break;
            // 投げるフェーズ
            case Phase.PieceThrow:
                if (Input.GetMouseButtonDown(1))
                {
                    m_Phase = Phase.SquareSelect;
                    m_PieceCamera.Priority = 9;
                    break;
                }

                // 人間はこんな感じ
                float dt = Time.deltaTime;
                sumTime += dt;
                var mousePos = Input.mousePosition;
                m_MouseHistory.Enqueue(new(dt, mousePos));
                while (sumTime - m_MouseHistory.Peek().deltaTime > threshold)
                {
                    var (deltaTime, _) = m_MouseHistory.Dequeue();
                    sumTime -= deltaTime;
                }
                if (Input.GetMouseButtonUp(0))
                {
                    m_PieceCamera.Priority = 9;
                    m_FreeLookCamera.Priority = 11;
                    var dir = CalcurateDirection(mousePos);
                    dir.y = CalculateSpeed(m_MouseHistory);
                    Throw(dir);
                    m_TurnManager.SendMessage("OnPieceThrown");
                }

                // CPU側でもアニメーションを見せるなら数字を決めるだけではだめ
                // 候補1 : いくつかのアニメーションを用意しておく
                // 候補2 : その場でいい感じに計算してThrowする
                // 開始点、折り返し点、終点を計算し、補間する点をMouseHistory(現在名)に入れる

                break;
        }
    }
}
