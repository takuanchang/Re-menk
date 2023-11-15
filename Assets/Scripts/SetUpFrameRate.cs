using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetUpFrameRate : MonoBehaviour
{
    private void Start()
    {
        Application.targetFrameRate = 120;
    }
}
