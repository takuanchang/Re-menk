using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Assertions;
using Cysharp.Threading.Tasks;

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
    [SerializeField]
    private GameObject m_CustomModePanel = null;

    static readonly int HowToPageNum = 2;
    private int m_NowHowToPage = 1; // 1-index
    [SerializeField]
    private GameObject[] m_HowToPages = new GameObject[HowToPageNum];
    [SerializeField]
    private Button m_LeftArrow;
    [SerializeField]
    private Button m_RightArrow;

    private SettingManager settingManager;

    /// <summary>
    /// 読み込むゲームシーン名
    /// </summary>
    const string GameSceneName = "Main";


    // TODO:以下わざわざ持っとくべきかは要議論
    [SerializeField]
    private Slider m_HumanSlider;
    [SerializeField]
    private Slider m_ComputerSlider;
    [SerializeField]
    private Slider m_TeamSlider;
    [SerializeField]
    private Button m_CustomStartButton;
    [SerializeField]
    private Text m_HumanNumText;
    [SerializeField]
    private Text m_ComputerNumText;
    [SerializeField]
    private Text m_TeamNumText;
    [SerializeField]
    private Text m_CustomAlertText;

    private Setting m_Setting = new(2, 0, 2);

    public void Start()
    {
        settingManager = FindObjectOfType<SettingManager>();
    }

    public void ChangeToGameScene()
    {
        SceneManager.LoadScene(GameSceneName);
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

    public void ShowCustomMode()
    {
        if (m_RaycastGuard == null || m_CustomModePanel == null)
        {
            return;
        }
        // SelectModeから
        m_CustomModePanel.SetActive(true);
        m_SelectModePanel.SetActive(false);

        return;
    }

    public void HideCustomMode()
    {
        if (m_RaycastGuard == null || m_CustomModePanel == null)
        {
            return;
        }
        // SelectModeに戻る
        m_CustomModePanel.SetActive(false);
        m_SelectModePanel.SetActive(true);

        return;
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


    //------------------------------------------------------------------------
    // 以下別にクラスを分けるべきと思われる
    public void SetGameMode(Setting set)
    {
        settingManager.SendMessage("SetGameMode", set);
    }

    public void ChangeSettingNum()
    {
        m_Setting.humanNum = (int)m_HumanSlider.value;
        m_Setting.computerNum = (int)m_ComputerSlider.value;
        m_Setting.teamNum = (int)m_TeamSlider.value;

        m_HumanNumText.text = $"{m_Setting.humanNum}";
        m_ComputerNumText.text = $"{m_Setting.computerNum}";
        m_TeamNumText.text = $"{m_Setting.teamNum}";

        // startボタンの処理(ここに書くべきか)
        if(m_Setting.PlayersNum < 2 || m_Setting.PlayersNum > 16)
        {
            m_CustomAlertText.enabled = true;
            m_CustomStartButton.interactable = false;
        }
        else
        {
            m_CustomAlertText.enabled = false;
            m_CustomStartButton.interactable = true;
        }
    }

    // TODO:可能なら一人用、二人用の設定はSetting側に用意したい
    public void SetSingleMode()
    {
        SetGameMode(Setting.Single);
    }
    public void SetDoubleMode()
    {
        SetGameMode(Setting.Double);
    }
    public void SetCustomMode()
    {
        SetGameMode(m_Setting);
    }
}
