using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private const int numPlayer = 2;
    private int currentPlayer;

    [SerializeField]
    private PlayerController[] playerControllers = new PlayerController[numPlayer];

    public void InitializePlayer()
    {
        // リバーシは黒が先行
        currentPlayer = 0;
        playerControllers[currentPlayer].Team = Team.Black;
        playerControllers[currentPlayer].PrepareNextPiece();

        playerControllers[(currentPlayer + 1) % numPlayer].Team = Team.White;
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
        currentPlayer = (currentPlayer + 1) % numPlayer; // プレイヤーの入れ替え
        if (playerControllers[currentPlayer].PrepareNextPiece()) // 次のプレイヤーに準備させる
        {
            return;
        }
        else
        {
            // リザルト移行関数を記述

            return;
        }
    }
}
