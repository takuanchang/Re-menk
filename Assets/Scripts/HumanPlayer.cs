using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using UnityEngine.Purchasing;

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
    [field:SerializeField]
    public int RemainingPieces { get; private set; } = 2; // TODO:実際は32等に

    private int m_SquareLayerMask;

    private Piece m_Target;
    // 選択中のマスのコライダー
    private Collider m_SquareCollider = null;

    private PiecesManager m_PiecesManager;

    [SerializeField] private Camera m_MainCamera;
    // オンライン・NPC対戦の場合は待機中にFreeLookCameraを使う
    // オフライン対戦の場合はDollyCameraを使う
    // Cinemachine.CinemachineVirtualCameraBaseにどちらかを代入して使う
    private Cinemachine.CinemachineVirtualCameraBase m_SelectCamera;
    private Cinemachine.CinemachineVirtualCameraBase m_FreeLookCamera;
    private Cinemachine.CinemachineVirtualCamera m_PieceCamera;
    private Cinemachine.CinemachineVirtualCamera m_KiraanCamera;
    private Transform m_Sky;


    [SerializeField] private float rayLength = 20.0f;

    private Transform m_Reticule;
    private ReticuleControler m_ReticuleControler;

    // フェーズ
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
    // 速さ決定時の履歴保存秒数の閾値
    static readonly float threshold = 0.1f;

    // パラメータ群
    [SerializeField] private float directionParam = 3.0f;
    [SerializeField] private float speedParam = 1.0f;

    /// <summary>
    /// ピースの高さ
    /// </summary>
    private static readonly float PiecePositionY = 3.0f;

    /// <summary>
    /// 使用しているカメラの優先度
    /// </summary>
    private static readonly int UsingPriority = 11;
    /// <summary>
    /// 使用していないカメラの優先度
    /// </summary>
    private static readonly int NonUsingPriority = 9;

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

    public void Initialize(Team team, GameObject turnManager, PiecesManager piecesManager, int piecesNum)
    {
        Team = team;
        m_TurnManager = turnManager;
        m_PiecesManager = piecesManager;
        RemainingPieces = piecesNum;

        m_ReticuleControler = GameObject.Find("Reticule").GetComponent<ReticuleControler>();
        m_KiraanCamera = GameObject.Find("KiraanCamera").GetComponent<Cinemachine.CinemachineVirtualCamera>();

        m_Sky = GameObject.Find("Sky").GetComponent<Transform>();
        m_KiraanCamera.LookAt = m_Sky;
        m_KiraanCamera.Priority = 5;
    }

    public void SetupCameras(Camera main, Cinemachine.CinemachineVirtualCameraBase select, Cinemachine.CinemachineVirtualCameraBase freeLook, Cinemachine.CinemachineVirtualCamera piece)
    {
        m_MainCamera = main;
        m_SelectCamera = select;
        m_FreeLookCamera = freeLook;
        m_PieceCamera = piece;
        ChangeCamerasPriority(UsingCamera.FreeLook);
    }

    private void ChangeCamerasPriority(UsingCamera usingCamera)
    {
        m_SelectCamera.Priority = NonUsingPriority;
        m_FreeLookCamera.Priority = NonUsingPriority;
        m_PieceCamera.Priority = NonUsingPriority;
        switch (usingCamera)
        {
            case UsingCamera.Select:
                m_SelectCamera.Priority = UsingPriority;
                break;
            case UsingCamera.FreeLook:
                m_FreeLookCamera.Priority = UsingPriority;
                break;
            case UsingCamera.Piece:
                m_PieceCamera.Priority = UsingPriority;
                break;
            default:
                m_FreeLookCamera.Priority = UsingPriority;
                break;
        }
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
        m_Target = m_PiecesManager.CreatePiece(Team);
        m_PieceCamera.Follow = m_Target.transform;
        m_PieceCamera.LookAt = m_Target.transform;
        m_Target.Thrower = this;
        RemainingPieces--;

        // 操作可能にする
        IsPlayable = true;
        m_Phase = Phase.SquareSelect;

        // カメラを俯瞰視点にする
        ChangeCamerasPriority(UsingCamera.Select);
        //m_SelectCamera.Priority = UsingPriority;
        //m_PieceCamera.Priority = NonUsingPriority; // fixme : 相手のカメラのプライオリティが上がったままなので切り替わらない。修正する
        //m_FreeLookCamera.Priority = NonUsingPriority;

        // レティクルのアニメーションを選択中のものに変更
        Debug.Log("hoge");
        m_ReticuleControler.ChangeAnimation(GameState.Selecting);

        return true;
    }

    public string CurrentPhaseString()
    {
        return m_Phase.ToString();
    }

    private Vector3 CalcurateDirection(Vector3 mousePos) {
        var gap = mousePos - targetPosition;
        (gap.y, gap.z) = (0.0f, gap.y);
        // 座標変換でカメラの方向とズレの方向を調整
        gap = RotateThrowingVector(gap);

        gap /= MathF.Min(Screen.width, Screen.height);
        gap *= directionParam;
        // Debug.Log(gap);
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
        m_Reticule = GameObject.Find("Reticule").GetComponent<Transform>();
    }

    // チーム(自身のカメラ)に合わせて向きを調整
    public Vector3 RotateThrowingVector(Vector3 mouseDifference)
    {
        var roty = m_MainCamera.transform.rotation.eulerAngles.y;
        var quat = Quaternion.AngleAxis(roty, Vector3.up);
        return quat * mouseDifference;
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
                if (Physics.Raycast(ray, out var hit, rayLength, m_SquareLayerMask, QueryTriggerInteraction.Ignore)) // 人間依存
                {
                    var col = hit.collider;
                    if(col!= m_SquareCollider)
                    {
                        // 駒の位置変更
                        Vector3 pos = col.transform.position;
                        pos.y = PiecePositionY;
                        m_Target.transform.position = pos;
                        pos.y = 0.051f;
                        m_Reticule.position = pos;

                        m_SquareCollider = col;
                    }
                }

                //if (IPlayer~~.checkCanMoveTo~~())
                //{

                //    IPlayer~~.clear
                //}

                if (m_SquareCollider != null && Input.GetMouseButtonDown(0)) // 人間依存
                {
                    m_SquareCollider = null;

                    m_Phase = Phase.ButtonUpWait;

                    //m_PieceCamera.Priority = UsingPriority;
                    ChangeCamerasPriority(UsingCamera.Piece);
                    if(Team == Team.White)
                    {
                        var body = m_PieceCamera.GetCinemachineComponent<Cinemachine.CinemachineTransposer>();
                        body.m_FollowOffset.z = 5;
                    }
                }

                break;
            // 人間：マウス押し直し待ち
            // CPU : カメラ移動待ち
            case Phase.ButtonUpWait: // フェーズの名前が人間依存なので変えたほうがgood
                if (Input.GetMouseButtonDown(1))
                {
                    m_Phase = Phase.SquareSelect;
                    //m_PieceCamera.Priority = NonUsingPriority;
                    ChangeCamerasPriority(UsingCamera.Select);
                    break;
                }
                if (Input.GetMouseButtonDown(0))
                {
                    sumTime = 0.0f;
                    m_MouseHistory.Clear();
                    targetPosition = Input.mousePosition;
                    m_Phase = Phase.PieceThrow;
                    m_ReticuleControler.ChangeAnimation(GameState.WatingThrow);
                }
                break;
            // 投げるフェーズ
            case Phase.PieceThrow:
                if (Input.GetMouseButtonDown(1))
                {
                    m_Phase = Phase.SquareSelect;
                    //m_PieceCamera.Priority = NonUsingPriority;
                    ChangeCamerasPriority(UsingCamera.Piece);
                    m_ReticuleControler.ChangeAnimation(GameState.Selecting);
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
                    //m_PieceCamera.Priority = NonUsingPriority;
                    //m_FreeLookCamera.Priority = UsingPriority;
                    ChangeCamerasPriority(UsingCamera.FreeLook);
                    m_ReticuleControler.ChangeAnimation(GameState.Threw);
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

    public async UniTaskVoid LookUpSky()
    {
        m_KiraanCamera.Priority = 15;
        await UniTask.Delay(2000);
        m_KiraanCamera.Priority = 5;
        Debug.Log(m_KiraanCamera.LookAt);
    }
}
