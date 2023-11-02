using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUI : MonoBehaviour
{
    public void ChangeToGameScene()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void ShowHowTo()
    {
        return;
    }

    public void HideHowTo()
    {
        return;
    }

    public void OpenSettings()
    {
        // 音量
        // 画質
        // 見た目(スキン選べると楽しい)

        return;
    }

    public void CloseSettings()
    {
        return;
    }
}
