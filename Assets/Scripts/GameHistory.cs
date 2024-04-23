using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// プレイヤーの固有番号, チーム, ターン数, ターン終了時の残りマス数
/// </summary>
public struct HistoryData
{
    int player;
    Team team;
    int turn;
    int remaining;
    List<int> piecesNums;

    public HistoryData(int player, Team team, int turn, int remaining, List<int> piecesNums)
    {
        this.player = player;
        this.team = team;
        this.turn = turn;
        this.remaining = remaining;
        this.piecesNums = piecesNums;
    }
}

public class GameHistory : MonoBehaviour
{
    private List<HistoryData> m_GameHistory;
    public IEnumerable<HistoryData> History => m_GameHistory;

    public void Initialize()
    {
        m_GameHistory = new List<HistoryData>();
    }

    public void UpdateHistory(HistoryData historyDate)
    {
        // TODO: ここでFrontBack(名前なんだこれ)の更新をかける
        m_GameHistory.Add(historyDate);
    }
}
