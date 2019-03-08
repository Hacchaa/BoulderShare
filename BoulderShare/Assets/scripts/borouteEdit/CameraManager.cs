using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Transform root3D;
    [SerializeField] private Transform move3D;
    [SerializeField] private Camera camera2D;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private ThreeDWall threeDWall;
    private const float CAMERA2D_DEPTH_UB = -1.2f;
    private const float CAMERA2D_DEPTH_LB = -12.0f;
    private const float CAMERA2D_DEPTH_DEF = -10.0f;
    private const float CAMERA3D_DEPTH_UB = -2.0f;
    private const float CAMERA3D_DEPTH_LB = -12.0f;
    private const float CAMERA3D_DEPTH_DEF = -10.0f;
    private List<Vector3> fixPosTmp;

    void Start(){
    }

    public void Rotate3DWithAnim(Quaternion rot){
    	root3D.DORotateQuaternion(rot, duration);
    }

    public void Translate3D(Vector3 v, Space relativeTo= Space.Self){
        move3D.Translate(v, relativeTo);
    }

    public void Rotate3D(float x, float y, float z, Space relativeTo = Space.Self){
        root3D.Rotate(x, y, z, relativeTo);
    }
    public void Translate2D(Vector3 v, Space relativeTo= Space.Self){
        camera2D.transform.Translate(v, relativeTo);
    }

    public void SetRootWorldPos(Vector3 v){
        root3D.position = v;
    }

    public Vector3 GetRootWorldPos(){
        return root3D.position;
    }

    public bool Is2DActive(){
        return camera2D.gameObject.activeSelf;
    }
    public void Active2D(){
        camera2D.transform.gameObject.SetActive(true);
        root3D.gameObject.SetActive(false);
    }
    public void Active3D(){
        camera2D.transform.gameObject.SetActive(false);
        root3D.gameObject.SetActive(true);
    }

    public Camera Get2DCamera(){
        return camera2D;
    }
    //
    public float Get2DDepth(){
        return camera2D.transform.localPosition.z;
    }

    public float Get3DDepth(){
        return move3D.localPosition.z;
    }
    public void Set3DDepth(float d){
        Vector3 p = move3D.localPosition;
        move3D.localPosition = new Vector3(p.x, p.y, d);
    }
    public void Bounds2D(Vector2 size){
        Vector3 p = camera2D.transform.localPosition;
        float height = size.y;
        float width = size.x;
        p.x = Mathf.Clamp(p.x, -width/2f, width/2f);
        p.y = Mathf.Clamp(p.y, -height/2f, height/2f);

        camera2D.transform.localPosition = p;
    }
    public void Bounds3D(Bounds b){
        Vector3 p = move3D.localPosition;
        float height = b.size.y;
        float width = b.size.x;
        p.x = Mathf.Clamp(p.x, -width/2f, width/2f);
        p.y = Mathf.Clamp(p.y, -height/2f, height/2f);

        move3D.localPosition = p;
    }
    public void Zoom2D(float r){
        Vector3 p = camera2D.transform.localPosition;
        float depth = Mathf.Clamp(p.z + p.z * r, CAMERA2D_DEPTH_LB, CAMERA2D_DEPTH_UB);
        camera2D.transform.localPosition = new Vector3(p.x, p.y, depth);

    }
    public void Zoom3D(float r){
        Vector3 p = move3D.localPosition;
        float depth = Mathf.Clamp(p.z + p.z * r, CAMERA3D_DEPTH_LB, CAMERA3D_DEPTH_UB);
        move3D.localPosition = new Vector3(p.x, p.y, depth);

    }
    public void Reset2DCamPosAndDepth(){
        camera2D.transform.localPosition = new Vector3(0.0f, 0.0f, CAMERA2D_DEPTH_DEF);
    }
    public void Reset3DCamPosAndDepth(){
        Vector3 v = threeDWall.GetWallWorldPos();
        root3D.position = new Vector3(v.x, v.y, 0.0f);
        root3D.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        move3D.localPosition = new Vector3(0.0f, 0.0f, CAMERA3D_DEPTH_DEF);
    }
    /*
    public void SetPosWithFixedHierarchyPos(Vector3 pos){
        fixPosTmp = new List<Vector3>();
        StorePos(transform, true);

        transform.position = pos;

        RestorePos(transform, true);
    }

    private void StorePos(Transform t, bool isRoot){
        if (!isRoot){
            fixPosTmp.Add(t.position);
        }
        foreach(Transform child in t){
            StorePos(child, false);
        }
    }

    private void RestorePos(Transform t, bool isRoot){
        if (!isRoot){
            t.transform.position = fixPosTmp[0];
            fixPosTmp.RemoveAt(0);
        }
        foreach(Transform child in t){
            RestorePos(child, false);
        }
    }*/
}
