using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using Cysharp.Threading.Tasks;
using static HumanPlayer;

public class TurnManager : MonoBehaviour
{
    private int m_currentPlayer = 0;

    // 現状このフラグを使う必要がなくなっている
    // private bool m_isWaiting = false;
    private static readonly float MaxWait = 2.0f;
    private static readonly float Span = 1.0f;

    private List<IPlayer> m_Players;

    [SerializeField]
    private UiPrinter uiPrinter;

    [SerializeField]
    private GameObject m_ResultUI;

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
        CurrentPlayer = (CurrentPlayer + 1) % m_Players.Count; // プレイヤーの入れ替え
        if (m_Players[CurrentPlayer].PrepareNextPiece()) // 次のプレイヤーに準備させる
        {
            return;
        }
        else
        {
            GoToResult();
            return;
        }
    }

    // TODO:マスを全破壊してしまった場合の処理をどうするか
    void GoToResult()
    {
        m_ResultUI.SetActive(true);

        //(int white, int black) count = m_FrontBackCounter.CountFrontBack();
        var (white, black) = m_FrontBackCounter.CountFrontBack();


        string result = "";
        if(white < black)
        {
            result = "Black Win!";
        }
        else if(black < white)
        {
            result = "White Win!";
        }
        else
        {
            result = "Draw";
        }
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

    void Start()
    {
        var setting = FindObjectOfType<SettingManager>();
        int humanNum = setting.HumanNum;
        int cpuNum = setting.ComputerNum;

        m_Board.InitializeBoard();
        m_Players = m_PlayerGenerator.GeneratePlayers(humanNum, cpuNum);

        // リバーシは黒が先行
        CurrentPlayer = StartPlayer;
        m_Players[CurrentPlayer].PrepareNextPiece();

        // 仕方ないがUIプリンターにプレイヤー情報を入れる
        // TODO 実装時は表示しない(消す)
        uiPrinter.Initialize(m_Players);
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
        PlayerChange();
        // m_isWaiting = false;
    }
}
