using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetUpFrameRate : MonoBehaviour
{
    /// <summary>
    /// 使用するフレームレート
    /// </summary>
    const int FrameRate = 120;

    private void Start()
    {
        Application.targetFrameRate = FrameRate;
    }
}
