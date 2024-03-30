using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindowGraph : MonoBehaviour
{
    [SerializeField]
    private Sprite m_CircleSprite;
    private RectTransform m_GraphContainer;

    private Vector2 m_CircleRectSize = new Vector2(11, 11);
    private const float connectionWidth = 3.0f;

    private Color[] colorList = { Color.white, Color.black, Color.red };

    private void Awake()
    {
        m_GraphContainer = transform.Find("GraphContainer").GetComponent<RectTransform>();

        //List<int> valueList = new List<int>() { 5, 98, 56, 45, 30, 22, 17, 15, 13, 17, 25, 37, 40, 36, 33 };
        //ShowGraph(valueList);
        //valueList = new List<int>() { 8, 8, 35, 87, 25, 83, 11, 24, 63, 83, 52, 64, 66, 46, 21 };
        //ShowGraph(valueList);
    }

    private void CreateCircle(Vector2 anchoredPosition, int color)
    {
        GameObject gameObject = new GameObject("circle", typeof(Image));
        gameObject.transform.SetParent(m_GraphContainer, false);
        gameObject.GetComponent<Image>().sprite = m_CircleSprite;
        gameObject.GetComponent<Image>().color = colorList[color];

        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = m_CircleRectSize;
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.zero;
    }

    public void ShowGraph(List<int> valueList, int yMax, int mark)
    {
        Debug.Log(valueList);

        float graphHeight = m_GraphContainer.sizeDelta.y;
        float graphWidth = m_GraphContainer.sizeDelta.x;

        Vector2? lastCircleAnchoredPosition = null;

        // 要調整
        // float yMaximum = 64f;
        float xMaximum = valueList.Count - 1;

        for (int i = 0; i < valueList.Count; i++)
        {
            float xPosition = ((float)i / (float)xMaximum) * graphWidth;
            float yPosition = ((float)valueList[i] /  (float)yMax) * graphHeight;
            Vector2 anchoredPosition = new Vector2(xPosition, yPosition);
            CreateCircle(anchoredPosition, mark);
            if (lastCircleAnchoredPosition != null)
            {
                CreateDotConnection(lastCircleAnchoredPosition.Value, anchoredPosition, mark);
            }
            lastCircleAnchoredPosition = anchoredPosition;
        }
    }

    private void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB, int color)
    {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObject.transform.SetParent(m_GraphContainer, false);
        gameObject.GetComponent<Image>().color = colorList[color];

        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.sizeDelta = new Vector2(distance, connectionWidth);
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.zero;
        rectTransform.anchoredPosition = dotPositionA + dir * distance * .5f; 
        rectTransform.localEulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.right, dir));
    }
}
