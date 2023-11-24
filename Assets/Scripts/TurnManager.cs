using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class TurnManager : MonoBehaviour
{
    private const int numPlayer = 2;
    private int m_currentPlayer = 0;

    public int CurrentPlayer
    {
        get => m_currentPlayer;

        private set
        {
            Assert.IsTrue(0 <= value && value <= numPlayer, $"Range error : CurrentPlayer {value}");

            m_currentPlayer = value;
        }
    }

    [SerializeField]
    private PlayerController[] playerControllers = new PlayerController[numPlayer];

    [SerializeField]
    private GameObject m_ResultUI;
    [SerializeField]
    private Text m_ResultText;

    [SerializeField]
    private FrontBackCounter m_FrontBackCounter;

    public void InitializePlayer()
    {
        // リバーシは黒が先行
        CurrentPlayer = 0;
        playerControllers[CurrentPlayer].Team = Team.Black;
        playerControllers[CurrentPlayer].PrepareNextPiece();

        playerControllers[(CurrentPlayer + 1) % numPlayer].Team = Team.White;
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializePlayer();
    }


    private float timer = 0.0f;
    private bool isTimer = false;

    public void OnPieceThrown()
    {
        timer = 0.0f;
        isTimer = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTimer)
        {
            timer += Time.deltaTime;
            if (timer >= 2.0f)
            {
                PlayerChange();
                timer = 0.0f;
                isTimer = false;
            }
            return;
        }
    }

    void PlayerChange()
    {
        CurrentPlayer = (CurrentPlayer + 1) % numPlayer; // プレイヤーの入れ替え
        if (playerControllers[CurrentPlayer].PrepareNextPiece()) // 次のプレイヤーに準備させる
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

        (int white, int black) count = m_FrontBackCounter.CountFrontBack();


        string result = "";
        if(count.white < count.black)
        {
            result = "Black Win!";
        }
        else if(count.black < count.white)
        {
            result = "White Win!";
        }
        else
        {
            result = "Draw";
        }
        m_ResultText.text = result;
    }
}
