using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RootMotion.FinalIK;

public class FBBIKController : MonoBehaviour, IHumanModelController
{
	public enum HandAnim{Default, Kachi, Pinch, Pocket, Sloper};
	[SerializeField] private Animator animator;
	[SerializeField] private Transform[] avatarReferences;
    [SerializeField] private List<FBBIKBase> markList;
    [SerializeField] private Camera cam;
    private Dictionary<MyUtility.FullBodyMark, FBBIKBase> map;
    private Dictionary<MyUtility.FullBodyMark, FBBAimIKComponent> aimMap;
    private Dictionary<string, Transform> handPoseMap;
    [SerializeField] private BodyRotation bodyRotation;
    [SerializeField] private Transform lHPoseRoot;
    [SerializeField] private Transform rHPoseRoot;
    [SerializeField] private List<FBBAimIKComponent> aimIKComponents;
    [SerializeField] private List<FootPoser> footPoserList;
    [SerializeField] private VRIK ik;
    [SerializeField] private List<LegIK> legIKList;
    [SerializeField] private Transform model;
    [SerializeField] private LineRenderer hfLinePrefab;
    [SerializeField] private bool switchAimIKActivate = false;
    [SerializeField] private Vector3 initOffset;
    private HandAnim lHandAnim;
    private HandAnim rHandAnim;

    [SerializeField] private HandPoser lhp;
    [SerializeField] private HandPoser rhp;
    [SerializeField] private bool ikInit = false;


    public void Start(){
    	Init();
    	InitMarks();
    }

    public VRIK GetVRIK(){
        return ik;
    }
   
    public void Init(){
    	if (map == null){
    		map = new Dictionary<MyUtility.FullBodyMark, FBBIKBase>();
    	}else{
    		map.Clear();
    	}
    	if (aimMap == null){
    		aimMap = new Dictionary<MyUtility.FullBodyMark, FBBAimIKComponent>();
    	}else{
    		aimMap.Clear();
    	}

    	InitHandAnimator();
 
    	foreach(FBBIKBase mark in markList){
           // Debug.Log("mark:"+mark.GetBodyID()+ ", "+ mark.GetWorldPosition());
    		map.Add(mark.GetBodyID(), mark);
    		mark.SetCamera(cam);
    		mark.SetAvatar(avatarReferences[(int)mark.GetBodyID()]);
            mark.Init();
            //lineの設定
            if (mark.GetType() == typeof(FBBIKMarkHF)){
                LineRenderer line = Instantiate(hfLinePrefab, mark.transform) as LineRenderer;
                ((FBBIKMarkHF)mark).SetLine(line);
            }
    	}

    	foreach(FBBAimIKComponent com in aimIKComponents){
    		aimMap.Add(com.GetTargetAvatarBodyID(), com);
    		com.SetCamera(cam);
    		com.SetAvatar(avatarReferences[(int)com.GetTargetAvatarBodyID()]);
            com.Init();
    	}
    }

    public void InitHandAnimator(){
    	if (handPoseMap == null){
            handPoseMap = new Dictionary<string, Transform>();
    	}else{
            handPoseMap.Clear();
    	}

        string prefix = "L";

        handPoseMap.Add(prefix + HandAnim.Default.ToString(), lHPoseRoot.Find(HandAnim.Default.ToString()));
        handPoseMap.Add(prefix + HandAnim.Kachi.ToString(), lHPoseRoot.Find(HandAnim.Kachi.ToString()));
        handPoseMap.Add(prefix + HandAnim.Pinch.ToString(), lHPoseRoot.Find(HandAnim.Pinch.ToString()));
        handPoseMap.Add(prefix + HandAnim.Pocket.ToString(), lHPoseRoot.Find(HandAnim.Pocket.ToString()));
        handPoseMap.Add(prefix + HandAnim.Sloper.ToString(), lHPoseRoot.Find(HandAnim.Sloper.ToString()));

        prefix = "R";

        handPoseMap.Add(prefix + HandAnim.Default.ToString(), rHPoseRoot.Find(HandAnim.Default.ToString()));
        handPoseMap.Add(prefix + HandAnim.Kachi.ToString(), rHPoseRoot.Find(HandAnim.Kachi.ToString()));
        handPoseMap.Add(prefix + HandAnim.Pinch.ToString(), rHPoseRoot.Find(HandAnim.Pinch.ToString()));
        handPoseMap.Add(prefix + HandAnim.Pocket.ToString(), rHPoseRoot.Find(HandAnim.Pocket.ToString()));
        handPoseMap.Add(prefix + HandAnim.Sloper.ToString(), rHPoseRoot.Find(HandAnim.Sloper.ToString()));
    }

    public void ReInitIK(){
        ik.GetIKSolver().Initiate(ik.transform);
    }
    public void SetIKEnable(bool b){
        ik.enabled = b;
        rhp.enabled = b;
        lhp.enabled = b;
    }
    void Update(){

        if (ikInit){
            ikInit = false;
            ik.GetIKSolver().Initiate(ik.transform);
        }
    /*
    	foreach(FBBAimIKComponent com in aimIKComponents){
	    	if (switchAimIKActivate ^ com.IsActive()){
	    		if(com.IsActive()){
	    			com.Deactivate();
	    		}else{
	    			com.Activate();
	    		}
	    	}
    	}*/
        //BoundMark(hipCollider);
        //BoundMark(spineCollider);
    }

    public void ActiveMarks(){
    	foreach(FBBIKBase m in markList){
    		m.gameObject.SetActive(true);
    	}
        bodyRotation.gameObject.SetActive(true);
    }

