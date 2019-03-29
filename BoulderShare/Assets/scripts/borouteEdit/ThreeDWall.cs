using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeDWall : BaseWall{
	[SerializeField] private GameObject wall;
	[SerializeField] private ThreeDWallMarks threeDWallMarks;
	[SerializeField] private ThreeDView view;
	[SerializeField] private Collider collider;
	[SerializeField] private WallManager wallManager;
	[SerializeField] private int wallDegOffset = 90;
	public override void SetWallImage(Texture2D tex){
		Vector2 wallSize = wallManager.GetMasterWallSize();
		wall.transform.localScale = new Vector3(wallSize.x * 0.1f, 0.1f, wallSize.y * 0.1f);
		wall.transform.parent.localPosition = new Vector3(0.0f, wallSize.y/2, 0.0f);

		//3Dモデルの壁のテクスチャーを設定
		Renderer rend = wall.GetComponent<Renderer>();
		Material[] mat = rend.materials;
		Material m = new Material(mat[0]);
		m.SetTexture("_MainTex", tex);
		mat[0] = m;
		rend.materials = mat;
	}
	public override void SetIncline(int incline){
		transform.localRotation = Quaternion.Euler(
			90 - incline,
			0,
			0);
	}
	public int GetIncline(){
		return CalcIncline();
	}
	public override void SetWallMarks(GameObject rootMarks, int n){
		threeDWallMarks.Synchronize(rootMarks);
	}

	public void AcceptEvents(){
		view.enabled = true;
	}

	public void IgnoreEvents(){
		view.enabled = false;
	}

	public Bounds Get3DWallBounds(){
		return collider.bounds;
	}

	public Vector3 CalcWorldSubVec(Vector3 localP){
		Debug.Log("calcWorldSubVec:"+localP+" = "+ (transform.TransformPoint(localP) - transform.position));
		return transform.TransformPoint(localP) - transform.position;
	}

	private int CalcIncline(){
		float angle = transform.eulerAngles.x;
		if (angle > 180){
			angle -= 360;
		}
		Debug.Log("calcIncline " + (90 - angle));
		return (int)Mathf.Round(90 - angle);
	}


	//壁の傾斜に沿ったワールド座標のz座標を返す
	public float CalcZPos(Vector2 p){
		float incline = CalcIncline() ;
		if (incline == 90){
			return wall.transform.localPosition.z; 
		}
		return -(p.y - wall.transform.localPosition.y) * Mathf.Tan(Mathf.Deg2Rad * (incline-wallDegOffset)) + wall.transform.localPosition.z; 
	}

	public Vector3 CalcWallPoint(Vector3 p){
		//Debug.Log("input "+ p);
		p = threeDWallMarks.transform.InverseTransformPoint(p);
		//Debug.Log("local "+ p);
		p = new Vector3(p.x, p.y, 0.0f);
		//Debug.Log("mod "+ p);
		//Debug.Log("return "+ transform.TransformPoint(p));
		return threeDWallMarks.transform.TransformPoint(p);
		/*
		float incline = CalcIncline() ;
		//wallのlocal座標に変換
		Debug.Log("input p "+p);
		p = transform.InverseTransformPoint(p);
		Debug.Log("local p "+p);
		float z = 0.0f;
		if (incline == 90){
			z = wall.transform.localPosition.z;
		}else{
			z =  -(p.y - wall.transform.localPosition.y) * Mathf.Tan(Mathf.Deg2Rad * (incline-90)) + wall.transform.localPosition.z; 
		}
		Debug.Log("z "+z);
		Debug.Log("world "+transform.TransformPoint(p.x, p.y, z));
		return transform.TransformPoint(p.x, p.y, z); */
	}

	public Vector3 GetWallWorldPos(){
		return wall.transform.position;
	}
}
