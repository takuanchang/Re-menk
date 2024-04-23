using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// プレイヤーの固有番号, チーム, ターン数, ターン終了時の残りマス数
/// </summary>
struct HistoryData
{
    int player;
    Team team;
    int turn;
    int remaining;
    List<int> piecesNums;
}

public class GameHistory : MonoBehaviour
{
    private List<HistoryData> m_GameHistory;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
