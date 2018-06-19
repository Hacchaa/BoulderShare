using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class Observer : MonoBehaviour {
	public Camera curCamera;
	private GameObject focusObj;
	public const float CAMERA_DEPTH_LL = 1.2f;
	public const float CAMERA_DEPTH_UL = 12.0f;
	public const float WALL_W = 6.0f;
	public const float WALL_H = 4.0f;
	public const string KEY_HOLDS = "key_holds";
	public static int count = 0;
	private GameObject canvas;
	public static int currentPhase = 1;
	public enum Phase{HOLD_EDIT=1, SCENE_EDIT};
	private GameObject holds;
	private HScenes hScenes ;

	void Start(){
		focusObj = null;
		holds = GameObject.Find("Wall").transform.Find("Holds").gameObject;
		hScenes = GameObject.Find("HScenes").GetComponent<HScenes>();
		canvas = GameObject.Find("Canvas");
		currentPhase = 1;
		/*
		if (count < 1){
			SceneManager.activeSceneChanged += OnActiveSceneChanged;
			count++;
		}*/
	}

	public void InitHoldsAndScenes(){
		holds.GetComponent<Holds>().InitHolds();
		hScenes.InitScenes();
	}

	public Camera GetCamera(){
		return curCamera;
	}

	public void FocusObject(GameObject obj){
		if (focusObj != null){
			focusObj.SetActive(false);
		}

		obj.SetActive(true);
		focusObj = obj;
	}

	public GameObject GetFocusObject(){
		return focusObj;
	}

	public void ReleaseFocus(){
		if (focusObj != null){
			focusObj.SetActive(false);
			focusObj = null;
		}
	}

	public void SwitchPhase(int phase){
		ReleaseFocus();
		canvas.transform.Find("Phase"+currentPhase).gameObject.SetActive(false);

		canvas.transform.Find("Phase"+phase).gameObject.SetActive(true);
		currentPhase = phase;

		foreach(Transform child in holds.transform){
			if(phase == (int)Observer.Phase.HOLD_EDIT){
				child.Find("Phase2").gameObject.SetActive(false);
			}else if(phase == (int)Observer.Phase.SCENE_EDIT){
				child.Find("Hold_Scale").gameObject.SetActive(false);
				child.Find("Phase2").gameObject.SetActive(true);
			}
		}
	}
/*
	public void LoadScene(string next){
		if (SceneManager.GetActiveScene().name == "edit2"){
			GameObject.Find("Wall").transform.Find("Holds").gameObject.GetComponent<HData>().SaveHolds();
		}

		SceneManager.LoadScene(next);
	}

	public void OnActiveSceneChanged(Scene prevScene, Scene currentScene){
		GameObject wall = null;

		//Debug.Log("holds == "+PlayerPrefs.GetString(KEY_HOLDS));
		foreach(GameObject obj in currentScene.GetRootGameObjects()){
			if (obj.name == "Wall"){
				wall = obj;
				break;
			}
		}

		if (currentScene.name == "scenemake"){
			if (wall != null){
				SpriteRenderer rend = wall.transform.Find("image").gameObject.GetComponent<SpriteRenderer>();
				string filePath = Application.persistentDataPath + "/Wall.png";

				byte[] values;
		        using(FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read)){
		            BinaryReader bin = new BinaryReader(fileStream);
		            values = bin.ReadBytes((int)bin.BaseStream.Length);
		            bin.Close();
		        }
		        Texture2D texture = new Texture2D(1, 1);
		        texture.LoadImage(values);

		        rend.sprite = Sprite.Create(
		            texture, 
		            new Rect(0.0f, 0.0f, texture.width, texture.height), 
		            new Vector2(0.5f, 0.5f),
		            texture.height/4);

		        Transform parent = wall.transform.Find("Holds");
		        parent.gameObject.GetComponent<HData>().InitHolds(parent, PlayerPrefs.GetString(KEY_HOLDS));
		    }
		}else if(currentScene.name == "edit2"){
			Transform parent = wall.transform.Find("Holds");
		    parent.gameObject.GetComponent<HData>().InitHolds(parent, PlayerPrefs.GetString(KEY_HOLDS));
		}
	}*/
}
