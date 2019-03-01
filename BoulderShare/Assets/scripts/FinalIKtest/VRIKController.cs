using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class VRIKController : MonoBehaviour
{

    [System.Serializable]
    public enum FullBodyMark {
        Body = 0,
        Chest,
        Pelvis,
        LeftHand,
        RightHand,
        LeftFoot,
        RightFoot,
        LeftElbow,
        RightElbow,
        LeftKnee,
        RightKnee,
        Head,
        Look
    }
    [SerializeField] private Transform body;
    [SerializeField] private Transform avatarBody;
	[SerializeField] private VRIKFoot leftFoot;
	[SerializeField] private VRIKFoot rightFoot;
	[SerializeField] private VRIKHand leftHand;
	[SerializeField] private VRIKHand rightHand;
	[SerializeField] private VRIKComponent leftElbow;
	[SerializeField] private VRIKComponent rightElbow;
	[SerializeField] private VRIKComponent leftKnee;
	[SerializeField] private VRIKComponent rightKnee;
    [SerializeField] private VRIKComponent chest;
    [SerializeField] private VRIKPelvis pelvis;
    [SerializeField] private VRIKHead head;
    [SerializeField] private VRIKComponent look;
    [SerializeField] private bool isNeedInit;
    [SerializeField] private VRIK ik;
    [SerializeField] private AimIK aimIK;

    private bool isLookingActive = false;

    void Awake(){
        aimIK.enabled = false;
    }

    void Update(){
        if(isNeedInit){
            InitAvatar();
            isNeedInit = false;
        }
    }

    //頭とmodel本体の座標とのオフセット
    public Vector3 GetOffsetFromRoot(FullBodyMark m){
        switch(m){
            case FullBodyMark.Body: return Vector3.zero;
            case FullBodyMark.Chest: return chest.GetOffset();
            case FullBodyMark.Pelvis: return pelvis.GetOffset();
            case FullBodyMark.LeftHand: return leftHand.GetOffset();
            case FullBodyMark.RightHand: return rightHand.GetOffset();
            case FullBodyMark.LeftFoot: return leftFoot.GetOffset();
            case FullBodyMark.RightFoot: return rightFoot.GetOffset();
            case FullBodyMark.LeftElbow: return leftElbow.GetOffset();
            case FullBodyMark.RightElbow: return rightElbow.GetOffset();
            case FullBodyMark.LeftKnee: return leftKnee.GetOffset();
            case FullBodyMark.RightKnee: return rightKnee.GetOffset();
            case FullBodyMark.Head: return head.GetOffset();
            case FullBodyMark.Look: return look.GetOffset();
        }
        return Vector3.zero;
    }

    public void DoVRIK(){
        ik.GetIKSolver().Update();
    }

    void LateUpdate(){
        Invoke("UpdateAimIK", 0.0f);
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
        return avatarBody.position;
    }

    public void InitMarks(){
    	body.localPosition = Vector3.zero;
        leftFoot.Init();
        rightFoot.Init();
        leftHand.Init();
        rightHand.Init();
        leftElbow.Init();
        rightElbow.Init();
        leftKnee.Init();
        rightKnee.Init();
        chest.Init();
        pelvis.Init();
        head.Init();
        look.Init();
    }

    public GameObject GetObj(FullBodyMark m){
        switch(m){
            case FullBodyMark.Body: return body.gameObject;
            case FullBodyMark.Chest: return chest.gameObject;
            case FullBodyMark.Pelvis: return pelvis.gameObject;
            case FullBodyMark.LeftHand: return leftHand.gameObject;
            case FullBodyMark.RightHand: return rightHand.gameObject;
            case FullBodyMark.LeftFoot: return leftFoot.gameObject;
            case FullBodyMark.RightFoot: return rightFoot.gameObject;
            case FullBodyMark.LeftElbow: return leftElbow.gameObject;
            case FullBodyMark.RightElbow: return rightElbow.gameObject;
            case FullBodyMark.LeftKnee: return leftKnee.gameObject;
            case FullBodyMark.RightKnee: return rightKnee.gameObject;
            case FullBodyMark.Head: return head.gameObject;
            case FullBodyMark.Look: return look.gameObject;
        }

        return null;
    }

    public Vector3 GetPosition(FullBodyMark m){
        switch(m){
            case FullBodyMark.Body: return body.localPosition;
            case FullBodyMark.Chest: return chest.GetPosition();
            case FullBodyMark.Pelvis: return pelvis.GetPosition();
            case FullBodyMark.LeftHand: return leftHand.GetPosition();
            case FullBodyMark.RightHand: return rightHand.GetPosition();
            case FullBodyMark.LeftFoot: return leftFoot.GetPosition();
            case FullBodyMark.RightFoot: return rightFoot.GetPosition();
            case FullBodyMark.LeftElbow: return leftElbow.GetPosition();
            case FullBodyMark.RightElbow: return rightElbow.GetPosition();
            case FullBodyMark.LeftKnee: return leftKnee.GetPosition();
            case FullBodyMark.RightKnee: return rightKnee.GetPosition();
            case FullBodyMark.Head: return head.GetPosition();
            case FullBodyMark.Look: return look.GetPosition();
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
            case FullBodyMark.Body: body.localPosition = p; break;
            case FullBodyMark.Chest: chest.SetPosition(p); break;
            case FullBodyMark.Pelvis: pelvis.SetPosition(p); break;
            case FullBodyMark.LeftHand: leftHand.SetPosition(p); break;
            case FullBodyMark.RightHand: rightHand.SetPosition(p); break;
            case FullBodyMark.LeftFoot: leftFoot.SetPosition(p); break;
            case FullBodyMark.RightFoot: rightFoot.SetPosition(p); break;
            case FullBodyMark.LeftElbow: leftElbow.SetPosition(p); break;
            case FullBodyMark.RightElbow: rightElbow.SetPosition(p); break;
            case FullBodyMark.LeftKnee: leftKnee.SetPosition(p); break;
            case FullBodyMark.RightKnee: rightKnee.SetPosition(p); break;
            case FullBodyMark.Head: head.SetPosition(p); break;
            case FullBodyMark.Look: look.SetPosition(p); break;
        }
    }

    public void SetPositions(Vector3[] pos){
        
        foreach(FullBodyMark value in Enum.GetValues(typeof(FullBodyMark))){
            SetPosition(pos[(int)value], value);
        }
    }

    public Quaternion GetRotation(FullBodyMark m){
        switch(m){
            case FullBodyMark.Body: return body.localRotation;
            case FullBodyMark.Chest: return chest.GetRotation();
            case FullBodyMark.Pelvis: return pelvis.GetRotation();
            case FullBodyMark.LeftHand: return leftHand.GetRotation();
            case FullBodyMark.RightHand: return rightHand.GetRotation();
            case FullBodyMark.LeftFoot: return leftFoot.GetRotation();
            case FullBodyMark.RightFoot: return rightFoot.GetRotation();
            case FullBodyMark.LeftElbow: return leftElbow.GetRotation();
            case FullBodyMark.RightElbow: return rightElbow.GetRotation();
            case FullBodyMark.LeftKnee: return leftKnee.GetRotation();
            case FullBodyMark.RightKnee: return rightKnee.GetRotation();
            case FullBodyMark.Head: return head.GetRotation();
            case FullBodyMark.Look: return look.GetRotation();
        }

        return Quaternion.identity;
    }
    public Quaternion[] GetRotations(){
        Quaternion[] rots = new Quaternion[Enum.GetNames(typeof(FullBodyMark)).Length];
        
        foreach(FullBodyMark value in Enum.GetValues(typeof(FullBodyMark))){
            rots[(int)value] = GetRotation(value);
        }
        return rots;
    }

    public void SetRotation(Quaternion rot, FullBodyMark m){
        switch(m){
            case FullBodyMark.Body: body.localRotation = rot; break;
            case FullBodyMark.Chest: chest.SetRotation(rot); break;
            case FullBodyMark.Pelvis: pelvis.SetRotation(rot); break;
            case FullBodyMark.LeftHand: leftHand.SetRotation(rot); break;
            case FullBodyMark.RightHand: rightHand.SetRotation(rot); break;
            case FullBodyMark.LeftFoot: leftFoot.SetRotation(rot); break;
            case FullBodyMark.RightFoot: rightFoot.SetRotation(rot); break;
            case FullBodyMark.LeftElbow: leftElbow.SetRotation(rot); break;
            case FullBodyMark.RightElbow: rightElbow.SetRotation(rot); break;
            case FullBodyMark.LeftKnee: leftKnee.SetRotation(rot); break;
            case FullBodyMark.RightKnee: rightKnee.SetRotation(rot); break;
            case FullBodyMark.Head: head.SetRotation(rot); break;
            case FullBodyMark.Look: look.SetRotation(rot); break;
        }
    }

    public void SetRotations(Quaternion[] rots){
        
        foreach(FullBodyMark value in Enum.GetValues(typeof(FullBodyMark))){
            SetRotation(rots[(int)value], value);
        }
    }
    public void SetIsLookingActive(bool b){
        look.gameObject.SetActive(b);
        if (b){
            //aimIK.GetIKSolver().IKPositionWeight = 1.0f;
        }else{
            //aimIK.GetIKSolver().IKPositionWeight = 0.0f;
        }
    }
    public bool IsLookingActive(){
        return look.gameObject.activeSelf;
    }

    public void HideAllFO(){
       chest.gameObject.SetActive(false);
       leftElbow.gameObject.SetActive(false);
       rightElbow.gameObject.SetActive(false);
       leftKnee.gameObject.SetActive(false);
       rightKnee.gameObject.SetActive(false);
       look.gameObject.SetActive(false);
    }
}
