using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class TitleUI : MonoBehaviour
{
    [SerializeField]
    private GameObject m_RaycastGuard = null;
    [SerializeField]
    private GameObject m_HowToPlayPanel = null;
    [SerializeField]
    private GameObject m_SettingsPanel = null;
    [SerializeField]
    private GameObject m_SelectModePanel = null;

    static readonly int HowToPageNum = 2;
    private int m_NowHowToPage = 1; // 1-index
    [SerializeField]
    private GameObject[] m_HowToPages = new GameObject[HowToPageNum];
    [SerializeField]
    private Button m_LeftArrow;
    [SerializeField]
    private Button m_RightArrow;

    private SettingManager setting;

    public void Start()
    {
        setting = FindObjectOfType<SettingManager>();
    }

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

    public void ShowSelectMode()
    {
        if (m_RaycastGuard == null || m_SelectModePanel == null)
        {
            return;
        }
        m_RaycastGuard.SetActive(true);
        m_SelectModePanel.SetActive(true);

        return;
    }

    public void HideSelectMode()
    {
        if (m_RaycastGuard == null || m_SelectModePanel == null)
        {
            return;
        }
        m_RaycastGuard.SetActive(false);
        m_SelectModePanel.SetActive(false);

        return;
    }

    public void ShowHowTo()
    {
        if (m_RaycastGuard == null|| m_HowToPlayPanel == null)
        {
            return;
        }

        // ページをアクティブにする前にボタンの状態を設定しておく
        // ページをアクティブにした後に設定してしまうと最初の数フレームだけボタンがアクティブになっている
        m_LeftArrow.interactable = false;
        m_RightArrow.interactable = (m_NowHowToPage == HowToPageNum) ? false : true;

        m_RaycastGuard.SetActive(true);
        m_HowToPlayPanel.SetActive(true);

        m_NowHowToPage = 1;
        for (int i = 0; i < HowToPageNum; i++)
        {
            if (m_NowHowToPage - 1 == i)
            {
                m_HowToPages[m_NowHowToPage - 1].SetActive(true);
            }
            else
            {
                m_HowToPages[i].SetActive(false);
            }
        }

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

    public void ChangeArrow()
    {

    }

    public void NextHowToPage()
    {
        if(m_NowHowToPage >= HowToPageNum)
        {
            return;
        }

        m_NowHowToPage++;
        m_HowToPages[m_NowHowToPage - 1].SetActive(false);
        m_HowToPages[m_NowHowToPage].SetActive(true);


    }

    public void ChangeHowToPage(int turnNum)
    {
        if(m_NowHowToPage + turnNum < 1 || HowToPageNum < m_NowHowToPage + turnNum)
        {
            return;
        }

        m_HowToPages[m_NowHowToPage - 1].SetActive(false);
        m_HowToPages[m_NowHowToPage + turnNum - 1].SetActive(true);

        m_LeftArrow.interactable = true;
        m_RightArrow.interactable = true;

        // ページが左端まで移動する場合
        if (m_NowHowToPage + turnNum == 1)
        {
            m_LeftArrow.interactable = false;
        }
        // 同様に右端まで移動する場合
        if (m_NowHowToPage + turnNum == HowToPageNum)
        {
            m_RightArrow.interactable = false;
        }

        m_NowHowToPage += turnNum;
    }

    public void PreviousHowToPage()
    {
        if(m_NowHowToPage <= 1)
        {
            return;
        }

        m_NowHowToPage--;
        m_HowToPages[m_NowHowToPage + 1].SetActive(false);
        m_HowToPages[m_NowHowToPage].SetActive(true);
    }

    public void SetGameMode(int settingId)
    {
        // settingId : 上位4ビットは人間の数、下位4ビットはCPUの数
        int cpu = settingId & 0b1111;
        int human = settingId >> 4;

        setting.SendMessage("SetGameMode", (human, cpu));
    }
}
