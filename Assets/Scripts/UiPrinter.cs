using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiPrinter : MonoBehaviour
{
    [SerializeField] private Text m_PhaseText;
    [SerializeField] private Text m_TurnText;
    [SerializeField] private TurnManager m_TurnManager;
    [SerializeField] private GameObject[] m_PlayerObjects = new GameObject[2];
    private IPlayer[] m_Players = new IPlayer[2]; // とりあえず数字そのまま放り込む

    void Start()
    {
        m_Players[0] = m_PlayerObjects[0].GetComponent<IPlayer>();
        m_Players[1] = m_PlayerObjects[1].GetComponent<IPlayer>();

        m_TurnText.text = "You";
        m_PhaseText.text = "You : SquareSelect, Other : Not playable";
    }

    // Update is called once per frame
    void Update()
    {
        m_TurnText.text = m_TurnManager.CurrentPlayer == 0 ? "You" : "Other";

        string yourPhase = "Not playable";
        string othersPhase = "Not playable";
        if (m_Players[0].IsPlayable)
        {
            yourPhase = m_Players[0].CurrentPhaseString();
        }
        if (m_Players[1].IsPlayable)
        {
            othersPhase = m_Players[1].CurrentPhaseString();
        }

        m_PhaseText.text = $"You : {yourPhase}, Other : {othersPhase}";
    }
}
