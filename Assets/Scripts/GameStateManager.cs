using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    [SerializeField]
    public StateMachine stateMachine { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        stateMachine = new StateMachine(this);
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Do();
    }
}
