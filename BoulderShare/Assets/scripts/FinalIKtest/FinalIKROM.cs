using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class FinalIKROM : MonoBehaviour
{

    [System.Serializable]
    public enum FullBodyMark {
        Body = 0,
        LeftShoulder,
        RightShoulder,
        LeftThigh,
        RightThigh,
        LeftHand,
        RightHand,
        LeftFoot,
        RightFoot,
        LeftElbow,
        RightElbow,
        LeftKnee,
        RightKnee,
        Head
    }
    [SerializeField] private FinalIKMarkBody body;
	[SerializeField] private FinalIKMarkFoot leftFoot;
	[SerializeField] private FinalIKMarkFoot rightFoot;
	[SerializeField] private FinalIKMarkHand leftHand;
	[SerializeField] private FinalIKMarkHand rightHand;
	[SerializeField] private FinalIKMarkBend leftElbow;
	[SerializeField] private FinalIKMarkBend rightElbow;
	[SerializeField] private FinalIKMarkBend leftKnee;
	[SerializeField] private FinalIKMarkBend rightKnee;
    [SerializeField] private FinalIKMarkPelvisAndShoulder leftShoulder;
    [SerializeField] private FinalIKMarkPelvisAndShoulder rightShoulder;
    [SerializeField] private FinalIKMarkPelvisAndShoulder leftThigh;
    [SerializeField] private FinalIKMarkPelvisAndShoulder rightThigh;
    [SerializeField] private FinalIKMarkHead head;
    [SerializeField] private bool isNeedInit;
    [SerializeField] private FullBodyBipedIK ik;
    [SerializeField] private AimIK aimIK;
    [SerializeField] private Transform modelCopy;
    [SerializeField] private Transform model;

    private bool isLookingActive = false;
    [SerializeField] private float diffLimit = 0.1f;
    void Awake(){
        aimIK.enabled = false;
    }

    void Update(){
        if(isNeedInit){
            InitAvatar();
            isNeedInit = false;
        }
    }

    void LateUpdate(){
        Invoke("ROM", 0.0f);
        Invoke("UpdateAimIK", 0.0f);
        Invoke("CorrectModelPos", 0.0f);
        Invoke("Trace", 0.0f);
    }

    //手足腰肩のtargetの位置を更新
    public void ModifyPositions(){
        leftFoot.ModifyPosition();
        rightFoot.ModifyPosition();
        leftHand.ModifyPosition();
        rightHand.ModifyPosition();
        leftShoulder.ModifyPosition();
        rightShoulder.ModifyPosition();
        leftThigh.ModifyPosition();
        rightThigh.ModifyPosition();
    }

    private void CorrectModelPos(){
        if (body.IsMoved() && CalcHFDiff() > diffLimit){
            CopyPos(model, modelCopy);
        }else{
            CopyPos(modelCopy, model);
        }
    }
    private void CopyPos(Transform to, Transform from){
        from.transform.position = to.transform.position;
        from.transform.rotation = to.transform.rotation;

        Transform[] childrenTo = to.GetComponentsInChildren<Transform>();
        Transform[] childrenFrom = from.GetComponentsInChildren<Transform>();

        for(int i = 0 ; i < childrenFrom.Length ; i++){
            childrenTo[i].position = childrenFrom[i].position;
            childrenTo[i].rotation = childrenFrom[i].rotation;
        }
    }

    public float CalcHFDiff(){
        float diff = 0.0f;

        diff += leftHand.CalcDiff();
        diff += rightHand.CalcDiff();
        diff += leftFoot.CalcDiff();
        diff += rightFoot.CalcDiff();

        return diff;
    }

    private void UpdateAimIK(){
        aimIK.GetIKSolver().Update();
    }

    public void InitAvatar(){
        InitMarks();
    }

    public bool IsEnabled(){
        return ik.enabled;
    }

    public Vector3 GetBodyPosition(){
        return body.GetWorldPosition();
    }

    public void InitMarks(){
        body.Init();
        leftFoot.Init();
        rightFoot.Init();
        leftHand.Init();
        rightHand.Init();
        leftElbow.Init();
        rightElbow.Init();
        leftKnee.Init();
        rightKnee.Init();
        leftShoulder.Init();
        rightShoulder.Init();
        leftThigh.Init();
        rightThigh.Init();
    }

    private void ROM(){
    	//rightThighRL.Apply();
    	//rightKneeRL.Apply();
    }

    private void Trace(){
        body.Correct();
    	leftFoot.Correct();
    	rightFoot.Correct();
    	leftHand.Correct();
    	rightHand.Correct();
    	leftElbow.Correct();
    	rightElbow.Correct();
    	leftKnee.Correct();
    	rightKnee.Correct();
        leftShoulder.Correct();
        rightShoulder.Correct();
        leftThigh.Correct();
        rightThigh.Correct();
    }

    public Vector3 GetPosition(FullBodyMark m){
        switch(m){
            case FullBodyMark.Body: return body.GetPosition();
            case FullBodyMark.LeftShoulder: return leftShoulder.GetPosition();
            case FullBodyMark.RightShoulder: return rightShoulder.GetPosition();
            case FullBodyMark.LeftThigh: return leftThigh.GetPosition();
            case FullBodyMark.RightThigh: return rightThigh.GetPosition();
            case FullBodyMark.LeftHand: return leftHand.GetPosition();
            case FullBodyMark.RightHand: return rightHand.GetPosition();
            case FullBodyMark.LeftFoot: return leftFoot.GetPosition();
            case FullBodyMark.RightFoot: return rightFoot.GetPosition();
            case FullBodyMark.LeftElbow: return leftElbow.GetPosition();
            case FullBodyMark.RightElbow: return rightElbow.GetPosition();
            case FullBodyMark.LeftKnee: return leftKnee.GetPosition();
            case FullBodyMark.RightKnee: return rightKnee.GetPosition();
            case FullBodyMark.Head: return head.GetPosition();
        }

        return Vector3.zero;
    }
    public Vector3[] GetPositions(){
        Vector3[] pos = new Vector3[Enum.GetNames(typeof(FullBodyMark)).Length];
        
        foreach(FullBodyMark value in Enum.GetValues(typeof(FullBodyMark))){
            pos[(int)value] = GetPosition(value);
        }
        return pos;
    }

    public void SetPosition(Vector3 p, FullBodyMark m){
        switch(m){
            case FullBodyMark.Body: body.SetPosition(p); break;
            case FullBodyMark.LeftShoulder: leftShoulder.SetPosition(p); break;
            case FullBodyMark.RightShoulder: rightShoulder.SetPosition(p); break;
            case FullBodyMark.LeftThigh: leftThigh.SetPosition(p); break;
            case FullBodyMark.RightThigh: rightThigh.SetPosition(p); break;
            case FullBodyMark.LeftHand: leftHand.SetPosition(p); break;
            case FullBodyMark.RightHand: rightHand.SetPosition(p); break;
            case FullBodyMark.LeftFoot: leftFoot.SetPosition(p); break;
            case FullBodyMark.RightFoot: rightFoot.SetPosition(p); break;
            case FullBodyMark.LeftElbow: leftElbow.SetPosition(p); break;
            case FullBodyMark.RightElbow: rightElbow.SetPosition(p); break;
            case FullBodyMark.LeftKnee: leftKnee.SetPosition(p); break;
            case FullBodyMark.RightKnee: rightKnee.SetPosition(p); break;
            case FullBodyMark.Head: head.SetPosition(p); break;
        }
    }

    public void SetPositions(Vector3[] pos){
        
        foreach(FullBodyMark value in Enum.GetValues(typeof(FullBodyMark))){
            SetPosition(pos[(int)value], value);
        }
    }

    public void SetIsLookingActive(bool b){
        head.SetIsLookingActive(b);
        if (b){
            aimIK.GetIKSolver().IKPositionWeight = 1.0f;
        }else{
            aimIK.GetIKSolver().IKPositionWeight = 0.0f;
        }
    }
    public bool IsLookingActive(){
        return head.IsLookingActive();
    }
}
