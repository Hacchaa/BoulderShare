using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SceneComment3D : MonoBehaviour
{
	[SerializeField] private RectTransform rectT;
	[SerializeField] private TextMeshProUGUI tmpText;
    [SerializeField] private RectTransform textRect;
	[SerializeField]
	private SceneCommentController3D scController;
    [SerializeField]
    private GameObject focusObj;
    [SerializeField]
    private Image frameImage;
    [SerializeField]
    private GameObject frame;
    [SerializeField]
    private TwoDWall twoDWall;
    [SerializeField] private Transform showObj;
    [SerializeField] private Transform dontShowObj;
    [SerializeField] private CommentExtendWidth3D cewRight;
    [SerializeField] private CommentExtendWidth3D cewLeft;
    [SerializeField] private SceneCommentShow scs;
    [SerializeField] private Camera camera;
    [SerializeField] private EditorPopup popup;
    [SerializeField] private ColorSetter3D colorSetter;
    [SerializeField] private float margin = 12.0f;

    private bool isShowing;

	public static string INIT_STRING = "タップして編集";
    private float MIN_HEIGHT = 60.0f;
    private float WIDTH = 300.0f;
    private float MIN_FRAMEWIDTH = 80.0f;


    public void Init(){
        rectT = gameObject.GetComponent<RectTransform>();
        Resize();
        ShowComment(true);
        cewRight.SetCamera(camera);
        cewLeft.SetCamera(camera);
        scs.SetCamera(camera);
    }

    public void ShowComment(bool b){
        if(b){
            Show();
        }else{
            DontShow();
        }
    }

    private void Show(){
        showObj.gameObject.SetActive(true);
        dontShowObj.gameObject.SetActive(false);
        isShowing = true;
    }

    private void DontShow(){
        showObj.gameObject.SetActive(false);
        dontShowObj.gameObject.SetActive(true);
        isShowing = false;
    }

    public void Look(Quaternion rot){
        transform.localRotation = rot;
    }

    public void Rotate(float x, float y, float z){
        transform.Rotate(x, y, z);
    }

    public void Delete(){
        popup.Open(DeleteProc, null, "コメントを削除しますか？", "", "削除", "キャンセル");
    }

    private void DeleteProc(){
        scController.Release();
        scController.DeleteComment(this);       
    }

    public void Focus(bool b){
        Color c = frameImage.color;
        if (b){
            //c.a = 20.0f / 255.0f;
            focusObj.SetActive(true);
            frame.SetActive(true);
            scController.SetActiveText(this);
        }else{
            //c.a = 0.0f;
            focusObj.SetActive(false);
            frame.SetActive(false);
        }
        colorSetter.InitObjs();
        //frameImage.color = c;
    }

    public void Resize(){
        float height = tmpText.preferredHeight;
        if (height < MIN_HEIGHT){
            height = MIN_HEIGHT;
        }
    	//rectT.sizeDelta = new Vector2(rectT.rect.width , height );
        //rectT.sizeDelta = new Vector2(WIDTH , height);
    	//rectT.sizeDelta = new Vector2(tmpText.preferredWidth , tmpText.preferredHeight );
    	//rectT.sizeDelta = new Vector2(tmpText.preferredWidth , tmpText.preferredHeight );
    }

    public void SetFontSize(float val){
        tmpText.fontSize = val;
        UpdateWidth(rectT.sizeDelta.x);
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
        /*
        if (tmpText.GetTextInfo(tmpText.text).lineCount == 1){
            if (w > tmpText.preferredWidth){
                w = tmpText.preferredWidth;
            }
        }*/
        float textMin = tmpText.fontSize;
        if (w < textMin){
            w = textMin;
        }

        float diff = 0.0f;
        float frameW = w;
        if (w < MIN_FRAMEWIDTH){
            diff = MIN_FRAMEWIDTH - w;
            frameW = MIN_FRAMEWIDTH;
        }
        rectT.sizeDelta = new Vector2(w , 0.0f);
        rectT.sizeDelta = new Vector2(w , tmpText.preferredHeight + margin*2 );

        rectT.sizeDelta = new Vector2(frameW , rectT.sizeDelta.y);
        textRect.offsetMin = new Vector2(diff/2.0f, 0.0f);
        textRect.offsetMax = new Vector2(-diff/2.0f, 0.0f);
    }
}
