using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReticuleControler : MonoBehaviour
{
    private Animator m_Animator;

    // Start is called before the first frame update
    void Start()
    {
        m_Animator = this.GetComponent<Animator>();
        m_Animator.SetInteger("gameState", ((int)GameState.Selecting));
    }

    public void ChangeAnimation(GameState state)
    {
        m_Animator.SetInteger("gameState", ((int)state));
    }
}
