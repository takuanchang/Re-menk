using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDisplay : MonoBehaviour
{
    [SerializeField] GameObject m_DebugDisplay;
    private bool m_CanSetDisplay = false;

    void Update()
    {
        bool haveDownedAllKeys = Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.D);
        if (haveDownedAllKeys && !m_CanSetDisplay)
        {
            m_CanSetDisplay = true;
            m_DebugDisplay.SetActive(!m_DebugDisplay.activeSelf);
        }
        else if(!haveDownedAllKeys && m_CanSetDisplay)
        {
            m_CanSetDisplay = false;
        }
    }
}
