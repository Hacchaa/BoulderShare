using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraUtility : MonoBehaviour
{
   [SerializeField] private Transform depth;
   [SerializeField] private float duration = 0.5f;
   [SerializeField] private Camera windowCam;
   private float rate = 0.3f;
   private float yBlankRate = 0.2f;
   private List<Vector3> fixPosTmp;
    void Start(){

    	float length, x, y;
    	length = Screen.width * rate;
    	x = Screen.width * (1 - rate);
    	y = Screen.height * (1 - (yBlankRate + length / (float)Screen.height));
    	windowCam.pixelRect = new Rect(x, y, length, length);
    	//Debug.Log(x + "," + y + "," +length);
    }
    public float GetWidthRate(){
    	return rate;
    }
    public void RotateWithAnim(Quaternion rot){
    	transform.DORotateQuaternion(rot, duration);
    }
    public void Zoom(float f){
    	depth.localPosition = depth.localPosition + Vector3.forward * f;
    }

    public void SetActive(bool b){
    	depth.gameObject.SetActive(b);
    }

    public void SetPosWithFixedHierarchyPos(Vector3 pos){
        fixPosTmp = new List<Vector3>();
        StorePos(transform, true);

        transform.position = pos;

        RestorePos(transform, true);
    }

    private void StorePos(Transform t, bool isRoot){
        if (!isRoot){
            fixPosTmp.Add(t.position);
        }
        foreach(Transform child in t){
            StorePos(child, false);
        }
    }

    private void RestorePos(Transform t, bool isRoot){
        if (!isRoot){
            t.transform.position = fixPosTmp[0];
            fixPosTmp.RemoveAt(0);
        }
        foreach(Transform child in t){
            RestorePos(child, false);
        }
    }
}
