using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ResulatDetailsViewer : MonoBehaviour
{
    [SerializeField]
    private TurnManager m_TurnManager;

    [SerializeField]
    private GameObject m_ResultDetailsDisplay;

    [SerializeField]
    private WindowGraph m_ResultGraph;

    private Color[] m_ColorList = { Color.black, Color.white, Color.blue }; // TODO:要調整

    private bool m_IsGenerated = false;
    void GenerateGraph()
    {
        var history = m_TurnManager.GameHistory.History;

        int teamCount = history[0].piecesNums.Count; // TODO:取り方変えるべきかも(settingから取る等)

        int maxPiece = 0;
        foreach (var data in history)
        {
            maxPiece = Mathf.Max(maxPiece, data.piecesNums.Max());
        }

        m_ResultGraph.Initialize(history.Count - 1, maxPiece); // グラフ表示が端から端になるよう-1をしている

        for (int teamIndex = 0; teamIndex < teamCount; teamIndex++)
        {
            Vector2? lastCircleAnchoredPosition = null;

            for (int turn = 0; turn < history.Count; turn++)
            {
                var anchoredPosition = m_ResultGraph.CreateCircle(turn, history[turn].piecesNums[teamIndex], m_ColorList[teamIndex]);
                if (lastCircleAnchoredPosition != null)
                {
                    m_ResultGraph.CreateDotConnection(lastCircleAnchoredPosition.Value, anchoredPosition, m_ColorList[teamIndex]);
                }
                lastCircleAnchoredPosition = anchoredPosition;
            }
        }
        m_IsGenerated = true;
    }

    public void ViewResulatDetails()
    {
        if (!m_IsGenerated)
        {
            GenerateGraph();
        }
        m_ResultDetailsDisplay.SetActive(true);
    }

    public void BackToMainResult()
    {
        m_ResultDetailsDisplay.SetActive(false);
    }
}
