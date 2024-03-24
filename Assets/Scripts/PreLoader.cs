using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PreLoader : MonoBehaviour
{
    [SerializeField]
    private SettingManager settingManager;
    [SerializeField]
    private Image m_Image;
    [SerializeField]
    private GameObject m_LoadingTextObjeck;

    private float epsilon = 0.03f;
    private float a = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        _ = TransitionScene();
    }

    private async UniTaskVoid TransitionScene()
    {
        await SceneManager.LoadSceneAsync("Title", LoadSceneMode.Additive);
        // await UniTask.Delay(1000);
        m_LoadingTextObjeck.SetActive(false);
        while (m_Image.fillAmount >= epsilon)
        {
            await UniTask.NextFrame();
            m_Image.fillAmount -= a;
        }
        m_Image.fillAmount = 0;
        await SceneManager.UnloadSceneAsync("PreLoad");
    }

}
