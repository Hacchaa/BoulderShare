using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SceneCommentRotation :MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
	private static int FINGER_NONE = -10;
	private int finger = FINGER_NONE;
	[SerializeField] private ThreeDWall threeDWall;
	[SerializeField] private float WEIGHT = 0.5f;
	[SerializeField] private CameraManager cManager;
	[SerializeField] private GameObject grid;
	[SerializeField] private Transform root;
    private Vector3 startMovePos;
    private Vector3 startRootPos;
    private Quaternion startRot;
    private float startFOV;

	public void OnPointerDown(PointerEventData data){
		if (finger == FINGER_NONE){
			finger = data.pointerId;

	    	startRootPos = cManager.GetRootWorldPos();
	    	startMovePos = cManager.GetMovePos();
	    	startRot = cManager.Get3DRotation();

	    	startFOV = cManager.Get3DFOV();
	   
	    	cManager.Transform3DWithAnim(root.position, startRot, startFOV);
	    	grid.SetActive(true); 
	    	grid.transform.position = root.position;

	    	threeDWall.HideTranslucentWall();
		}		
	}

	public void OnPointerUp(PointerEventData data){
		if (data.pointerId == finger){
			finger = FINGER_NONE;

			//cManager.Transform3DWithAnim(startRootPos, startRot, startFOV, startMovePos);

	    	grid.SetActive(false); 
	    	threeDWall.ShowTranslucentWall();
		}
	}

	public void OnDrag(PointerEventData data){
		if (data.pointerId == finger){
			//root.Rotate(0.0f, -data.delta.x * WEIGHT, 0.0f);
			cManager.Rotate3D(0.0f, -data.delta.x * WEIGHT, 0.0f);
			root.localRotation = cManager.Get3DRotation();
			grid.transform.localRotation = root.localRotation;
		}
	}
}
