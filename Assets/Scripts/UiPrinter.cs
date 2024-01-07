using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiPrinter : MonoBehaviour
{
    [SerializeField] private Text m_PhaseText;
    [SerializeField] private Text m_TurnText;
    [SerializeField] private TurnManager m_TurnManager;
    private List<IPlayer> m_Players = null;

    void Start()
    {
        m_TurnText.text = "You";
        m_PhaseText.text = "You : SquareSelect, Other : Not playable";
    }

    public void Initialize(List<IPlayer> Players)
    {
        m_Players = Players;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Players == null)
        {
            return;
        }

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
