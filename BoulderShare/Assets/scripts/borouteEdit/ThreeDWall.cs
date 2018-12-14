using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeDWall : MonoBehaviour {
	[SerializeField]
	private GameObject wall;
	[SerializeField]
	private TwoDWall twoDWall;
	[SerializeField]
	private ThreeDWallMarks threeDWallMarks;


	public void SetWall(){
		Bounds bounds = twoDWall.GetWallBounds();
		Texture tex = twoDWall.GetWallTexture();
		wall.transform.localScale = new Vector3(bounds.size.x * 0.1f, 0.1f, bounds.size.y * 0.1f);
		wall.transform.parent.localPosition = new Vector3(0.0f, bounds.size.y/2, 0.0f);

		//3Dモデルの壁のテクスチャーを設定
		Renderer rend = wall.GetComponent<Renderer>();
		Material[] mat = rend.materials;
		Material m = new Material(mat[0]);
		m.SetTexture("_MainTex", tex);
		mat[0] = m;
		rend.materials = mat;
	}

	public void SetWallIncline(int value){
		Debug.Log("3dwall setwallincline "+ value);
		transform.localRotation = Quaternion.Euler(
			90 - value,
			0,
			0);
	}

	public int GetWallIncline(){
		return CalcIncline();
	}

	private int CalcIncline(){
		float angle = transform.eulerAngles.x;
		if (angle > 180){
			angle -= 360;
		}
		Debug.Log("calcIncline " + (90 - angle));
		return (int)Mathf.Round(90 - angle);
	}

	//2dwallの内容を3dwallに同期
	public void Synchronize(){
		threeDWallMarks.Synchronize();
	}

	//壁の傾斜に沿ったワールド座標のz座標を返す
	public float CalcZPos(Vector2 p){
		float incline = CalcIncline() ;
		if (incline == 90){
			return wall.transform.localPosition.z; 
		}
		return -(p.y - wall.transform.localPosition.y) * Mathf.Tan(Mathf.Deg2Rad * (incline-90)) + wall.transform.localPosition.z; 
	}

	public Vector3 CalcWallPoint(Vector3 p){
		//Debug.Log("input "+ p);
		p = threeDWallMarks.gameObject.transform.InverseTransformPoint(p);
		//Debug.Log("local "+ p);
		p = new Vector3(p.x, p.y, 0.0f);
		//Debug.Log("mod "+ p);
		//Debug.Log("return "+ transform.TransformPoint(p));
		return threeDWallMarks.gameObject.transform.TransformPoint(p);
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
