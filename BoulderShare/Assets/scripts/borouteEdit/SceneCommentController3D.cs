using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SceneCommentController3D : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField inputF;
    [SerializeField]
    private GameObject commentPrefab;
    [SerializeField]
    private Transform pTrans;
    [SerializeField]
    private FontSizeSlider3D fss;
    [SerializeField]
    private ColorSetter3D colorSetter;
    [SerializeField]
    private CanvasGroup cg;
    [SerializeField]
    private SceneComment3D sc;
    [SerializeField] private SceneComments3D comments;
    [SerializeField] private Transform makeCommentPosition;
    [SerializeField] private CameraManager cManager;

    public void Init(){
        foreach(Transform t in pTrans){
            Destroy(t.gameObject);
        }
        comments.Init();
        Release();
        sc = null;
    }
    public void CommentLookAtCamera(){
        if (sc != null){
            sc.Look(cManager.Get3DRotation());
        }
    }

    public List<MyUtility.SceneCommentData3D> GetSceneComments(){
        List<MyUtility.SceneCommentData3D> list = new List<MyUtility.SceneCommentData3D>();

        foreach(Transform t in pTrans){
            SceneComment3D sceneComment = t.GetComponent<SceneComment3D>();
            MyUtility.SceneCommentData3D data = new MyUtility.SceneCommentData3D();
            data.text = sceneComment.GetText();
            data.fontSize = sceneComment.GetFontSize();
            data.pos = t.position;
            data.rot = t.localRotation;
            data.color = sceneComment.GetColor();
            data.width = sceneComment.GetWidth();

            list.Add(data);
        }

        return list;
    }

    public void SetSceneComments(List<MyUtility.SceneCommentData3D> list){
        Init();
        foreach(MyUtility.SceneCommentData3D data in list){
            GameObject obj = Instantiate(commentPrefab, pTrans);
            obj.SetActive(true);
            SceneComment3D sceneComment = obj.GetComponent<SceneComment3D>();
            //Debug.Log("data.text= "+ data.text);
            sceneComment.SetText(data.text);
            sceneComment.SetFontSize(data.fontSize);
            sceneComment.SetColor(data.color);
            obj.transform.position = data.pos;
            obj.transform.localRotation = data.rot;
            sceneComment.UpdateWidth(data.width);

            sceneComment.Focus(false);

            comments.AddComment(sceneComment);
        }
        comments.ShowDynamically();
    }
    

    public void SetActiveText(SceneComment3D com){
    	if (sc == com){
    		return ;
    	}
    	if(sc != null){
    		sc.Focus(false);
    	}

    	sc = com;
    	fss.SetFontSizeSliderVal(sc.GetFontSize());
    	colorSetter.SetAlphaSliderVal(sc.GetAlpha());

    	fss.gameObject.SetActive(true);
    	colorSetter.gameObject.SetActive(true);
    }

    public void ActiveIF(){
        //Debug.Log("ActiveIF");
    	if (sc != null){
    		inputF.text = sc.GetText();
    	}
    	//inputF.MoveTextEnd(false);
    	inputF.ActivateInputField();
    }

  	public void SyncText(){
    	sc.SetText(inputF.text);
    	sc.Resize();
    }

    public void Release(){
    	if(sc != null){
    		sc.Focus(false);
    	}
    	sc = null;

    	fss.gameObject.SetActive(false);
    	colorSetter.gameObject.SetActive(false);
    }

    public void CheckText(){
    	if (sc != null){
    		sc.CheckText();
    	}
    }

    public void MakeComment(){
    	GameObject obj = Instantiate(commentPrefab, pTrans);
    	obj.SetActive(true);
        obj.transform.position = makeCommentPosition.position;
        SceneComment3D com = obj.GetComponent<SceneComment3D>();
        com.Init();
        com.Look(cManager.Get3DRotation());
        com.SetText(SceneComment3D.INIT_STRING);
        com.Focus(true);
        comments.AddComment(com);
    	ActiveIF();
    }

    public void DeleteComment(SceneComment3D com){
        comments.DeleteComment(com);
    }

    public void ChangeFontSize(float v){
    	sc.SetFontSize(v);
    	sc.Resize();
    }

    public void ChangeAlpha(float v){
    	sc.SetAlpha(v);
    }

    public void ChangeColor(Color c){
    	sc.SetColor(c);
    }

    public void IgnoreEvents(){
		cg.blocksRaycasts = false;
	}

	public void AcceptEvents(){
		cg.blocksRaycasts = true;
	}
}
