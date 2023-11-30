using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiPrinter : MonoBehaviour
{
    [SerializeField] private Text m_phaseText;
    [SerializeField] private Text m_turnText;
    [SerializeField] private TurnManager m_turnManager;
    [SerializeField] private PlayerController[] m_playerController = new PlayerController[2]; // とりあえず数字そのまま放り込む

    void Start()
    {
        m_turnText.text = "Black";
        m_phaseText.text = "Black : SquareSelect, White : Not playable";
    }

    // Update is called once per frame
    void Update()
    {
        m_turnText.text = m_turnManager.CurrentPlayer == 0 ? "Black" : "White";

        string blackPhase = "Not playable";
        string whitePhase = "Not playable";
        if (m_playerController[0].IsPlayable)
        {
            blackPhase = m_playerController[0].CurrentPhase.ToString();
        }
        if (m_playerController[1].IsPlayable)
        {
            whitePhase = m_playerController[1].CurrentPhase.ToString();
        }

        m_phaseText.text = $"Black : {blackPhase}, White : {whitePhase}";
    }
}
