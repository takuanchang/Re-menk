using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

// TODO: Front BackじゃなくてWhilte Blackにする

public class FrontBackCounter : MonoBehaviour
{
    [SerializeField]
    private Text m_WhiteCounter;

    [SerializeField]
    private Text m_BlackCounter;

    [SerializeField]
    private GameObject m_Dropper;

    private (int, int) CountFrontBack()
    {
        var dropperTransform = m_Dropper.transform;
        var piecesNum = dropperTransform.childCount;

        int whiteCount = 0;
        int blackCount = 0;

        // 雑な方法だが、とりあえずDropperのすべての子オブジェクトについて白黒を呼び出す
        // 余裕があればもっと賢い方法に切り替える
        for (int i = 0; i < piecesNum; i++)
        {
            switch (dropperTransform.GetChild(i).GetComponent<Piece>().GetStatus())
            {
                case Piece.Status.Back:
                    whiteCount++;
                    break;

                case Piece.Status.Front:
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
