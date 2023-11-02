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
        // エディタ上で動作させる時とビルド時に動作させる時で停止命令が異なる
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
