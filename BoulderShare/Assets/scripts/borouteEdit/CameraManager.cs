using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
public class CameraManager : MonoBehaviour
{
	[SerializeField] private CanvasGroup fadeCanvas2D;
	[SerializeField] private CanvasGroup fadeCanvas3D;
	[SerializeField] private float fadeDuration = 0.4f;
    [SerializeField] private Transform root3D;
    [SerializeField] private Transform move3D;
    [SerializeField] private Transform depth3D;
    [SerializeField] private List<Camera> cameras3D;
    [SerializeField] private List<Camera> cameras2D;
    [SerializeField] private Camera camera2D;
    [SerializeField] private Transform camera3D;
    [SerializeField] private Transform root2D;
    [SerializeField] private float duration = 0.3f;
    [SerializeField] private HumanModel humanModel;
    [SerializeField] private Camera ss2DCameraFrom;
    [SerializeField] private Camera ss2DCameraTo;
    [SerializeField] private Camera ss3DCameraFrom;
    [SerializeField] private Camera ss3DCameraTo;

/*
    private const float CAMERA2D_DEPTH_UB = -1.0f;
    private const float CAMERA2D_DEPTH_LB = -15.0f;
    private const float CAMERA2D_DEPTH_DEF = -10.0f;
    private const float CAMERA3D_DEPTH_UB = -2.0f;
    private const float CAMERA3D_DEPTH_LB = -15.0f;
    public const float CAMERA3D_DEPTH_DEF = -10.0f;
    private const float CAMERA3D_MOVEZ = 4.0f;
    public const float CAMERA3D_DEPTH_LOOKING = -5.0f;*/
    private const float CAMERA3D_MOVEZ = 4.0f;
    public const float CAMERA_DEPTH = -10.0f;

    private const float FOV2D_MIN = 10.0f;
    private const float FOV2D_MAX = 110.0f;
    public const float FOV2D_DEF = 60.0f;
    private const float FOV3D_MIN = 2.0f;
    private const float FOV3D_MAX = 60.0f;
    public const float FOV3D_DEF = 45.0f;
	
	[SerializeField] private bool isFadeOut2D = false;
	[SerializeField] private bool isFadeIn2D = false;
	[SerializeField] private bool isFadeOut3D = false;
	[SerializeField] private bool isFadeIn3D = false;
	[SerializeField] private bool swithAnim = false;
	[SerializeField] private bool fadeOutIn2D = false;
	[SerializeField] private bool fadeOutIn3D = false;
    [SerializeField] private bool isRight = false;
    [SerializeField] private float dirLength = 1.0f;

	void Update(){
		if (fadeOutIn3D){
			fadeOutIn3D = false;
			FadeOutIn3DWithAnimation(isRight, GetRootWorldPos());
		}
		if (fadeOutIn2D){
			fadeOutIn2D = false;
			FadeOutIn2DWithAnimation(isRight);
		}
		if (swithAnim){
			swithAnim = false;
			SwitchDimWithAnimation(isRight, GetRootWorldPos());
		}
		if (isFadeOut2D){
			FadeOut2D(isRight);
			isFadeOut2D = false;
		}

		if (isFadeIn2D){
			FadeIn2D(isRight);
			isFadeIn2D = false;
		}
		if (isFadeOut3D){
			FadeOut3D(isRight);
			isFadeOut3D = false;
		}

		if (isFadeIn3D){
			FadeIn3D(isRight, GetRootWorldPos());
			isFadeIn3D = false;
		}
	}

    public void DontShow(){
        camera3D.gameObject.SetActive(false);
        camera2D.gameObject.SetActive(false);
    }

	public void FadeOutIn2DWithAnimation(bool isRightDir){
		Sequence seq = DOTween.Sequence();
		seq.Append(GetFadeOut2DSeq(isRightDir))
		.Append(GetFadeIn2DSeq(!isRightDir))
		.Play();
	}
    public Sequence GetFadeOutIn2DSeq(bool isRightDir){
        Sequence seq = DOTween.Sequence();
        seq.Append(GetFadeOut2DSeq(isRightDir))
        .Append(GetFadeIn2DSeq(!isRightDir));
        
        return seq;
    }
	public void FadeOutIn3DWithAnimation(bool isRightDir, Vector3 pos){
		Sequence seq = DOTween.Sequence();
		seq.Append(GetFadeOut3DSeq(isRightDir))
		.Append(GetFadeIn3DSeq(!isRightDir, pos))
		.Play();
	}
    public Sequence GetFadeOutIn3DSeq(bool isRightDir, Vector3 pos){
        Sequence seq = DOTween.Sequence();
        seq.Append(GetFadeOut3DSeq(isRightDir))
        .Append(GetFadeIn3DSeq(!isRightDir, pos));

        return seq;
    }

