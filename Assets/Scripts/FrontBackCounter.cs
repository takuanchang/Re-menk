using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FrontBackCounter : MonoBehaviour
{
    [SerializeField]
    private Text m_WhiteCounter;

    [SerializeField]
    private Text m_BlackCounter;

    [SerializeField]
    private GameObject m_PiecesCollector;

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

    void Update()
    {
        // バカ
        // フレーム毎に白黒の枚数を数えている
        (int white, int black) countNum = CountFrontBack();

        m_WhiteCounter.text = countNum.white.ToString();
        m_BlackCounter.text = countNum.black.ToString();
    }
}
