using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using Cysharp.Threading.Tasks;
using static HumanPlayer;
using System.Linq;

public class TurnManager : MonoBehaviour
{
    private int m_currentPlayer = 0;
    private int m_TurnNum = 0;

    // 現状このフラグを使う必要がなくなっている
    // private bool m_isWaiting = false;
    private static readonly float MaxWait = 6.0f;
    private static readonly float Span = 0.5f;

    private List<IPlayer> m_Players;

    [SerializeField]
    private UiPrinter uiPrinter;

    [SerializeField]
    private GameObject m_ResultUI;

    [SerializeField]
    private GameObject m_ResulatDetailsUI;

    [SerializeField]
    private Text m_ResultText;

    [SerializeField]
    private FrontBackCounter m_FrontBackCounter;

    [SerializeField]
    private PlayerGenerator m_PlayerGenerator;

    [SerializeField]
    private PiecesManager m_PiecesManager;

    [SerializeField]
    private Board m_Board;

    private CancellationTokenSource m_CancellationTokenSource = null;

    private GameHistory m_GameHistory;
    public GameHistory GameHistory => m_GameHistory;

    /// <summary>
    /// このプレイヤーからスタートする
    /// </summary>
    const int StartPlayer = 0;


    public int CurrentPlayer
    {
        get => m_currentPlayer;

        private set
        {
            Assert.IsTrue(0 <= value && value <= m_Players.Count, $"Range error : CurrentPlayer {value}");
            m_currentPlayer = value;
        }
    }

    void PlayerChange()
    {
        // 全マス破壊されている場合
        if (m_Board.IsBrokenAll)
        {
            GoToResult();
            return;
        }

        for(int i = 0; i < m_Players.Count; i++)
        {
            if (m_Players[(CurrentPlayer + 1 + i) % m_Players.Count].PrepareNextPiece()) // 次のプレイヤーに準備させる
            {
                CurrentPlayer = (CurrentPlayer + 1 + i) % m_Players.Count; // プレイヤーの入れ替え
                return;
            }
        }
        // 全員駒の準備に失敗した場合
        GoToResult();
        return;
    }

    // TODO:マスを全破壊してしまった場合の処理をどうするか
    void GoToResult()
    {
        m_ResultUI.SetActive(true);

        //(int white, int black) count = m_FrontBackCounter.CountFrontBack();

        string result = "";

        // 全マス破壊時
        if (m_Board.IsBrokenAll)
        {
            // 最後に破壊した人が負け
            foreach (Team team in Enum.GetValues(typeof(Team)))
            {
                if(team == Team.None || team == m_Players[CurrentPlayer].Team)
                {
                    continue;
                }
                else
                {
                    Debug.Log(team);
                    result += $"{team} ";
                }
            }
            result += "Win!";
        }
        else
        {
            var piecesCounts = m_FrontBackCounter.PiecesCounts;
            var maxPiecesCount = piecesCounts.Max();
            var minPiecesCount = piecesCounts.Min();
            // 全チーム同点の場合
            if(maxPiecesCount == minPiecesCount)
            {
                result = "Draw";
            }
            // そうでない場合
            else
            {
                for (int i = 0; i < piecesCounts.Count(); i++)
                {
                    if (piecesCounts[i] == maxPiecesCount)
                    {
                        result += $"{(Team)i} ";
                    }
                }
                result += "Win!";
            }
        }

        //var pre_remaining = m_Board.GetBoardSize();
        //foreach (var x in m_GameHistory) {
        //    result += $"\nTurn {x.turn} Player{x.player} 破壊枚数 {pre_remaining - x.remaining}";
        //    pre_remaining = x.remaining;
        //}

        m_ResultText.text = result;
    }

    public void OnPieceThrown()
    {
        // m_isWaiting = true;
        _ = EndTurn();
    }

    public void ResetEndTurn()
    {
        if(m_CancellationTokenSource != null)
        {
            m_CancellationTokenSource.Cancel();
            m_CancellationTokenSource.Dispose();
            m_CancellationTokenSource = null;
        }
        _ = EndTurn();
    }

    private void StartFirstTurn()
    {
        // リバーシは黒が先行
        CurrentPlayer = StartPlayer;
        m_Players[CurrentPlayer].PrepareNextPiece();
    }

    void Start()
    {
        var setting = FindObjectOfType<SettingManager>();
        int humanNum = setting.HumanNum;
        int cpuNum = setting.ComputerNum;
        int teamNum = setting.TeamNum;

        m_FrontBackCounter.Initialize(teamNum);

        m_Board.InitializeBoard();
        m_Players = m_PlayerGenerator.GeneratePlayers(humanNum, cpuNum, teamNum);

        // 仕方ないがUIプリンターにプレイヤー情報を入れる
        // TODO 実装時は表示しない(消す)
        uiPrinter.Initialize(m_Players);

        m_TurnNum = 0; // ターン数を0に初期化

        m_GameHistory = new GameHistory();
        m_GameHistory.Initialize();
        // 0ターン目情報の追加
        HistoryData history = new HistoryData(-1, Team.None, m_TurnNum, m_Board.GetRemainingSquaresNum(), m_FrontBackCounter.PiecesCounts.ToList());
        m_GameHistory.UpdateHistory(history);

        m_TurnNum++;
        StartFirstTurn();
    }

    private async UniTaskVoid EndTurn()
    {
        m_CancellationTokenSource = new CancellationTokenSource();
        await EndTurnCore(m_CancellationTokenSource.Token);
        m_CancellationTokenSource.Dispose();
        m_CancellationTokenSource = null;
    }

    private async UniTask EndTurnCore(CancellationToken token)
    {
        float time = 0.0f;
        // 全ピースが止まるか待機時間がMaxWaitを超えると抜け出す
        while (!m_PiecesManager.IsStableAll() && time < MaxWait)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(Span), cancellationToken:token);
            time += Span;
        }
        if(time >= MaxWait)
        {
            // TODO: 止めても揺れはおさまらない為コライダーの変更か位置移動(滑り)必須？
            m_PiecesManager.StopPiecesMove();
            await UniTask.Delay(TimeSpan.FromSeconds(Span), cancellationToken: token);
        }

        // TODO: ここでFrontBack(名前なんだこれ)の更新をかける
        HistoryData history = new HistoryData(CurrentPlayer, m_Players[CurrentPlayer].Team, m_TurnNum, m_Board.GetRemainingSquaresNum(), m_FrontBackCounter.PiecesCounts.ToList());
        m_GameHistory.UpdateHistory(history);

        m_TurnNum++;

        PlayerChange();
        // m_isWaiting = false;
    }
}
