using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System;

namespace BoulderNotes{
public class BNManager : SingletonMonoBehaviour<BNManager>
{
    [SerializeField] private AssetReference[] cornerPanelFill;
    [SerializeField] private AssetReference[] cornerPanelStroke;
    private Sprite[] cornerPanelFillSprites;
    private Sprite[] cornerPanelStrokeSprites;
    protected override void Awake(){
        base.Awake();
        cornerPanelFillSprites = new Sprite[cornerPanelFill.Length];
        cornerPanelStrokeSprites = new Sprite[cornerPanelStroke.Length];
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60; 

        StartCoroutine(Setup());

        //Addressables.LoadAssetsAsync<Sprite>(cornerPanelFill[0],OnLoadCornerPanel);
    }

    IEnumerator Setup(){
        CanvasResolutionManager.Instance.Init();
        BNGymDataCenter.Instance.Init();
        yield return null;
        BNScreens.Instance.Init();  
    }
/*
    private void OnLoadCornerPanel(Sprite spr){
        cornerPanelFillSprites[index] = spr;
        index++;
        if(index < cornerPanelFill.Length){
            Addressables.LoadAssetsAsync<Sprite>(cornerPanelFill[index],OnLoadCornerPanel);
        }
    }*/

    public void GetCornerPanelFill(Action<Sprite> loadAction){
        int retina = (int)CanvasResolutionManager.Instance.GetRatioOfPtToPx() - 1;
        if (retina < 0 || retina > cornerPanelFill.Length - 1){
            if(loadAction != null){
                loadAction(null);
            }
        }
        if (cornerPanelFillSprites[retina] == null){
            Addressables.LoadAssetsAsync<Sprite>(cornerPanelFill[retina], loadAction);
        }else{
            loadAction(cornerPanelFillSprites[retina]);
        }
    }
    public void GetCornerPanelStroke(Action<Sprite> loadAction){
        int retina = (int)CanvasResolutionManager.Instance.GetRatioOfPtToPx() - 1;
        if (retina < 0 || retina > cornerPanelStroke.Length - 1){
            if(loadAction != null){
                loadAction(null);
            }
        }
        if (cornerPanelStrokeSprites[retina] == null){
            Addressables.LoadAssetsAsync<Sprite>(cornerPanelStroke[retina], loadAction);
        }else{
            loadAction(cornerPanelStrokeSprites[retina]);
        }
    }
}
}