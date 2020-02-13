using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasResolutionManager :SingletonMonoBehaviour<CanvasResolutionManager>
{
	[SerializeField] private List<Transform> canvasList;
    [SerializeField] private float PT_STATUSBAR = 20.0f;
	[SerializeField] private ScreenTransitionManager sManager;
    [SerializeField] private float statusBarHeight;
    [SerializeField] private float botSafeAreaInset;

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
    public void Init()
    {	
    	Rect safe = Screen.safeArea;
    	float width, topHeight, botHeight, retina;

        # if UNITY_EDITOR
    	   safe = SimulateIPhoneSafeArea();
        # endif

    	retina = GetRatioOfPtToPx();
        if (hasStatusBarInSafeArea()){
            safe.yMin += PT_STATUSBAR * retina;
        }

    	topHeight = safe.y / retina;
    	botHeight = (Screen.height - (safe.y + safe.height)) / retina;

        statusBarHeight = topHeight;
        botSafeAreaInset = botHeight;
        if (botHeight < 1.0f){
            botHeight = 0.0f;
        }
/* 
    	Debug.Log("width:"+Screen.width);
        Debug.Log("height:"+Screen.height);
    	Debug.Log("topHeight:"+topHeight);
    	Debug.Log("botHeight:"+botHeight);
    	Debug.Log("safeRect:"+safe);
        Debug.Log("safeRect2:"+Screen.safeArea);
        Debug.Log("retina:"+retina);*/
    	foreach(Transform t in canvasList){
    		t.GetComponent<CanvasScaler>().scaleFactor = retina;		
            t.localScale = Vector3.one;
    	}

    	float mar = GetMarginPt();
    	foreach(SEComponentBase com in sManager.GetUIList()){
            RectTransform top = null;
            RectTransform bot = null;
            RectTransform content = null;
            Transform t = null;
            t = com.transform.Find("Top");
            if (t != null){
                top = t.GetComponent<RectTransform>();
            }
            t = com.transform.Find("Bot");
            if (t != null){
                bot = t.GetComponent<RectTransform>();
            }  
            t = com.transform.Find("Content");
            if (t != null){
                content = t.GetComponent<RectTransform>();
            }

            if (top != null){
                if(topHeight <= 0.0f){
                    top.gameObject.SetActive(false);
                }else{
                    top.gameObject.SetActive(true);
                }
            
                top.anchorMin = new Vector2(0.0f, 1.0f);
                top.anchorMax = new Vector2(1.0f, 1.0f);
                top.anchoredPosition = new Vector2(0.0f, -topHeight/2.0f);
                top.sizeDelta = new Vector2(0.0f, topHeight);
                top.localScale = Vector3.one;
            }
            if (bot != null){
                if(botHeight <= 0.0f){
                    bot.gameObject.SetActive(false);
                }else{
                    bot.gameObject.SetActive(true);
                }
                bot.anchorMin = new Vector2(0.0f, 0.0f);
                bot.anchorMax = new Vector2(1.0f, 0.0f);
                bot.anchoredPosition = new Vector2(0.0f, botHeight/2.0f);
                bot.sizeDelta = new Vector2(0.0f, botHeight);
                bot.localScale = Vector3.one;
            }
            if (content != null){
                content.gameObject.SetActive(true);
                content.anchorMin = new Vector2(0.0f, 0.0f);
                content.anchorMax = new Vector2(1.0f, 1.0f);
                if (bot != null){
                    content.offsetMin = new Vector2(0.0f, botHeight);
                }else{
                    content.offsetMin = new Vector2(0.0f, 0.0f);
                }
                if (top != null){
                    content.offsetMax = new Vector2(0.0f, -topHeight);
                }else{
                    content.offsetMax = new Vector2(0.0f, 0.0f);
                }
                content.localScale = Vector3.one;
            }

    		foreach(RectTransform rect in com.GetMarginList()){
    			rect.anchorMin = new Vector2(0.0f, 0.0f);
	    		rect.anchorMax = new Vector2(1.0f, 1.0f);
	    		rect.offsetMin = new Vector2(mar, rect.offsetMin.y);
	    		rect.offsetMax = new Vector2(-mar, rect.offsetMax.y);
    		}
    	}
    }

    public List<Transform> GetCanvasLisat(){
        return new List<Transform>(canvasList);
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

    public static float GetRatioOfPtToPx(){
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
    public static float GetMarginPt(){
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
