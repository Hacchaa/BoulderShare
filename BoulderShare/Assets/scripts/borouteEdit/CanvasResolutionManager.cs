using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasResolutionManager : MonoBehaviour
{
	[SerializeField] private List<Transform> canvasList;
	[SerializeField] private float topOutSafeArea = 44.0f;
	[SerializeField] private float botOutSafeArea = 34.0f;
	[SerializeField] private float margin = 16.0f;
    [SerializeField] private float marginXSMAX = 20.0f;
	[SerializeField] private ScreenTransitionManager sManager;
    private Rect safe;

    void Start()
    {	
    	safe = Screen.safeArea;
    	float width, topHeight, botHeight, retina;
        # if UNITY_EDITOR
    	   safe = SimulateIPhoneSafeArea(Screen.width, Screen.height);
        # endif

    	retina = GetRetina(Screen.width, Screen.height);
    	width = Screen.width;
    	topHeight = CalcTopSize();
    	botHeight = CalcBotSize();
/*
    	Debug.Log("width:"+width);
        Debug.Log("height:"+Screen.height);
    	Debug.Log("topHeight:"+topHeight);
    	Debug.Log("botHeight:"+botHeight);
    	Debug.Log("safeRect:"+safe);*/
    	foreach(Transform t in canvasList){
    		t.GetComponent<CanvasScaler>().scaleFactor = retina;
    		RectTransform top = t.Find("Top").GetComponent<RectTransform>();
    		RectTransform bot = t.Find("Bot").GetComponent<RectTransform>();
    		RectTransform content = t.Find("Content").GetComponent<RectTransform>();

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

			content.gameObject.SetActive(true);
    		content.anchorMin = new Vector2(0.0f, 0.0f);
    		content.anchorMax = new Vector2(1.0f, 1.0f);
    		content.offsetMin = new Vector2(0.0f, botHeight);
    		content.offsetMax = new Vector2(0.0f, -topHeight);
            content.localScale = Vector3.one;

            t.localScale = Vector3.one;
    	}

    	//float mar = CalcCanvasMargin(Screen.width, Screen.height);
    	float mar = GetMarginPt(Screen.width, Screen.height);
    	foreach(SEComponentBase com in sManager.GetUIList()){
    		foreach(RectTransform rect in com.GetMarginList()){
    			rect.anchorMin = new Vector2(0.0f, 0.0f);
	    		rect.anchorMax = new Vector2(1.0f, 1.0f);
	    		rect.offsetMin = new Vector2(mar, rect.offsetMin.y);
	    		rect.offsetMax = new Vector2(-mar, rect.offsetMax.y);
    		}
    	}
    }

    public List<Transform> GetCanvasList(){
        return new List<Transform>(canvasList);
    }

    public float CalcTopSize(){
        return safe.y / GetRetina(Screen.width, Screen.height);     
    }
    public float CalcBotSize(){
        return (Screen.height - (safe.y + safe.height)) / GetRetina(Screen.width, Screen.height);
    }

    private Rect SimulateIPhoneSafeArea(int width, int height){
    	Vector2 logicalPt, safeAreaPt;

    	if (width == 1125 && height == 2436){
			//iPhone X, iPhone XS
			logicalPt = new Vector2(375.0f, 812.0f);
			safeAreaPt = new Vector2(375.0f, 734.0f);

    	}else if ((width == 828 && height == 1792) || (width == 1242 && height == 2688)){
			//iPhone XR, iPhone XS MAX
			logicalPt = new Vector2(414.0f, 896.0f);
			safeAreaPt = new Vector2(414.0f, 818.0f);

    	}else{
    		//safearea無し
    		return new Rect(0.0f, 0.0f, Screen.width, Screen.height);
    	}

    	return new Rect(0.0f, topOutSafeArea / logicalPt.y * height, (safeAreaPt.x / logicalPt.x * width), (safeAreaPt.y / logicalPt.y * height));
    }

    private float SimulateDPI(int width, int height){
        if ((width == 1125 && height == 2436) || (width == 1242 && height == 2688)){
            //iPhone X, iPhone XS, iPhone XSMax
            return 458;
        }else if ((width == 828 && height == 1792) || (width == 750 && height == 1334)){
            //iPhone XR, iphone 8
            return 326;
        }else if (width == 1080 && height == 1920){
            //iphone 8 Plus
            return 401;
        }
        return Screen.dpi;
    }

    //マージンのピクセル量を返す
    public float CalcCanvasMargin(int width, int height){
        RectTransform canvasRect = canvasList[0].GetComponent<RectTransform>();
        return canvasRect.sizeDelta.x * (GetRetina(width, height) * GetMarginPt(width, height) / width);
    }

    public static float GetRetina(int width, int height){
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
            return 3.0f;
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
    public static float GetMarginPt(int width, int height){
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
