using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerGraphController : MonoBehaviour
{
    [SerializeField] private CameraManager cManager;
    [SerializeField] private LayerGraphMove lgMove;

    public void SetViewportRect(float viewPortY, float h){
    	SetViewportRect(0.0f, viewPortY, 1.0f, h);
    }

    public void ResetViewportRect(){
    	SetViewportRect(0.0f, 0.0f, 1.0f, 1.0f);
    }

    private void SetViewportRect(float x, float y, float w, float h){
    	List<Camera> cameraList = cManager.GetCameras();
    	foreach(Camera cam in cameraList){
    		cam.rect = new Rect(x, y, w, h);
    	}
    }

    public void ResetLGCameraPos(){
    	lgMove.ResetCamPosAndDepth();
    }

    public void SetCameraActive(bool b){
    	lgMove.SetCameraActive(b);
    }
}
