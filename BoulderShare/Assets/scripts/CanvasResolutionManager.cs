using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BoulderNotes{
public class CanvasResolutionManager :SingletonMonoBehaviour<CanvasResolutionManager>
{
    [SerializeField] private float PT_STATUSBAR = 20.0f;
    [SerializeField] private float statusBarHeight;
    [SerializeField] private float botSafeAreaInset;
    [SerializeField] private List<CanvasScaler> canvases; 
    public void Init(){
        InitHeights();

        float retina = GetRatioOfPtToPx();
        foreach(CanvasScaler scaler in canvases){
            scaler.scaleFactor = retina;
        }
    }
    public float GetStatusBarHeight(){
        return statusBarHeight;
    }
    public float GetBotSafeAreaInset(){
        return botSafeAreaInset;
    }
    public void InitHeights(){
        Rect safe = Screen.safeArea;
        # if UNITY_EDITOR
    	   safe = SimulateIPhoneSafeArea();
        # endif
        float retina = GetRatioOfPtToPx();
        if (hasStatusBarInSafeArea()){
            safe.yMin += PT_STATUSBAR * retina;
        }
    	statusBarHeight = safe.y / retina;
    	botSafeAreaInset = (Screen.height - (safe.y + safe.height)) / retina;       
    }
    private Rect SimulateIPhoneSafeArea(){
        float width = Screen.width;
        float height = Screen.height;
        float retina = GetRatioOfPtToPx();

    	if (width == 1125 && height == 2436){
			//iPhone X, iPhone XS
            return new Rect(0.0f, 44.0f * retina, 375.0f * retina, 734.0f * retina);
    	}else if ((width == 828 && height == 1792) || (width == 1242 && height == 2688)){
			//iPhone XR, iPhone XS MAX
            return new Rect(0.0f, 44.0f * retina, 414.0f * retina, 818.0f * retina);
    	}else{
    		//safearea無し
    		return new Rect(0.0f, 0.0f, width, height);
    	}
    }

    public bool hasStatusBarInSafeArea(){
        float width = Screen.width;
        float height = Screen.height;
        if(width == 1125 && height == 2436){
            //iPhoneX iPhoneXS
            return false;
        }else if(width == 828 && height == 1792){
            //iPhoneXR
            return false;
        }else if(width == 1242 && height == 2688){
            //iPhoneXSMax
            return false;
        }

        return true;
    }

    public float GetRatioOfPtToPx(){
        float width = Screen.width;
        float height = Screen.height;

        if (width == 320 && height == 480){
            //iPhone iPhone3G iPhone3GS
            return 1.0f;
        }else if(width == 640 && height == 960){
            //iPhone4 iPhone4S
            return 2.0f;
        }else if(width == 640 && height == 1136){
            //iPhone5 iPhone5s iPhone5c iPhoneSE
            return 2.0f;
        }else if(width == 750 && height == 1334){
            //iPhone6 iPhone6S iPhone7 iPhone8
            return 2.0f;
        }else if(width == 1080 && height == 1920){
            //iPhone6Plus iPhone6SPlus iPhone7Plus iPhone8Plus
            return 2.6087f;
        }else if(width == 1125 && height == 2436){
            //iPhoneX iPhoneXS
            return 3.0f;
        }else if(width == 828 && height == 1792){
            //iPhoneXR
            return 2.0f;
        }else if(width == 1242 && height == 2688){
            //iPhoneXSMax
            return 3.0f;
        }
        return 1.0f;
    }
    public float GetMarginPt(){
        float width = Screen.width;
        float height = Screen.height;

        if(width == 1125 && height == 2436){
            //iPhoneX iPhoneXS
            return 16.0f;
        }else if(width == 828 && height == 1792){
            //iPhoneXR
            return 16.0f;
        }else if(width == 1242 && height == 2688){
            //iPhoneXSMax
            return 20.0f;
        }
        return 16.0f;
    }
}
}