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

        m_TurnText.text = "Black";
        m_PhaseText.text = "Black : SquareSelect, White : Not playable";
    }

    // Update is called once per frame
    void Update()
    {
        m_TurnText.text = m_TurnManager.CurrentPlayer == 0 ? "Black" : "White";

        string blackPhase = "Not playable";
        string whitePhase = "Not playable";
        if (m_Players[0].IsPlayable)
        {
            blackPhase = m_Players[0].CurrentPhaseString();
        }
        if (m_Players[1].IsPlayable)
        {
            whitePhase = m_Players[1].CurrentPhaseString();
        }

        m_PhaseText.text = $"Black : {blackPhase}, White : {whitePhase}";
    }
}
