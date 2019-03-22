using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RootMotion.FinalIK;

public class FBBIKController : MonoBehaviour, IHumanModelController
{
	[SerializeField] private Transform[] avatarReferences;
    [SerializeField] private List<FBBIKBase> markList;
    [SerializeField] private Camera cam;
    private Dictionary<MyUtility.FullBodyMark, FBBIKBase> map;
    [SerializeField] private AimIK aimIK;
    [SerializeField] private FullBodyBipedIK ik;
    [SerializeField] private Transform model;

    public void Start(){
    	Init();
    	InitMarks();
    }
    public void Init(){
    	if (map == null){
    		map = new Dictionary<MyUtility.FullBodyMark, FBBIKBase>();
    	}else{
    		map.Clear();
    	}
 
    	foreach(FBBIKBase mark in markList){
            Debug.Log("mark:"+mark.GetBodyID()+ ", "+ mark.GetWorldPosition());
    		map.Add(mark.GetBodyID(), mark);
    		mark.Init();
    		mark.SetCamera(cam);
    		mark.SetAvatar(avatarReferences[(int)mark.GetBodyID()]);
    	}
    }
    void LateUpdate(){
        Invoke("UpdateAimIK", 0.0f);
    }
    public void DoVRIK(){
        ik.GetIKSolver().Update();
    }
    private void UpdateAimIK(){
       aimIK.GetIKSolver().Update();
    }
    public void InitMarks(){
    	foreach(FBBIKBase mark in map.Values){
    		mark.InitPosition();
    	}
    }
    public Transform GetModelRoot(){
        return model;
    }
    public Vector3 GetWorldPosition(MyUtility.FullBodyMark mark){
        Debug.Log("getworldposition:"+mark+" "+map.ContainsKey(mark));
    	if(map.ContainsKey(mark)){
    		return map[mark].GetWorldPosition();
    	}
    	return Vector3.zero;
    }
    public Vector3 GetPosition(MyUtility.FullBodyMark mark){
    	if(map.ContainsKey(mark)){
    		return map[mark].GetPosition();
    	}
    	return Vector3.zero;
    }
    public Vector3[] GetPositions(){
    	Vector3[] pos = new Vector3[Enum.GetNames(typeof(MyUtility.FullBodyMark)).Length];

    	foreach(FBBIKBase mark in map.Values){
    		pos[(int)mark.GetBodyID()] = mark.GetPosition();
    	}
    	return pos;
    }

    public Vector3 GetInitPosition(MyUtility.FullBodyMark mark){
    	if(map.ContainsKey(mark)){
    		return map[mark].GetInitPosition();
    	}
    	return Vector3.zero;
    }

    public Quaternion[] GetRotations(){
	 	Quaternion[] rots = new Quaternion[Enum.GetNames(typeof(MyUtility.FullBodyMark)).Length];

		return rots;   	
    }

    public GameObject GetObj(MyUtility.FullBodyMark mark){
     	if(map.ContainsKey(mark)){
    		return map[mark].gameObject;
    	}
    	return null;   	
    }

    public void SetPose(Vector3[] pos, Quaternion[] rot){
    	for(int i = 0 ; i < pos.Length ; i++){
    		MyUtility.FullBodyMark mark = (MyUtility.FullBodyMark)i;
    		if (map.ContainsKey(mark)){
    			map[mark].SetPosition(pos[i]);
    		}
    	}
    }

}
