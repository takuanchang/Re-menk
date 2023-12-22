using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class TurnManager : MonoBehaviour
{
    private const int NUM_PLAYER = 2;
    private int m_currentPlayer = 0;

    private float m_timer = 0.0f;
    private bool m_isWaiting = false;
    private static readonly float MinWait = 2.0f;

    [SerializeField]
    private GameObject[] m_PlayerObjects = new GameObject[NUM_PLAYER];
    private IPlayer[] m_Players = new IPlayer[NUM_PLAYER];

    [SerializeField]
    private GameObject m_ResultUI;

    [SerializeField]
    private Text m_ResultText;

    [SerializeField]
    private FrontBackCounter m_FrontBackCounter;

    public int CurrentPlayer
    {
        get => m_currentPlayer;

        private set
        {
            Assert.IsTrue(0 <= value && value <= NUM_PLAYER, $"Range error : CurrentPlayer {value}");
            m_currentPlayer = value;
        }
    }

    public void InitializePlayer()
    {
        m_Players[0] = m_PlayerObjects[0].GetComponent<IPlayer>();
        m_Players[1] = m_PlayerObjects[1].GetComponent<IPlayer>();

        m_Players[0].Initialize(Team.Black, this.gameObject);
        m_Players[1].Initialize(Team.White, this.gameObject);

        // リバーシは黒が先行
        CurrentPlayer = 0;
        m_Players[CurrentPlayer].PrepareNextPiece();
    }

    void PlayerChange()
    {
        CurrentPlayer = (CurrentPlayer + 1) % NUM_PLAYER; // プレイヤーの入れ替え
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
        InitializePlayer();
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
