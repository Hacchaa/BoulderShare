using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasResolutionManager : MonoBehaviour
{
	[SerializeField] private List<Transform> canvasList;
	[SerializeField] private float topOutSafeArea = 44.0f;
	[SerializeField] private float botOutSafeArea = 34.0f;
	[SerializeField] private float margin = 16.0f;
	[SerializeField] private ScreenTransitionManager sManager;

    void Start()
    {	
    	Rect safe = Screen.safeArea;
    	float width, topHeight, botHeight;

    	#if UNITY_IPHONE
    		safe = SimulateIPhoneSafeArea(Screen.width, Screen.height);
    	#endif

    	width = Screen.width;
    	topHeight = safe.y;
    	botHeight = Screen.height - (safe.y + safe.height);
    	Debug.Log("width:"+width);
    	Debug.Log("topHeight:"+topHeight);
    	Debug.Log("botHeight:"+botHeight);
    	Debug.Log("safeRect:"+safe);
    	foreach(Transform t in canvasList){
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

    		if(botHeight <= 0.0f){
    			bot.gameObject.SetActive(false);
    		}else{
    			bot.gameObject.SetActive(true);
    		}
		    bot.anchorMin = new Vector2(0.0f, 0.0f);
			bot.anchorMax = new Vector2(1.0f, 0.0f);
			bot.anchoredPosition = new Vector2(0.0f, botHeight/2.0f);
			bot.sizeDelta = new Vector2(0.0f, botHeight);

			content.gameObject.SetActive(true);
    		content.anchorMin = new Vector2(0.0f, 0.0f);
    		content.anchorMax = new Vector2(1.0f, 1.0f);
    		content.offsetMin = new Vector2(0.0f, botHeight);
    		content.offsetMax = new Vector2(0.0f, -topHeight);
    	}

    	float margin = GetMargin();
    	foreach(SEComponentBase com in sManager.GetUIList()){
    		foreach(RectTransform rect in com.GetMarginList()){
    			rect.anchorMin = new Vector2(0.0f, 0.0f);
	    		rect.anchorMax = new Vector2(1.0f, 1.0f);
	    		rect.offsetMin = new Vector2(margin, rect.offsetMin.y);
	    		rect.offsetMax = new Vector2(-margin, rect.offsetMax.y);
    		}
    	}
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

    public float GetMargin(){
    	float dpi = Screen.dpi;
    	if (dpi == 0.0f){
    		return 42.0f;
    	}
    	return margin * (Screen.dpi / 72.0f);
    }

}