	public void SwitchDimWithAnimation(bool isRightDir, Vector3 pos){
		Sequence seq = DOTween.Sequence();
		if (Is2DActive()){
			seq.Append(GetFadeOut2DSeq(isRightDir))
			.Append(GetFadeIn3DSeq(!isRightDir, pos))
			.InsertCallback(
				fadeDuration, 
				() => {
					Active3D();
				}
			);
		}else{
			seq.Append(GetFadeOut3DSeq(isRightDir))
			.Append(GetFadeIn2DSeq(!isRightDir))
			.InsertCallback(
				fadeDuration, 
				() => {
					Active2D();
				}
			);			
		}

		seq.Play();
	}

    public Sequence GetSwitchDimSeq(bool isRightDir, Vector3 pos){
        Sequence seq = DOTween.Sequence();
        if (Is2DActive()){
            seq.Append(GetFadeOut2DSeq(isRightDir))
            .Append(GetFadeIn3DSeq(!isRightDir, pos))
            .InsertCallback(
                fadeDuration, 
                () => {
                    Active3D();
                }
            );
        }else{
            seq.Append(GetFadeOut3DSeq(isRightDir))
            .Append(GetFadeIn2DSeq(!isRightDir))
            .InsertCallback(
                fadeDuration, 
                () => {
                    Active2D();
                }
            );          
        }

        return seq;
    }

    public List<Camera> GetCameras(){
        List<Camera> list = new List<Camera>();
        list.AddRange(cameras3D);
        list.AddRange(cameras2D);
        return list;
    }

    public void Set2DCamPos(Vector3 v){
        root2D.transform.position = v;
    }

    public void Rotate3DWithAnim(Quaternion rot){
        Sequence seq = DOTween.Sequence();
    	seq.Append(root3D.DORotateQuaternion(rot, duration).SetEase(Ease.OutQuad));
        seq.Play();
    }

    public void Transform3DWithAnim(Vector3 vec, Quaternion rot, float fov, Vector3 movePos, float dur = 0.0f){
        if (dur == 0.0f){
            dur = duration;
        }
        //float dep = -d;
        //Vector3 v = new Vector3(vec.x, vec.y, 0.0f);

        Sequence seq = DOTween.Sequence();

        seq.Append(root3D.DORotateQuaternion(rot, dur).SetEase(Ease.OutQuad))
        .Join(root3D.DOMove(vec, dur).SetEase(Ease.OutQuad))
        .Join(move3D.DOLocalMove(movePos, dur).SetEase(Ease.OutQuad));
        foreach(Camera c in cameras3D){
            seq.Join(c.DOFieldOfView(fov, dur).SetEase(Ease.OutQuad));
        }
        //.Join(depth3D.DOLocalMoveZ(dep, dur).SetEase(Ease.OutQuad));

        seq.Play();
    }
    public void Transform3DWithAnim(Vector3 vec, Quaternion rot, float fov, float dur = 0.0f){
        Transform3DWithAnim(vec, rot, fov, Vector3.zero, dur);
    }

    public Sequence GetFadeOut2DSeq(bool isRightDir){
        //Debug.Log("GetFadeOut2DSeq");
        float dir = dirLength;
        if (isRightDir){
            dir *= -1.0f;
        }
    	Sequence sequence = DOTween.Sequence()
	        .OnStart(() =>
	        {
	            fadeCanvas2D.alpha = 0.0f;
                fadeCanvas3D.alpha = 0.0f;
	            fadeCanvas2D.blocksRaycasts = true;
	        })
	        .Append(fadeCanvas2D.DOFade(1.0f, fadeDuration).SetEase(Ease.InQuad))
	        .Join(root2D.transform.DOMoveX(dir, fadeDuration).SetEase(Ease.OutQuad).SetRelative())
	        .OnComplete(() =>
	        {
	        	fadeCanvas2D.blocksRaycasts = false;
	        });
	    
	    return sequence;
    }

