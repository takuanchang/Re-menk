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

        m_TurnText.text = $"Player{m_TurnManager.CurrentPlayer + 1}が操作中";
        m_PhaseText.text = "";
        for (int i = 0; i < m_Players.Count; i++)
        {
            string phase = (m_Players[i].IsPlayable ? m_Players[i].CurrentPhaseString() : "Not Playable");
            m_PhaseText.text += $"Player{i + 1} : {phase}, 残り : {m_Players[i].RemainingPieces}";
            if (i != m_Players.Count - 1) m_PhaseText.text += '\n';
        }
    }
}
