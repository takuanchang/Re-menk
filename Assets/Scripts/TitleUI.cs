using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class TitleUI : MonoBehaviour
{
    [SerializeField]
    private GameObject m_RaycastGuard = null;
    [SerializeField]
    private GameObject m_HowToPlayPanel = null;
    [SerializeField]
    private GameObject m_SettingsPanel = null;

    public void ChangeToGameScene()
    {
        SceneManager.LoadScene("Main");
    }

    public void ExitGame()
    {
        // エディタ上で動作させる時とビルド時に動作させる時で停止命令が異なる
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void ShowHowTo()
    {
        if (m_RaycastGuard == null|| m_HowToPlayPanel == null)
        {
            return;
        }
        m_RaycastGuard.SetActive(true);
        m_HowToPlayPanel.SetActive(true);
        return;
    }

    public void HideHowTo()
    {
        if (m_RaycastGuard == null || m_HowToPlayPanel == null)
        {
            return;
        }
        m_RaycastGuard.SetActive(false);
        m_HowToPlayPanel.SetActive(false);
        return;
    }

    public void ShowSettings()
    {
        if (m_RaycastGuard == null || m_SettingsPanel == null)
        {
            return;
        }
        m_RaycastGuard.SetActive(true);
        m_SettingsPanel.SetActive(true);
        // 音量
        // 画質
        // 見た目(スキン選べると楽しい)
        return;
    }

    public void HideSettings()
    {
        if (m_RaycastGuard == null || m_SettingsPanel == null)
        {
            return;
        }
        m_RaycastGuard.SetActive(false);
        m_SettingsPanel.SetActive(false);
        return;
    }
}
