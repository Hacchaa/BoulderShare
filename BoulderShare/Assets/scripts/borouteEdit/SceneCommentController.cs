using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SceneCommentController : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField inputF;
    [SerializeField]
    private GameObject commentPrefab;
    [SerializeField]
    private Transform pTrans;
    [SerializeField]
    private FontSizeSlider fss;
    [SerializeField]
    private ColorSetter colorSetter;
    [SerializeField]
    private CanvasGroup cg;

    [SerializeField]
    private SceneComment sc;

    public void Init(){
        foreach(Transform t in pTrans){
            Destroy(t.gameObject);
        }
        sc = null;
    }


    public List<MyUtility.SceneCommentData> GetSceneComments(){
        List<MyUtility.SceneCommentData> list = new List<MyUtility.SceneCommentData>();

        foreach(Transform t in pTrans){
            SceneComment sceneComment = t.GetComponent<SceneComment>();
            MyUtility.SceneCommentData data = new MyUtility.SceneCommentData();
            data.text = sceneComment.GetText();
            data.fontSize = sceneComment.GetFontSize();
            data.pos = t.position;
            data.rotDeg = t.rotation.eulerAngles.z;
            data.width = sceneComment.GetWidth();
            data.color = sceneComment.GetColor();

            list.Add(data);
        }

        return list;
    }

    public void SetSceneComments(List<MyUtility.SceneCommentData> list){
        Init();
        foreach(MyUtility.SceneCommentData data in list){
            GameObject obj = Instantiate(commentPrefab, pTrans);
            obj.SetActive(true);
            SceneComment sceneComment = obj.GetComponent<SceneComment>();
            //Debug.Log("data.text= "+ data.text);
            sceneComment.SetText(data.text);
            sceneComment.SetFontSize(data.fontSize);
            obj.transform.position = data.pos;
            obj.transform.rotation = Quaternion.Euler(0.0f, 0.0f, data.rotDeg);
            sceneComment.UpdateWidth(data.width);
            sceneComment.SetColor(data.color);

            sceneComment.Focus(false);
        }
    }
    

    public void SetActiveText(SceneComment com){
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
        Debug.Log("ActiveIF");
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
        SceneComment com = obj.GetComponent<SceneComment>();
        com.SetText(SceneComment.INIT_STRING);
        com.Focus(true);
    	ActiveIF();
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
