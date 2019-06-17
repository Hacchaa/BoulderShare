using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

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
    private ColorSetter3DComment colorSetter;
    [SerializeField]
    private CanvasGroup cg;
    [SerializeField]
    private SceneComment3D sc;

    [SerializeField] private Transform makeCommentPosition;
    [SerializeField] private CameraManager cManager;
    [SerializeField] private SceneEditorComment sceneEditorComment;

    private List<SceneComment3D> commentList;
    private bool isUpdateDynamically;
    private int index;
    [SerializeField] private float showAngle = 90.0f;
    public static float ANGLE_VIEW = 30.0f;
    public static float ANGLE_EDIT = 90.0f;
    public static float DEF_DEPTH = 4.0f;
    void Start(){
        Init();
    }
    public void Init(){
        foreach(Transform t in pTrans){
            Destroy(t.gameObject);
        }

        commentList = new List<SceneComment3D>();
        index = -1;
        isUpdateDynamically = false;

        Release(false);
    }
    public void CommentLookAtCamera(){
        if (sc != null){
            sc.Look(cManager.Get3DRotation());
        }
    }

    public List<MyUtility.SceneCommentData3D> GetSceneComments(){
        List<MyUtility.SceneCommentData3D> list = new List<MyUtility.SceneCommentData3D>();

        foreach(SceneComment3D sceneComment in commentList){
            MyUtility.SceneCommentData3D data = new MyUtility.SceneCommentData3D();
            data.text = sceneComment.GetText();
            data.fontSize = sceneComment.GetFontSize();
            data.pos = sceneComment.transform.position;
            data.rot = sceneComment.transform.localRotation;
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
            sceneComment.Init();
            sceneComment.SetText(data.text);
            sceneComment.SetFontSize(data.fontSize);
            sceneComment.SetColor(data.color);
            obj.transform.position = data.pos;
            obj.transform.localRotation = data.rot;
            sceneComment.UpdateWidth(data.width);

            sceneComment.Focus(false);

            AddComment(sceneComment);
        }
        ShowDynamically();
    }
    

    public void SetActiveText(SceneComment3D com){
        bool isAlreadyFocused = false;

    	if (sc == com){
    		return ;
    	}
    	if(sc != null){
    		sc.Focus(false);
            isAlreadyFocused = true;
    	}

    	sc = com;
    	fss.SetFontSizeSliderVal(sc.GetFontSize());
    	colorSetter.SetAlphaSliderVal(sc.GetAlpha());

    	sceneEditorComment.OpenFocusField(isAlreadyFocused);
        cManager.Transform3DWithAnim(sc.transform.position, sc.transform.rotation, DEF_DEPTH);
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

    public void Release(bool isWithAnimation = true){
    	if(sc != null){
    		sc.Focus(false);
            if (isWithAnimation){   
                sceneEditorComment.CloseFocusField();
            }
    	}
    	sc = null;
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
        AddComment(com);
    	ActiveIF();
    }

    public void ChangeFontSize(float v){
        if (sc != null){
        	sc.SetFontSize(v);
        	//sc.Resize();
        }
    }

    public void ChangeAlpha(float v){
        if (sc != null){
    	   sc.SetAlpha(v);
        }
    }

    public void ChangeColor(Color c){
        if (sc != null){
            sc.SetColor(c);
        }
    }

    public void IgnoreEvents(){
		cg.blocksRaycasts = false;
        foreach(SceneComment3D com in commentList){
            com.IgnoreEvent();
        }
	}

	public void AcceptEvents(){
		cg.blocksRaycasts = true;
        foreach(SceneComment3D com in commentList){
            com.AcceptEvent();
        }
	}

    public void Next(){
        if (!commentList.Any()){
            return ;
        }

        index++;
        if (index >= commentList.Count){
            index = 0;
        }

        cManager.Rotate3DWithAnim(commentList[index].transform.localRotation);
    }

    public void SetShowAngle(float f){
        showAngle = f;
    }

    void Update(){
        if(isUpdateDynamically){
            float camY = cManager.Get3DRotation().eulerAngles.y;
            foreach(SceneComment3D com in commentList){
                float delta = Mathf.DeltaAngle(camY, com.transform.localRotation.eulerAngles.y);
                if (Mathf.Abs(delta) <= showAngle){
                    com.ShowComment(true);
                }else{
                    com.ShowComment(false);
                }
            }
        }
    }

    public void AddComment(SceneComment3D com){
        commentList.Add(com);
    }

    public void DeleteComment(SceneComment3D com){
        if (com != null){
            commentList.Remove(com);
            Destroy(com.gameObject);
        }
        if (index >= commentList.Count){
            index = -1;
        }
    }
    
    public void ShowAll(){
        foreach(SceneComment3D com in commentList){
            com.ShowComment(true);
        }
        isUpdateDynamically = false;
    }

    public void DontShowAll(){
        foreach(SceneComment3D com in commentList){
            com.ShowComment(false);
        }
        isUpdateDynamically = false;
    }

    public void ShowDynamically(){
        SetShowAngle(ANGLE_VIEW);
        isUpdateDynamically = true;
    }
}
