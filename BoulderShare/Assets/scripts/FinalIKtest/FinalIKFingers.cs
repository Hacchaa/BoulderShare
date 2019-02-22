using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalIKFingers : MonoBehaviour
{
	[SerializeField] private FingerPose avatarFingers;
	[SerializeField] private FingerPose[] poses;
	[SerializeField] private int poseIndex = -1;
    // Start is called before the first frame update
    void Start()
    {
    	poseIndex = -1;
    }

    void LateUpdate(){
    	Invoke("ChoicePose", 0.0f);
    } 

    public void SetPoseIndex(int index){
    	if (index < 0 || index > poses.Length - 1){
    		poseIndex = -1;
    		return ;
    	}
    	poseIndex = index;
    }

    private void ChoicePose(){
    	if (poseIndex < 0 || poseIndex > poses.Length - 1){
    		return ;
    	}
    	CopyRotRecursively(avatarFingers.thumb, poses[poseIndex].thumb);
    	CopyRotRecursively(avatarFingers.index, poses[poseIndex].index);
    	CopyRotRecursively(avatarFingers.middle, poses[poseIndex].middle);
    	CopyRotRecursively(avatarFingers.ring, poses[poseIndex].ring);
    	CopyRotRecursively(avatarFingers.little, poses[poseIndex].little);

    }

    private void CopyRotRecursively(Transform to, Transform from){
    	to.localRotation = from.localRotation;
    	if(from.childCount > 0){
    		CopyRotRecursively(to.GetChild(0), from.GetChild(0));
    	}
    }

    [System.Serializable]
    public class FingerPose{
    	[SerializeField] private string name;
    	public Transform thumb;
    	public Transform index;
    	public Transform middle;
    	public Transform ring;
    	public Transform little;

    	public FingerPose(){

    	}

    	public string GetName(){
    		return name;
    	}
    }
}
