using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultState : MonoBehaviour, IState
{
    [SerializeField]
    private GameStateManager gameStateManager;

    public ResultState(GameStateManager gameStateManager)
    {
        this.gameStateManager = gameStateManager;
    }

    public void Enter()
    {

    }
    // Update is called once per frame
    public void OnState()
    {

    }
    public void Exit()
    {

    }
}
