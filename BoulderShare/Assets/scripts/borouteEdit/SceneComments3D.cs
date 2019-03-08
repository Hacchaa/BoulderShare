using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SceneComments3D : MonoBehaviour
{
	private List<SceneComment3D> list;
	private bool isUpdateDynamically;
	[SerializeField] private float showAngle = 90.0f;
	public static float ANGLE_VIEW = 30.0f;
	public static float ANGLE_EDIT = 90.0f;
	private int index;
	[SerializeField] private CameraManager cameraManager;
	void Awake(){
		list = new List<SceneComment3D>();
		index = -1;
		isUpdateDynamically = false;
	}

	public void Init(){
		Awake();
	}

	public void Next(){
		if (!list.Any()){
			return ;
		}

		index++;
		if (index >= list.Count){
			index = 0;
		}

		cameraManager.Rotate3DWithAnim(list[index].transform.localRotation);
	}

	public void SetShowAngle(float f){
		showAngle = f;
	}

	void Update(){
		if(isUpdateDynamically){
			foreach(SceneComment3D com in list){
				//Debug.Log(com.GetDeltaAngleToCam());
				if (Mathf.Abs(com.GetDeltaAngleToCam()) <= showAngle){
					com.ShowComment(true);
				}else{
					com.ShowComment(false);
				}
			}
		}
	}

	public void AddComment(SceneComment3D com){
		list.Add(com);
	}

	public void DeleteComment(SceneComment3D com){
		if (com != null){
			list.Remove(com);
			Destroy(com.gameObject);
		}
		if (index >= list.Count){
			index = -1;
		}
	}
    
    public void ShowAll(){
    	foreach(SceneComment3D com in list){
    		com.ShowComment(true);
    	}
    	isUpdateDynamically = false;
    }
    public void DontShowAll(){
    	foreach(SceneComment3D com in list){
    		com.ShowComment(false);
    	}
    	isUpdateDynamically = false;
    }

    public void ShowDynamically(){
    	SetShowAngle(ANGLE_VIEW);
    	isUpdateDynamically = true;
    }
}
