using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public IState currentState;
    public PlayingState playingState;
    public SteadyState steadyState;
    public ResultState resultState;
    // Start is called before the first frame update
    public StateMachine(GameStateManager gameStateManager)
    {
        // create an instance for each state and pass in PlayerController
        this.playingState = new PlayingState(gameStateManager);
        this.steadyState = new SteadyState(gameStateManager);
        this.resultState = new ResultState(gameStateManager);
        currentState = this.playingState;
    }

    void Start()
    {

    }

    // Update is called once per frame
    public void Do()
    {
        currentState.OnState();
    }

    public void TransitionTo(IState nextState)
    {
        currentState.Exit();
        nextState.Enter();
        currentState = nextState;
    }
}
