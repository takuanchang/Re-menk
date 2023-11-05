using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteadyState : MonoBehaviour, IState
{
    [SerializeField]
    private GameStateManager gameStateManager;

    private static int max_num = 64;
    private int current_num = 0;
    private Rigidbody[] rbs = new Rigidbody[max_num];

    public SteadyState(GameStateManager gameStateManager)
    {
        this.gameStateManager = gameStateManager;
    }

    // 定常状態かどうか全探索
    private bool IsSteady()
    {
        bool steady = true;
        foreach(var p in rbs)
        {
            if (p == null)
            {
                continue;
            }
            steady &= p.IsSleeping();
        }
        return steady;
    }

    public void SetRigidbody(Rigidbody rb)
    {
        rbs[current_num++] = rb;
    }

    public void Enter()
    {

    }
    
    public void OnState()
    {
        if (IsSteady())
        {
            if (current_num == max_num)
            {
                gameStateManager.stateMachine.TransitionTo(gameStateManager.stateMachine.resultState);
            }
            else
            {
                gameStateManager.stateMachine.TransitionTo(gameStateManager.stateMachine.playingState);
            }
        }
    }
    public void Exit()
    {

    }
}
