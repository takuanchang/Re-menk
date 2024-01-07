using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class TurnManager : MonoBehaviour
{
    private int m_currentPlayer = 0;

    private float m_timer = 0.0f;
    private bool m_isWaiting = false;
    private static readonly float MinWait = 2.0f;

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
        m_timer = 0.0f;
        m_isWaiting = true;
    }

    void Start()
    {
        var setting = FindObjectOfType<SettingManager>();
        int humanNum = setting.HumanNum;
        int cpuNum = setting.ComputerNum;

        m_Players = m_PlayerGenerator.GeneratePlayers(humanNum, cpuNum);

        // リバーシは黒が先行
        CurrentPlayer = 0;
        m_Players[CurrentPlayer].PrepareNextPiece();

        // 仕方ないがUIプリンターにプレイヤー情報を入れる
    }

    void Update()
    {
        if (m_isWaiting)
        {
            m_timer += Time.deltaTime;
            if (m_timer >= MinWait)
            {
                PlayerChange();
                m_timer = 0.0f;
                m_isWaiting = false;
            }
            return;
        }
    }
}
