using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

// TODO: Front Back����Ȃ���Whilte Black�ɂ���

public class FrontBackCounter : MonoBehaviour
{
    [SerializeField] private GameObject m_WhiteCounter;
    [SerializeField] private GameObject m_BlackCounter;
    [SerializeField] private GameObject m_Dropper;

    private (int, int) CountFrontBack()
    {
        var dropperTransform = m_Dropper.transform;
        var piecesNum = dropperTransform.childCount; 

        int whiteCount = 0;
        int blackCount = 0;

        // �G�ȕ��@�����A�Ƃ肠����Dropper�̂��ׂĂ̎q�I�u�W�F�N�g�ɂ��Ĕ������Ăяo��
        // �]�T������΂����ƌ������@�ɐ؂�ւ���
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
        // �o�J
        // �t���[�����ɔ����̖����𐔂��Ă���
        (int white, int black) countNum = CountFrontBack();

        m_WhiteCounter.GetComponent<Text>().text = countNum.white.ToString();
        m_BlackCounter.GetComponent<Text>().text = countNum.black.ToString();
    }
}