    public Sequence GetFadeIn2DSeq(bool isRightDir){
        //Debug.Log("GetFadeIn2DSeq");
        float dir = -dirLength;
        if (isRightDir){
            dir *= -1.0f;
        }
 	    Sequence sequence = DOTween.Sequence()
	        .OnStart(() =>
	        {
	            fadeCanvas2D.alpha = 1.0f;
                fadeCanvas3D.alpha = 0.0f;
	            fadeCanvas2D.blocksRaycasts = true;
	            root2D.transform.localPosition = new Vector3(-dir, 0.0f, CAMERA_DEPTH);
                Reset2DFOV();
	        })
	        .Append(fadeCanvas2D.DOFade(0.0f, fadeDuration).SetEase(Ease.OutQuad))
	        .Join(root2D.transform.DOMoveX(dir, fadeDuration).SetEase(Ease.OutQuad).SetRelative())
	        .OnComplete(() =>
	        {
	        	fadeCanvas2D.blocksRaycasts = false;
                fadeCanvas2D.alpha = 0.0f;
                fadeCanvas3D.alpha = 0.0f;
	        });
	    
	    return sequence;   	
    }

    public void FadeOut2D(bool isRightDir){
	    Sequence sequence = GetFadeOut2DSeq(isRightDir);
	    sequence.Play();
    }

    public void FadeIn2D(bool isRightDir){
	    Sequence sequence = GetFadeIn2DSeq(isRightDir);

	    sequence.Play();
    }

    public Sequence GetFadeOut3DSeq(bool isRightDir){
        //Debug.Log("GetFadeOut3DSeq");
        float dir = dirLength;
        if (isRightDir){
            dir *= -1.0f;
        }        
	    Sequence sequence = DOTween.Sequence()
	        .OnStart(() =>
	        {
                fadeCanvas2D.alpha = 0.0f;
	            fadeCanvas3D.alpha = 0.0f;
	            fadeCanvas3D.blocksRaycasts = true;
	        })
	        .Append(fadeCanvas3D.DOFade(1.0f, fadeDuration).SetEase(Ease.InQuad))
	        .Join(move3D.transform.DOLocalMoveX(dir, fadeDuration).SetEase(Ease.OutQuad).SetRelative())    	
			.OnComplete(() =>
	        {
	        	fadeCanvas3D.blocksRaycasts = false;
	        });
	    
	    return sequence;
	}

	public Sequence GetFadeIn3DSeq(bool isRightDir, Vector3 pos){
        //Debug.Log("GetFadeIn3DSeq");
        float dir = -dirLength;
        if (isRightDir){
            dir *= -1.0f;
        }
	    Sequence sequence = DOTween.Sequence()
	        .OnStart(() =>
	        {
                fadeCanvas2D.alpha = 0.0f;
	            fadeCanvas3D.alpha = 1.0f;
	            fadeCanvas3D.blocksRaycasts = true;

                Reset3DCamPosAndDepth();
                SetRootWorldPos(pos);
                //Set3DDepth(CAMERA3D_DEPTH_LOOKING);
                move3D.transform.localPosition = new Vector3(-dir, 0.0f, 0.0f);
	        })
	        .Append(fadeCanvas3D.DOFade(0.0f, fadeDuration).SetEase(Ease.OutQuad))
	        .Join(move3D.transform.DOMoveX(dir, fadeDuration).SetEase(Ease.OutQuad).SetRelative())
	        .OnComplete(() =>
	        {
	        	fadeCanvas3D.blocksRaycasts = false;
                fadeCanvas3D.alpha = 0.0f;
                fadeCanvas2D.alpha = 0.0f;
	        });
	
		return sequence;		
	}

    public void FadeOut3D(bool isRightDir){
	    Sequence sequence = GetFadeOut3DSeq(isRightDir);
	    sequence.Play();
    }

    public void FadeIn3D(bool isRightDir, Vector3 pos){
	    Sequence sequence = GetFadeIn3DSeq(isRightDir, pos);
	    sequence.Play();
    }

/////////////Start of FOV/////////////
    public float Get3DFOV(){
        return cameras3D[0].fieldOfView;
    }
    public float Get2DFOV(){
        return cameras2D[0].fieldOfView;
    }

