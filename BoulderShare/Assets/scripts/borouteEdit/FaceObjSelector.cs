using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceObjSelector : MonoBehaviour
{
	[SerializeField] private VRIKController ikController;
	[SerializeField] private LineRenderer line;
	private Transform from;
	private Transform to;
	private GameObject current;
	[SerializeField] private bool isDraw;

	void Start(){
		isDraw = false;
		line.gameObject.SetActive(false);
	}
    // Update is called once per frame
    void Update()
    {
        if (isDraw){
        	Vector3[] p = new Vector3[2];
        	p[0] = from.position;
        	p[1] = to.position;
        	line.SetPositions(p);
        }
    }

    public void Regist(VRIKController.FullBodyMark m){
    	if (!IsFaceObj(m)){
    		return ;
    	}

    	GameObject obj = ikController.GetObj(m);
    	VRIKComponent com = obj.GetComponent<VRIKComponent>();
    	from = com.GetFaceAvatar();
    	to = com.GetTarget();    	

    	if (current != null){
    		current.SetActive(false);
    	}
    	obj.SetActive(true);

    	current = obj;
    	isDraw = true;
    	line.gameObject.SetActive(true);
    }

    public void Init(){
    	ikController.HideAllFO();
    	line.gameObject.SetActive(false);
    }

    public void Release(){
    	if (current != null){
    		current.SetActive(false);
    	}
    	line.gameObject.SetActive(false);
    	isDraw = false;
    }
    public bool IsFaceObj(VRIKController.FullBodyMark m){
    	return (m == VRIKController.FullBodyMark.Chest) || 
		    	(m == VRIKController.FullBodyMark.LeftElbow) || 
		    	(m == VRIKController.FullBodyMark.RightElbow) || 
		    	(m == VRIKController.FullBodyMark.LeftKnee) || 
		    	(m == VRIKController.FullBodyMark.RightKnee) || 
		    	(m == VRIKController.FullBodyMark.Look);
	    }
}
