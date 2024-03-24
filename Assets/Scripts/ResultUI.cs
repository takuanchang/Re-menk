using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultUI : MonoBehaviour
{
    public void ChangeToTitleScene()
    {
        SceneManager.LoadScene("Title");
    }
    public void Rematch()
    {
        _ = LoadMainScene();
    }
    private async UniTaskVoid LoadMainScene()
    {
        // 現在のSceneを取得
        Scene loadScene = SceneManager.GetActiveScene();
        // 現在のシーンを再読み込みする
        await SceneManager.LoadSceneAsync(loadScene.name);
    }
}
