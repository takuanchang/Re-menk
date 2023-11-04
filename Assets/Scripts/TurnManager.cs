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


    float timer = 0.0f;

    // Update is called once per frame
    void Update()
    {
        // 操作不可能→既に置いた、多分良くない実装
        if (!playerControllers[currentPlayer].IsPlayable)
        {
            timer += Time.deltaTime;
            if (timer >= 2.0f)
            {
                currentPlayer = (currentPlayer + 1) % numPlayer; // プレイヤーの入れ替え
                playerControllers[currentPlayer].PrepareNextPiece(); // 次のプレイヤーに準備させる
                timer = 0.0f;
            }
            return;
        }
    }
}
