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
    public int RemainingPieces { get; private set; } = 32;

    [SerializeField]
    private Piece m_OriginalPiece;

    private Piece m_Target;

    [SerializeField]
    private GameObject m_PiecesCollector;

    [SerializeField]
    private Camera m_MainCamera;

    // オンライン・NPC対戦の場合は待機中にFreeLookCameraを使う
    // オフライン対戦の場合はDollyCameraを使う
    // Cinemachine.CinemachineVirtualCameraBaseにどちらかを代入して使う
    [SerializeField]
    private Cinemachine.CinemachineFreeLook m_FreeLookCamera;

    private Cinemachine.CinemachineVirtualCameraBase m_WaitTimeCamera;

    [SerializeField]
    private Cinemachine.CinemachineVirtualCamera m_PieceCamera;

    private string m_Phase = "SquareSelect";

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

    public void Initialize(Team team, GameObject turnManager)
    {
        Team = team;
        m_TurnManager = turnManager;
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

        // カメラを俯瞰視点にする
        m_PieceCamera.Priority = 9; // fixme : 相手のカメラのプライオリティが上がったままなので切り替わらない。修正する
        m_FreeLookCamera.Priority = 8;

        _ = ExecuteTurn();

        return true;
    }

    private async UniTaskVoid ExecuteTurn()
    {
        // === マス選択 start ===
        m_Phase = "SquareSelect";

        await UniTask.Delay(TimeSpan.FromSeconds(1.0f)); // カメラ移動待ち

        const float positionAdjustmentValue = 0.5f;
        float posX = UnityEngine.Random.Range(-4, 4) + positionAdjustmentValue;
        float posZ = UnityEngine.Random.Range(-4, 4) + positionAdjustmentValue;
        Vector3 pos = new Vector3(posX, 3.0f, posZ);
        m_Target.transform.position = pos;

        await UniTask.Delay(TimeSpan.FromSeconds(1.0f)); // 人間が目視できるように待ち時間を設定

        m_PieceCamera.Follow = m_Target.transform;
        m_PieceCamera.LookAt = m_Target.transform;
        m_PieceCamera.Priority = 11;

        await UniTask.Delay(TimeSpan.FromSeconds(1.0f)); // カメラ移動待ち

        // === マス選択 end ===

        // === コマ投げ start ===
        m_Phase = "PieceThrow";

        await UniTask.Delay(TimeSpan.FromSeconds(1.0f)); // すぐ投げられるとびっくりするので待つ

        m_PieceCamera.Priority = 9;
        m_FreeLookCamera.Priority = 11;

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
    public void Start()
    {
        
    }
}
