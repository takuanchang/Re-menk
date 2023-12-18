using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerController
{
    /// <summary>
    /// このプレイヤーのチーム
    /// </summary>
    public Team Team { get; }

    /// <summary>
    /// このプレイヤーが操作可能かどうか
    /// </summary>
    public bool IsPlayable { get; } // setは実装

    /// <summary>
    /// このプレイヤーに残っている駒の数
    /// 但し操作中の駒はカウントされない
    /// </summary>
    public int RemainingPieces { get; } // setは実装

    public void Initialize(Team team, GameObject turnManager);

    /// <summary>
    /// 次の駒を用意してから操作可能にする
    /// </summary>
    public bool PrepareNextPiece();
}
