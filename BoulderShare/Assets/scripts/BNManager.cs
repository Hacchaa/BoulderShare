using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoulderNotes{
public class BNManager : MonoBehaviour
{
    void Awake(){
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60; 
        CanvasResolutionManager.Instance.InitHeights();
        BNGymDataCenter.Instance.Init();
        BNScreens.Instance.Init();
    }
}
}