    public void Set3DFOV(float a){
        float angle = Mathf.Clamp(a, FOV3D_MIN, FOV3D_MAX);
        foreach(Camera c in cameras3D){
            c.fieldOfView = angle;
        }
    }

    public void Set2DFOV(float a){
        float angle = Mathf.Clamp(a, FOV2D_MIN, FOV2D_MAX);
        foreach(Camera c in cameras2D){
            c.fieldOfView = angle;
        }
    }

    public void Zoom2DFOV(float r){
        float rad = (Get2DFOV() / 2.0f) * Mathf.Deg2Rad;
        float newFOV = Mathf.Atan(Mathf.Tan(rad)) / r * Mathf.Rad2Deg * 2.0f;

        Set2DFOV(newFOV);
    }
    public void Zoom3DFOV(float r){
        float rad = (Get3DFOV() / 2.0f) * Mathf.Deg2Rad;
        float newFOV = Mathf.Atan(Mathf.Tan(rad)) / r * Mathf.Rad2Deg * 2.0f;

        Set3DFOV(newFOV);
    }

    public float Get3DZoomRate(){
        return Mathf.Tan(Get3DFOV() / 2.0f * Mathf.Deg2Rad) / Mathf.Tan(FOV3D_DEF / 2.0f * Mathf.Deg2Rad);
    }

    public float Get2DZoomRate(){
        return Mathf.Tan(Get2DFOV() / 2.0f * Mathf.Deg2Rad) / Mathf.Tan(FOV2D_DEF / 2.0f * Mathf.Deg2Rad);
    }

    public void Reset2DFOV(){
        Set2DFOV(FOV2D_DEF);
    }
    public void Reset3DFOV(){
        Set3DFOV(FOV3D_DEF);
    }
/////////////End of FOV///////////////

    public void Translate3D(Vector3 v, Space relativeTo= Space.Self){
        float d = move3D.localPosition.z;
        move3D.Translate(v, relativeTo);
        //depth3D.Translate(new Vector3(0.0f, 0.0f, move3D.localPosition.z - d));
        //move3D.localPosition = new Vector3(move3D.localPosition.x, move3D.localPosition.y, d);
    }

    public void Rotate3D(float x, float y, float z, Space relativeTo = Space.Self){
        root3D.Rotate(x, y, z, relativeTo);
    }
    public Quaternion Get3DRotation(){
        return root3D.localRotation;
    }
    public void Translate2D(Vector3 v, Space relativeTo= Space.Self){
        root2D.transform.Translate(v, relativeTo);
    }
/*
    public void LookAt2D(Transform target){
        Vector3 p = target.position;
        float depth = target.localScale.x * CAMERA2D_DEPTH_DEF;
        root2D.transform.position = new Vector3(p.x, p.y, depth);
    }*/

    public void SetRootWorldPos(Vector3 v){
        root3D.position = v;
        //SetPosWithFixedHierarchyPos(root3D, v);
    }

    public Vector3 GetMovePos(){
        return move3D.localPosition;
    }

    public Vector3 GetRootWorldPos(){
        return root3D.position;
    }

    public bool Is2DActive(){
        return camera2D.gameObject.activeSelf;
    }
    public void Active2D(){
        //Debug.Log("Active2D");
        camera2D.transform.gameObject.SetActive(true);
        camera3D.gameObject.SetActive(false);
    }
    public void Active3D(){
        //Debug.Log("active3D");
        camera2D.transform.gameObject.SetActive(false);
        camera3D.gameObject.SetActive(true);
    }

    public Camera Get2DCamera(){
        return camera2D;
    }

