using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayingState : MonoBehaviour, IState
{
    [SerializeField]
    private GameStateManager gameStateManager;

    public PlayingState(GameStateManager gameStateManager)
    {
        this.gameStateManager = gameStateManager;
    }

    public void Enter()
    {

    }
    // Update is called once per frame
    public void OnState()
    {
        if (true)
        {
            gameStateManager.stateMachine.TransitionTo(gameStateManager.stateMachine.steadyState);
        }
    }
    public void Exit()
    {
        
    }
}
