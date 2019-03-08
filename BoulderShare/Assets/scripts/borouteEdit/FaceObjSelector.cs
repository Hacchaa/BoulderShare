using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceObjSelector : MonoBehaviour
{
	[SerializeField] private VRIKController ikController;
	private VRIKComponent current;

    public void Regist(VRIKController.FullBodyMark m){
    	if (!ikController.IsFaceObj(m)){
    		return ;
    	}

    	GameObject obj = ikController.GetObj(m);
    	VRIKComponent com = obj.GetComponent<VRIKComponent>(); 	

    	if (current != null){
    		current.Hide();
    	}
    	com.Show();
    	current = com;
    }

    public void Init(){
    	ikController.HideAllFO();
    }

    public void Release(){
    	if (current != null){
    		current.Hide();
    	}
    }
}