    public float GetDepth(){
        return CAMERA_DEPTH;
    }
    /*
    public float Get2DDepth(){
        return root2D.transform.localPosition.z;
    }
    public float Get2DDepthRate(){
        return root2D.transform.localPosition.z / CAMERA2D_DEPTH_DEF;
    }
    public float Get3DDepth(){
        return depth3D.localPosition.z;
    }
    public void Set3DDepth(float d){
        Vector3 p = depth3D.localPosition;
        depth3D.localPosition = new Vector3(p.x, p.y, d);
    }*/
    public void Bounds2D(Vector2 size, float baseX = 0, float baseY = 0){
        Vector3 p = root2D.transform.localPosition;
        float height = size.y;
        float width = size.x;
        p.x = Mathf.Clamp(p.x, baseX-width/2f, baseX+width/2f);
        p.y = Mathf.Clamp(p.y, baseY-height/2f, baseY+height/2f);

        root2D.transform.localPosition = p;
    }
    public void Bounds3D(Bounds b){
        Vector3 p = move3D.localPosition;
        float height = b.size.y;
        float width = b.size.x;
        float depth = CAMERA3D_MOVEZ;
        p.x = Mathf.Clamp(p.x, -width/2f, width/2f);
        p.y = Mathf.Clamp(p.y, -height/2f, height/2f);
        p.z = Mathf.Clamp(p.z, -depth/2f, depth/2f);
        move3D.localPosition = p;
    }/*
    public void Zoom2D(float r){
        Vector3 p = root2D.transform.localPosition;
        float depth = Mathf.Clamp(p.z + p.z * r, CAMERA2D_DEPTH_LB, CAMERA2D_DEPTH_UB);
        root2D.transform.localPosition = new Vector3(p.x, p.y, depth);

    }
    public void Zoom3D(float r){
        Vector3 p = depth3D.localPosition;
        float depth = Mathf.Clamp(p.z + p.z * r, CAMERA3D_DEPTH_LB, CAMERA3D_DEPTH_UB);
        depth3D.localPosition = new Vector3(0.0f, 0.0f, depth);

    }*/
    public void Reset2DCamPosAndDepth(){
        root2D.transform.localPosition = new Vector3(0.0f, 0.0f, CAMERA_DEPTH);
        Reset2DFOV();
    }
    public void Reset3DCamPosAndDepth(){
        Vector3 v = humanModel.GetModelBodyPosition();
        root3D.position = new Vector3(v.x, v.y, 0.0f);
        root3D.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        move3D.localPosition = Vector3.zero;
        //move3D.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        depth3D.localPosition = new Vector3(0.0f, 0.0f, CAMERA_DEPTH);

        Reset3DFOV();
    }

    public void Reset3DCamPosAndDepthWithAnim(){
        Vector3 v = humanModel.GetModelBodyPosition();
        Quaternion rot = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        Transform3DWithAnim(v, rot, FOV3D_DEF);
    }

    public void SetRootPosWithFixedHierarchyPos(Vector3 pos){
        SetPosWithFixedHierarchyPos(root3D, pos);
    }

    public void SetRootPosToMovePosWithFixedHierarchyPos(){
        SetPosWithFixedHierarchyPos(root3D, move3D.position);
    }
    
    private void SetPosWithFixedHierarchyPos(Transform target, Vector3 pos){
        List<Vector3> fixPosTmp = new List<Vector3>();
        StorePos(target, true, fixPosTmp);
        //Debug.Log("target.pos:"+target.position+", target.newpos:"+pos);
        target.position = pos;

        RestorePos(target, true, fixPosTmp);
    }

    private void StorePos(Transform t, bool isRoot, List<Vector3> fixPosTmp){
        if (!isRoot){
            //Debug.Log(t.name+" "+t.position);
            fixPosTmp.Add(t.position);
        }
        foreach(Transform child in t){
            StorePos(child, false, fixPosTmp);
        }
    }

    private void RestorePos(Transform t, bool isRoot, List<Vector3> fixPosTmp){
        if (!isRoot){
            //Debug.Log(t.name+" "+t.position);
            t.transform.position = fixPosTmp[0];
            fixPosTmp.RemoveAt(0);
        }
        foreach(Transform child in t){
            RestorePos(child, false, fixPosTmp);
        }
    }

    public void StartSS2DFrom(){
    	ss2DCameraFrom.gameObject.SetActive(true);
    }

    public void EndSS2DFrom(){
    	ss2DCameraFrom.gameObject.SetActive(false);
    }
    public void StartSS2DTo(){
    	ss2DCameraTo.gameObject.SetActive(true);
    }

    public void EndSS2DTo(){
    	ss2DCameraTo.gameObject.SetActive(false);
    }
    public void StartSS3DFrom(){
    	ss3DCameraFrom.gameObject.SetActive(true);
    }

    public void EndSS3DFrom(){
    	ss3DCameraFrom.gameObject.SetActive(false);
    }
    public void StartSS3DTo(){
    	ss3DCameraTo.gameObject.SetActive(true);
    }

    public void EndSS3DTo(){
    	ss3DCameraTo.gameObject.SetActive(false);
    }
}