    public void DeactiveMarks(){
    	foreach(FBBIKBase m in markList){
    		m.gameObject.SetActive(false);
    	}
        bodyRotation.gameObject.SetActive(false);
    }

    public void ActiveAimMark(MyUtility.FullBodyMark mark){
    	//Debug.Log("mark:"+mark);
        foreach(FootPoser poser in footPoserList){
            if (poser.GetTargetAvatarBodyID() == mark){
                poser.Show();
            }else{
                poser.Hide();
            }
        }
    	foreach(FBBAimIKComponent com in aimIKComponents){
    		if (com.GetTargetAvatarBodyID() == mark){
    			//Debug.Log("Active:"+com.GetTargetAvatarBodyID());
    			com.Activate();
    		}else{
    			//Debug.Log("Deactivate:"+com.GetTargetAvatarBodyID());
    			com.Deactivate();
    		}
    	}
    }

    public void DeactiveAimMarks(){
    	foreach(FBBAimIKComponent com in aimIKComponents){
    		com.Deactivate();
    	}

        foreach(FootPoser poser in footPoserList){
            poser.Hide();
        }
    }

    public void AddOnPostBeginDragAction(Action a){
        if (a != null){
            foreach(FBBIKBase m in markList){
                m.AddOnPostBeginDragAction(a);
            }

            foreach(FBBAimIKComponent com in aimIKComponents){
            	com.AddOnPostBeginDragAction(a);
            }
        }
    }
    public void RemoveOnPostBeginDragAction(Action a){
        if (a != null){
            foreach(FBBIKBase m in markList){
                m.RemoveOnPostBeginDragAction(a);
            }
            foreach(FBBAimIKComponent com in aimIKComponents){
            	com.RemoveOnPostBeginDragAction(a);
            }
        }
    }
/*
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
    }*/
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
        bodyRotation.FitTargetInAvatar();
    }
    public void DoVRIK(){
        ik.GetIKSolver().Update();
        LateUpdate();
    }
    private void UpdateAimIK(){

    	foreach(FBBAimIKComponent com in aimIKComponents){
    		com.UpdateManually();
    	}/*
       aimIK.GetIKSolver().Update();
       if (aimLHIK != null){
        aimLHIK.GetIKSolver().Update();
       }*/
    }
    public void InitMarks(){
    	foreach(FBBIKBase mark in map.Values){
    		mark.InitPosition(initOffset);
    	}

    	foreach(FBBAimIKComponent com in aimIKComponents){
            com.Reset();
    	}

    	SetHandAnim(HandAnim.Default, true);
    	SetHandAnim(HandAnim.Default, false);

        model.localPosition = initOffset;
        model.localRotation = Quaternion.identity;

    }
    public Transform GetModelRoot(){
        return model;
    }
   	public Transform GetAvatar(MyUtility.FullBodyMark mark){
   		int index = (int)mark;

   		if (index >= 0 && index < avatarReferences.Length){
   			return avatarReferences[index];
   		}

   		return null;
   	}
    public Vector3 GetWorldPosition(MyUtility.FullBodyMark mark){
        if (mark == MyUtility.FullBodyMark.Body){
            return model.position;
        }
        //Debug.Log("getworldposition:"+mark+" "+map.ContainsKey(mark));
    	if(map.ContainsKey(mark)){
    		return map[mark].GetWorldPosition();
    	}
    	return Vector3.zero;
    }
    public Vector3 GetPosition(MyUtility.FullBodyMark mark){
        if (mark == MyUtility.FullBodyMark.Body){
            return model.localPosition;
        }
    	if(map.ContainsKey(mark)){
    		return map[mark].GetPosition();
    	}
    	return Vector3.zero;
    }
    public Vector3[] GetPositions(){
    	Vector3[] pos = new Vector3[Enum.GetNames(typeof(MyUtility.FullBodyMark)).Length];
        pos[(int)MyUtility.FullBodyMark.Body] = model.localPosition;

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
        rots[(int)MyUtility.FullBodyMark.Body] = model.localRotation;
        foreach(FBBIKBase mark in map.Values){
            if(mark.GetType() == typeof(FBBIKMarkFoot)){
                rots[(int)mark.GetBodyID()] = ((FBBIKMarkFoot)mark).GetRotation();
            }
        }
	 	foreach(FBBAimIKComponent com in aimMap.Values){
	 		rots[(int)com.GetTargetAvatarBodyID()] = com.GetRotation();
	 	}
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

                if (map[mark].GetType() == typeof(FBBIKMarkFoot)){
                    ((FBBIKMarkFoot)map[mark]).SetRotation(rot[i]);
                }
    		}

    		if (aimMap.ContainsKey(mark)){
    			aimMap[mark].SetRotation(rot[i]);
    		}
    	}
        model.localPosition = pos[(int)MyUtility.FullBodyMark.Body];
        model.localRotation = rot[(int)MyUtility.FullBodyMark.Body];
    }

    public void SetHandAnim(HandAnim hand, bool isRight){
    	Debug.Log("SetHand:"+hand+" "+isRight);
    	string key = "";

    	if (isRight){
    		key = "R";
    	}else{
    		key = "L";
    	}

        key += hand.ToString();

    	if(handPoseMap.ContainsKey(key)){
    		if (isRight){
                rhp.poseRoot = handPoseMap[key];
    			rHandAnim = hand;
    		}else{
                lhp.poseRoot = handPoseMap[key];
    			lHandAnim = hand;
    		}
    	}
    }

    public HandAnim GetHandAnim(bool isRight){
    	if (isRight){
    		return rHandAnim;
    	}else{
    		return lHandAnim;
    	}
    }
}
