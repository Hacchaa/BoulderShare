using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SceneComment : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	[SerializeField]
	private RectTransform rectT;
	[SerializeField]
	private TextMeshProUGUI tmpText;
	[SerializeField]
	private SceneCommentController scController;
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private GameObject focusObj;
    [SerializeField]
    private Image frameImage;
    [SerializeField]
    private GameObject frame;
    [SerializeField]
    private TwoDWall twoDWall;

    private static int FINGER_NONE = -10;
    private static int finger = FINGER_NONE;
	public static string INIT_STRING = "タップして編集";
    private float MIN_HEIGHT = 60.0f;

    private Vector3 baseP;
    private bool isCommentEditable;
    private Bounds bounds;


    void Awake(){
        rectT = gameObject.GetComponent<RectTransform>();
        Resize();
        bounds = twoDWall.GetWallBounds();
    }

    public void Delete(){
        scController.Release();
        Destroy(gameObject);
    }

    public void Focus(bool b){
        Color c = frameImage.color;
        if (b){
            c.a = 20.0f / 255.0f;
            focusObj.SetActive(true);
            frame.SetActive(true);
            scController.SetActiveText(this);
        }else{
            c.a = 0.0f;
            focusObj.SetActive(false);
            frame.SetActive(false);
        }
        frameImage.color = c;
    }

    public void OnPointerDown(PointerEventData data){
        if (focusObj.activeSelf){
        	isCommentEditable = true;
    	}else{
    		Focus(true);
    	}
	}	

    public void OnPointerUp(PointerEventData data){
        if(isCommentEditable){
            scController.ActiveIF();
        }
        isCommentEditable = false;
    }

    public void Resize(){
        float height = tmpText.preferredHeight;
        if (height < MIN_HEIGHT){
            height = MIN_HEIGHT;
        }
    	rectT.sizeDelta = new Vector2(rectT.rect.width , height );
    	//rectT.sizeDelta = new Vector2(tmpText.preferredWidth , tmpText.preferredHeight );
    	//rectT.sizeDelta = new Vector2(tmpText.preferredWidth , tmpText.preferredHeight );
    }

    public void SetFontSize(float val){
        tmpText.fontSize = val;
    }

    public float GetFontSize(){
        return tmpText.fontSize;
    }

    public void SetAlpha(float v){
        tmpText.alpha = v;
    }

    public float GetAlpha(){
        return tmpText.alpha;
    }

    public void SetColor(Color c){
        float a = tmpText.alpha;
        tmpText.color = c;
        tmpText.alpha = a;
    }

    public Color GetColor(){
        return tmpText.color;
    }

    public float GetWidth(){
        return rectT.sizeDelta.x;
    }

    public string GetText(){
        return tmpText.text;
    }

    public void SetText(string str){
    	tmpText.text = str;
    }

    public void CheckText(){
    	if(string.IsNullOrEmpty(tmpText.text)){
    		SetText(INIT_STRING);
    		Resize();
    	}
    }

    public void UpdateWidth(float w){
        rectT.sizeDelta = new Vector2(w , tmpText.preferredHeight );
        Resize();
    }

    public void OnBeginDrag(PointerEventData data){
        isCommentEditable = false;
        if (finger == FINGER_NONE){
            finger = data.pointerId;
            baseP = cam.ScreenToWorldPoint(
                new Vector3(
                    data.position.x, 
                    data.position.y, 
                    -cam.transform.position.z));

            baseP = baseP - transform.position;
        }
    }

    //マークを動かす
    public void OnDrag(PointerEventData data){
        if (data.pointerId == finger){
            Vector3 p = cam.ScreenToWorldPoint(
                new Vector3(
                    data.position.x, 
                    data.position.y, 
                    -cam.transform.position.z));
            
            transform.position = p - baseP;

            p = transform.position;

            float height = bounds.size.y;
            float width = bounds.size.x;
            p.x = Mathf.Min(p.x, width/2);
            p.x = Mathf.Max(p.x, -width/2);
            p.y = Mathf.Min(p.y, height/2);
            p.y = Mathf.Max(p.y, -height/2);

            transform.position = p;
        }
    }

    public void OnEndDrag(PointerEventData data){
        if (data.pointerId == finger){
            finger = FINGER_NONE;
            baseP = Vector3.zero;
        }
    }
}
