using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{
    public int frameRate = 30;

    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = frameRate;
    }
}
