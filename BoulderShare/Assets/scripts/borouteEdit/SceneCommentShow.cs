using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SceneCommentShow : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	[SerializeField] private SceneCommentController3D scController;
	[SerializeField] private SceneComment3D sc;
	[SerializeField] private GameObject focusObj;
	[SerializeField] private Transform parent;
    private Camera cam;
    private static int FINGER_NONE = -10;
    private static int finger = FINGER_NONE;
    private Vector3 baseP;
    private bool isCommentEditable;
    private float baseDepth;


    void Awake(){
  		cam = sc.GetCamera();
    }



    public void OnPointerDown(PointerEventData data){
        if (focusObj.activeSelf){
        	isCommentEditable = true;
    	}else{
    		sc.Focus(true);
    	}
	}	

    public void OnPointerUp(PointerEventData data){
        if(isCommentEditable){
            scController.ActiveIF();
        }
        isCommentEditable = false;
    }

    public void OnBeginDrag(PointerEventData data){
        isCommentEditable = false;
        if (finger == FINGER_NONE){
            finger = data.pointerId;
            baseDepth = cam.gameObject.transform.InverseTransformPoint(parent.position).z;
            baseP = cam.ScreenToWorldPoint(
                new Vector3(
                    data.position.x, 
                    data.position.y, 
                    baseDepth));

            baseP = baseP - parent.position;
        }
    }

    //マークを動かす
    public void OnDrag(PointerEventData data){
        if (data.pointerId == finger){
            Vector3 p = cam.ScreenToWorldPoint(
                new Vector3(
                    data.position.x, 
                    data.position.y, 
                    baseDepth));
            
            parent.position = p - baseP;
        }
    }

    public void OnEndDrag(PointerEventData data){
        if (data.pointerId == finger){
            finger = FINGER_NONE;
            baseP = Vector3.zero;
        }
    }
}
