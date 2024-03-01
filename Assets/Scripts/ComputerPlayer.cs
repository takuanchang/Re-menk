using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;

// CPU側の実装とする

[Serializable]
public class ComputerPlayer : MonoBehaviour , IPlayer
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
    public int RemainingPieces { get; private set; } = 8;

    [SerializeField]
    private Piece m_OriginalPiece;

    private Piece m_Target;

    private PiecesManager m_PiecesManager;
    private Board m_Board;

    [SerializeField]
    private Camera m_MainCamera;

    // オンライン・NPC対戦の場合は待機中にFreeLookCameraを使う
    // オフライン対戦の場合はDollyCameraを使う
    // Cinemachine.CinemachineVirtualCameraBaseにどちらかを代入して使う
    [SerializeField]
    private Cinemachine.CinemachineVirtualCameraBase m_FreeLookCamera;

    private Cinemachine.CinemachineVirtualCameraBase m_WaitTimeCamera;

    [SerializeField]
    private Cinemachine.CinemachineVirtualCamera m_PieceCamera;

    private string m_Phase = "SquareSelect";

    private Transform m_Reticule;
    private ReticuleControler m_ReticuleControler;

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

    /// <summary>
    /// 待機時間
    /// </summary>
    private static readonly float DelayTime = 1.0f;

    public string CurrentPhaseString()
    {
        return m_Phase;
    }

    private Vector3 targetPosition = Vector3.zero;

    [SerializeField]
    private float speedParam = 1.0f;

    private Vector3 CalcurateDirection()
    {
        return Vector3.zero; // とりあえずズレなし
    }

    public void Initialize(Team team, GameObject turnManager, PiecesManager piecesManager)
    {
        Team = team;
        m_TurnManager = turnManager;
        m_PiecesManager = piecesManager;

        m_Reticule = GameObject.Find("Reticule").GetComponent<Transform>();
        m_ReticuleControler = GameObject.Find("Reticule").GetComponent<ReticuleControler>();
    }

    public void RegisterBoard(Board board)
    {
        m_Board = board;
    }

    public void SetupCameras(Camera main, Cinemachine.CinemachineVirtualCameraBase waitTimeCamera, Cinemachine.CinemachineVirtualCamera piece)
    {
        m_MainCamera = main;
        m_WaitTimeCamera = waitTimeCamera;
        //m_FreeLookCamera = waitTimeCamera;
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
        m_Target = m_PiecesManager.CreatePiece(Team);

        RemainingPieces--;

        // 操作可能にする
        IsPlayable = true;

        // カメラを俯瞰視点にする
        m_PieceCamera.Priority = NonUsingPriority; // fixme : 相手のカメラのプライオリティが上がったままなので切り替わらない。修正する
        m_WaitTimeCamera.Priority = NonUsingPriority;

        _ = ExecuteTurn();

        return true;
    }

    private async UniTaskVoid ExecuteTurn()
    {
        // === マス選択 start ===
        m_Phase = "SquareSelect";
        m_ReticuleControler.ChangeAnimation(GameState.Selecting);

        await UniTask.Delay(TimeSpan.FromSeconds(DelayTime)); // カメラ移動待ち

        // TODO:マスが全破壊された場合の処理を考えるべき(勝敗条件など、Boardで破壊を通知された時等)
        IReadOnlyList<int> validSquareIndices = m_Board.ValidIndices;
        int selectedSquareId = validSquareIndices[UnityEngine.Random.Range(0, validSquareIndices.Count)];
        Vector3 pos = m_Board.GetSquarePosition(selectedSquareId);
        pos.y = PiecePositionY;
        m_Target.transform.position = pos;
        pos.y = 0.051f;
        m_Reticule.position = pos;

        await UniTask.Delay(TimeSpan.FromSeconds(DelayTime)); // 人間が目視できるように待ち時間を設定

        m_PieceCamera.Follow = m_Target.transform;
        m_PieceCamera.LookAt = m_Target.transform;
        m_PieceCamera.Priority = UsingPriority;
        if (Team == Team.White)
        {
            var body = m_PieceCamera.GetCinemachineComponent<Cinemachine.CinemachineTransposer>();
            body.m_FollowOffset.z = 5;
        }

        await UniTask.Delay(TimeSpan.FromSeconds(DelayTime)); // カメラ移動待ち
        m_ReticuleControler.ChangeAnimation(GameState.WatingThrow);

        // === マス選択 end ===

        // === コマ投げ start ===
        m_Phase = "PieceThrow";

        await UniTask.Delay(TimeSpan.FromSeconds(DelayTime)); // すぐ投げられるとびっくりするので待つ
        m_ReticuleControler.ChangeAnimation(GameState.Threw);

        m_PieceCamera.Priority = NonUsingPriority;
        m_WaitTimeCamera.Priority = UsingPriority;

        var dir = CalcurateDirection();
        dir.y = CalculateSpeed();
        Throw(dir);
        m_TurnManager.SendMessage("OnPieceThrown");

        IsPlayable = false;

        // === コマ投げ end ===

    }

    // 中身をCPU仕様に切り替える
    private float CalculateSpeed() {
        return speedParam;
    }

    // TODO : 実際はdir(direction)ではなく、大きさも含まれているので名前変える
    public void Throw(Vector3 dir) {
        Assert.IsNotNull(m_Target, "The piece trying to be thrown doesn't exist.");
        m_Target.Shoot(dir);
        m_Target = null;
        IsPlayable = false;
        return;
    }

    // ------------------------------------------------------------------------------------------
}
