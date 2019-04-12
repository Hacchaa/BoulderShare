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
    [SerializeField] private LineRenderer hfLinePrefab;
    [SerializeField] private BoxCollider hipCollider;
    [SerializeField] private BoxCollider spineCollider;

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
           // Debug.Log("mark:"+mark.GetBodyID()+ ", "+ mark.GetWorldPosition());
    		map.Add(mark.GetBodyID(), mark);
    		mark.Init();
    		mark.SetCamera(cam);
    		mark.SetAvatar(avatarReferences[(int)mark.GetBodyID()]);

            //lineの設定
            if (mark.GetType() == typeof(FBBIKMarkHF)){
                LineRenderer line = Instantiate(hfLinePrefab, mark.transform) as LineRenderer;
                ((FBBIKMarkHF)mark).SetLine(line);
            }
    	}
    }
    void Update(){
        //BoundMark(hipCollider);
        //BoundMark(spineCollider);
    }

    private void BoundMark(BoxCollider bodyCollider){
        Transform bodyTrans = bodyCollider.transform;
        Quaternion localRot = bodyTrans.localRotation;
        bodyTrans.localRotation = Quaternion.identity;
        Vector3 boxSize = bodyCollider.bounds.size;
        bodyTrans.localRotation = localRot;
        Vector3 boxCenter = bodyCollider.bounds.center;
        //Debug.Log("boxCenter:"+boxCenter + " boxSize:"+boxSize);
        Collider[] hitColliders = Physics.OverlapBox(boxCenter, boxSize/2.0f, bodyTrans.localRotation);

        for(int i = 0 ; i < hitColliders.Length ; i++){
            if (!gameObject.name.Equals(hitColliders[i].gameObject.name)){
                //Debug.Log("hit:"+hitColliders[i].gameObject.name+ ",pos:"+hitColliders[i].transform.position);
                FBBIKMarkHF ikMark = hitColliders[i].gameObject.GetComponent<FBBIKMarkHF>();
                if (ikMark != null){
                    //外→表面
                    Vector3 colPoint = bodyCollider.ClosestPoint(ikMark.GetWorldPosition());

                    //内→表面
                    Vector3 colPointLocal = bodyTrans.InverseTransformPoint(colPoint);
                    Vector3 bCenterLocal = bodyTrans.InverseTransformPoint(boxCenter);
                    Vector3 dif = colPointLocal - bCenterLocal;
                    //Debug.Log("colPointLocal:"+colPointLocal);
                    //Debug.Log("bCenterLocal:"+bCenterLocal);
                    //Debug.Log("dif:"+dif);
                    float[] v = new float[3];
                    v[0] = Mathf.Abs(Mathf.Abs(dif.x) - boxSize.x / bodyTrans.localScale.x / 2);
                    v[1] = Mathf.Abs(Mathf.Abs(dif.y) - boxSize.y / bodyTrans.localScale.y / 2);
                    v[2] = Mathf.Abs(Mathf.Abs(dif.z) - boxSize.z / bodyTrans.localScale.z / 2);
                    //Debug.Log("v:"+v[0]+" "+v[1]+" "+v[2]);
                    float min = Mathf.Min(v);
                    float inv = 1f;

                    if (min == v[0]){
                        if (dif.x < 0){
                            inv *= -1;
                        }
                        colPoint = bodyTrans.TransformPoint(new Vector3(bCenterLocal.x + (boxSize.x / bodyTrans.localScale.x / 2 * inv), colPointLocal.y, colPointLocal.z));

                    }else if(min == v[1]){
                        if (dif.y < 0){
                            inv *= -1;
                        }
                        colPoint = bodyTrans.TransformPoint(new Vector3(colPointLocal.x, bCenterLocal.y + (boxSize.y / bodyTrans.localScale.y / 2 * inv), colPointLocal.z));

                    }else if(min == v[2]){
                        if (dif.z < 0){
                            inv *= -1;
                        }
                        colPoint = bodyTrans.TransformPoint(new Vector3(colPointLocal.x, colPointLocal.y, bCenterLocal.z + (boxSize.z / bodyTrans.localScale.z / 2 * inv)));
                    }
                    Vector3 dir = (colPoint - ikMark.GetWorldPosition()).normalized;
                    if ((boxCenter - ikMark.GetWorldPosition()).sqrMagnitude > (boxCenter - colPoint).sqrMagnitude){
                            dir *= -1;
                    }
                    ikMark.SetWorldPosition(colPoint + dir * ikMark.GetR());
                    //ikMark.GetWorldPosition() = colPoint;
                    //Debug.Log("colPoint:"+colPoint + ", newPos:"+ikMark.GetWorldPosition() + "R:"+src.GetR());
                    //Debug.Break();
                }
            }
        }
    }
    void LateUpdate(){
        Invoke("UpdateAimIK", 0.0f);
        Invoke("ApplyRotationLimits", 0.0f);
        Invoke("CorrectPositions", 0.0f);
    }
    private void ApplyRotationLimits(){
        foreach(FBBIKBase mark in map.Values){
            if (mark.GetType() == typeof(FBBIKMarkSP)){
                ((FBBIKMarkSP)mark).ApplyRotationLimit();
            }
        }        
    }
    private void CorrectPositions(){
        foreach(FBBIKBase mark in map.Values){
            if (mark != null){
                mark.CorrectPosition();
            }
        }
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
        //Debug.Log("getworldposition:"+mark+" "+map.ContainsKey(mark));
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
    public FBBIKBase GetMark(MyUtility.FullBodyMark mark){
        if(map.ContainsKey(mark)){
            return map[mark];
        }
        return null;    
    }

    public IEnumerable<FBBIKBase> GetMarks(){
        return map.Values;
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
