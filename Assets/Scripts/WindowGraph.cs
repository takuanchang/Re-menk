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

    private float ratioX = 1f;
    private float ratioY = 1f;

    private void Awake()
    {
        m_GraphContainer = transform.Find("GraphContainer").GetComponent<RectTransform>();

        //List<int> valueList = new List<int>() { 5, 98, 56, 45, 30, 22, 17, 15, 13, 17, 25, 37, 40, 36, 33 };
        //ShowGraph(valueList);
        //valueList = new List<int>() { 8, 8, 35, 87, 25, 83, 11, 24, 63, 83, 52, 64, 66, 46, 21 };
        //ShowGraph(valueList);
    }

    public void Initialize(int countX, int countY)
    {
        float graphWidth = m_GraphContainer.sizeDelta.x;
        float graphHeight = m_GraphContainer.sizeDelta.y;

        ratioX = graphWidth / countX;
        ratioY = graphHeight / countY;
    }

    public Vector2 CreateCircle(int x, int y, Color color)
    {
        var circlePosition = new Vector2(x * ratioX, y * ratioY);
        GameObject gameObject = new GameObject("circle", typeof(Image));
        gameObject.transform.SetParent(m_GraphContainer, false);
        gameObject.GetComponent<Image>().sprite = m_CircleSprite;
        gameObject.GetComponent<Image>().color = color;

        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = circlePosition;
        rectTransform.sizeDelta = m_CircleRectSize;
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.zero;
        return circlePosition;
    }

    /*
    public void ShowGraph(List<int> valueList, int yMax, int mark)
    {
        Debug.Log(valueList);

        Initialize(valueList.Count - 1, yMax);
        Vector2? lastCircleAnchoredPosition = null;


        for (int i = 0; i < valueList.Count; i++)
        {
            var anchoredPosition = CreateCircle(i, valueList[i], colorList[mark]);
            if (lastCircleAnchoredPosition != null)
            {
                CreateDotConnection(lastCircleAnchoredPosition.Value, anchoredPosition, colorList[mark]);
            }
            lastCircleAnchoredPosition = anchoredPosition;
        }
    }
    */

    public void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB, Color color)
    {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObject.transform.SetParent(m_GraphContainer, false);
        gameObject.GetComponent<Image>().color = color;

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
