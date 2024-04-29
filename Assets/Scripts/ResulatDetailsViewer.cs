using System.Collections;
using System.Collections.Generic;
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

    /*
    public void ViewResulatDetails()
    {
        m_ResultDetailsDisplay.SetActive(true);
        var history = m_TurnManager.GetGameHistory();

        int teamCount = System.Enum.GetNames(typeof(Team)).Length - 1; // Noneがあるので一つ減らす。 TODO : この計算度々出てくるので関数化した方が良さそう
        List<List<int>> teamHistory = new List<List<int>>(teamCount);
        for (int i = 0; i < teamCount; i++)
        {
            teamHistory.Add(new List<int>());
        }

        int maxPiece = 0;
        for (int i = 0; i < history.Count; i++)
        {
            // FrontBackCounterがタプルで返す都合、とりあえずこのような実装にしている
            // FIXME: 当然タプルではなくリストの方が扱いが良いので、FrontBackCounterの実装側からリストに変えていく
            teamHistory[0].Add(history[i].piecesNums.Item1);
            teamHistory[1].Add(history[i].piecesNums.Item2);

            maxPiece = Mathf.Max(maxPiece, history[i].piecesNums.Item1);
            maxPiece = Mathf.Max(maxPiece, history[i].piecesNums.Item2);
        }

        Color[] colorList = { Color.white, Color.black, Color.red };
        m_ResultGraph.Initialize(teamHistory[0].Count - 1, maxPiece);




        for (int teamIndex = 0; teamIndex < teamCount; teamIndex++)
        {
            Vector2? lastCircleAnchoredPosition = null;

            for (int i = 0; i < teamHistory[teamIndex].Count; i++)
            {
                var anchoredPosition = m_ResultGraph.CreateCircle(i, teamHistory[teamIndex][i], colorList[teamIndex]);
                if (lastCircleAnchoredPosition != null)
                {
                    m_ResultGraph.CreateDotConnection(lastCircleAnchoredPosition.Value, anchoredPosition, colorList[teamIndex]);
                }
                lastCircleAnchoredPosition = anchoredPosition;
            }
        }
    }
    */

    public void BackToMainResult()
    {
        m_ResultDetailsDisplay.SetActive(false);
    }
}
