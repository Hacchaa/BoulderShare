using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Kakera;
using UnityEngine.SceneManagement;

public class Phase1 : MonoBehaviour {
	public GameObject model;
	public GameObject slider;
	public GameObject holdOpe;
	public GameObject inclineObj;
	public ThreeDModel threeDModel;
	public enum TYPE{DEFAULT=-1,HOLDOPERATION, MODELSIZE, INCLINE};
	public static int curType = 0;
	public Observer observer;
	public Slider msSlider;
	private Camera cam;
	public TransformWall3 wall;
	[SerializeField]
	private BoRouteLSManager bManager;
	[SerializeField]
	private GameObject firstLoadObj;
	[SerializeField]
	private PickerController pc;
	[SerializeField]
	private GameObject plane;
	[SerializeField]
	private WallImg wallRend;
	// Use this for initialization
	void Start () {
		cam = observer.GetCamera();
	}

	public void FirstProc(){
		if (!bManager.IsLoaded()){
			firstLoadObj.SetActive(true);
		}else{
			AfterLoadingImage();
		}
	}

	public void LoadImageFromCameraRoll(){
		pc.OnPressShowPicker();
	}

	public void AfterLoadingImage(){
		firstLoadObj.SetActive(false);

		//3Dモデルの壁のサイズを決定する
		Bounds b = observer.GetWallBounds();
		plane.transform.localScale = new Vector3(b.size.x * 0.1f, 0.1f, b.size.y * 0.1f);
		plane.transform.parent.localPosition = new Vector3(0.0f, b.size.y/2, 0.0f);

		//3Dモデルの壁のテクスチャーを設定
		Renderer rend = plane.GetComponent<Renderer>();
		Material[] mat = rend.materials;
		Material m = new Material(mat[0]);
		m.SetTexture("_MainTex", wallRend.GetTexture());
		mat[0] = m;
		rend.materials = mat;

	}
/*
	private IEnumerator Load(){
		pc.OnPressShowPicker();
		yield return null;

		//画像が選択された場合
		if (pc.IsLoading()){
			//画像が読み込まれるまで待つ
			while(pc.IsLoadComplete()){
				yield return new WaitForSeconds(0.5f); 
			}
			//画像が読み込まれたかどうか調べる
			if (File.Exists(Application.persistentDataPath + Observer.WALLPATH)){
				//読み込まれた場合、FirstLoadフレームを隠す
				firstLoadObj.SetActive(false);
			}
		}
	}*/

	public void GoBackScene(){
		SceneManager.LoadScene("routeview");
	}

	public void SwitchSubMenu(int type){
		if (type == (int)TYPE.HOLDOPERATION){
			holdOpe.SetActive(true);
		}else{
			holdOpe.SetActive(false);
		}
		
		if (type == (int)TYPE.MODELSIZE){
			slider.SetActive(true);
			threeDModel.ChangeMode((int)ThreeDModel.Mode.MODEL_SIZE);
			//カメラを動かせなくして、正面に配置する
			wall.ResetCamPosAndDepth();
			wall.IgnoreTouch(true);
		}else{
			slider.SetActive(false);
			threeDModel.ChangeMode((int)ThreeDModel.Mode.DEFAULT);
			wall.IgnoreTouch(false);
		}
		
		if (type == (int)TYPE.INCLINE){
			inclineObj.SetActive(true);
		}else{
			inclineObj.SetActive(false);
		}
		curType = type;
		observer.ReleaseFocus();
	}
	
	public void SetModelSizeSlider(float val){
		model.transform.localScale = Vector3.one * val;
	}

	public void SetModelSize(float val){
		model.transform.localScale = Vector3.one * val;
		msSlider.value = val;
	}

	public float GetModelSize(){
		return model.transform.localScale.x;
	}
}
