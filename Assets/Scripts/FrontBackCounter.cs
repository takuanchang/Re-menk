using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;


public class FrontBackCounter : MonoBehaviour
{
    [SerializeField]
    private Text m_WhiteCounter;

    [SerializeField]
    private Text m_BlackCounter;

    [SerializeField]
    private GameObject m_PiecesCollector;

    private List<int> m_PiecesCounts;

    public IReadOnlyList<int> PiecesCounts => m_PiecesCounts.AsReadOnly();

    public void Initialize(int teamNum)
    {
        m_PiecesCounts = new List<int>(teamNum);
        for (int i = 0; i < teamNum; ++i)
        {
            m_PiecesCounts.Add(0);
        }
    }

    public void UpdatePiecesCounts()
    {
        var collectorTransform = m_PiecesCollector.transform;
        var piecesNum = collectorTransform.childCount;

        // 枚数数えなおし
        for (int i = 0; i < m_PiecesCounts.Count; i++)
        {
            m_PiecesCounts[i] = 0;
        }
        for (int i = 0; i < piecesNum; i++)
        {
            var child = collectorTransform.GetChild(i);
            if (!child.TryGetComponent<Piece>(out var piece))
            {
                continue;
            }

            // 確実にチームが更新される保証があるなら不要だが、念のためチームを更新
            piece.UpdateTeam();

            if(piece.Team == Team.None)
            {
                continue;
            }
            m_PiecesCounts[(int)piece.Team]++;
        }
        // m_PiecesCounts[(int)Team.Test] = 2;
    }


    /*
    public (int, int) CountFrontBack()
    {
        var collectorTransform = m_PiecesCollector.transform;
        var piecesNum = collectorTransform.childCount;

        int whiteCount = 0;
        int blackCount = 0;

        // 雑な方法だが、とりあえずDropperのすべての子オブジェクトについて白黒を呼び出す
        // 余裕があればもっと賢い方法に切り替える
        for (int i = 0; i < piecesNum; i++)
        {
            var child = collectorTransform.GetChild(i);
            if (!child.TryGetComponent<Piece>(out var piece)) {
                continue;
            }

            // 確実にチームが更新される保証があるなら不要だが、念のためチームを更新
            piece.UpdateTeam();

            switch (piece.Team)
            {
                case Team.White:
                    whiteCount++;
                    break;

                case Team.Black:
                    blackCount++;
                    break;

                default:
                    break;
            }
        }

        return (whiteCount, blackCount);
    }
    */

    void Update()
    {
        // フレーム毎に白黒の枚数を数えている
        //var (white, black) = CountFrontBack();
        UpdatePiecesCounts();

        m_BlackCounter.text = m_PiecesCounts[(int)(Team.Black)].ToString();
        m_WhiteCounter.text = m_PiecesCounts[(int)(Team.White)].ToString();
    }
}
