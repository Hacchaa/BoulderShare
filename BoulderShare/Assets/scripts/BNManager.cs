using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System;

namespace BoulderNotes{
public class BNManager : SingletonMonoBehaviour<BNManager>
{
    [SerializeField] private AssetReference[] cornerPanels;
    private Sprite[] cornerPanelSprites;
    protected override void Awake(){
        base.Awake();
        cornerPanelSprites = new Sprite[cornerPanels.Length];
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60; 
        CanvasResolutionManager.Instance.Init();
        BNGymDataCenter.Instance.Init();
        BNScreens.Instance.Init();

        //Addressables.LoadAssetsAsync<Sprite>(cornerPanels[0],OnLoadCornerPanel);
    }
/*
    private void OnLoadCornerPanel(Sprite spr){
        cornerPanelSprites[index] = spr;
        index++;
        if(index < cornerPanels.Length){
            Addressables.LoadAssetsAsync<Sprite>(cornerPanels[index],OnLoadCornerPanel);
        }
    }*/

    public void GetCornerPanel(Action<Sprite> loadAction){
        int retina = (int)CanvasResolutionManager.Instance.GetRatioOfPtToPx() - 1;
        if (retina < 0 || retina > cornerPanels.Length - 1){
            if(loadAction != null){
                loadAction(null);
            }
        }
        if (cornerPanelSprites[retina] == null){
            Addressables.LoadAssetsAsync<Sprite>(cornerPanels[retina], loadAction);
        }else{
            loadAction(cornerPanelSprites[retina]);
        }
    }
}
}