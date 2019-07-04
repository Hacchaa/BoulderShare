using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallManager : MonoBehaviour
{
	[SerializeField] private List<BaseWall> walls;
	private int masterIncline = 90;
	private Texture2D masterWallImage;
	[SerializeField] private GameObject masterWallMarks;
	public static float WALL_H = 4.0f;
	public static float WALL_MIN = 4.0f;
	private Vector2 wallSize;
	private bool isShowTranslucentWall = false;


	public bool IsWallImagePrepared(){
		return masterWallImage != null;
	}

	public Texture2D GetMasterWallImage(){
		return masterWallImage;
	}
	public int GetMasterIncline(){
		return masterIncline;
	}
	public GameObject GetMasterWallMarks(){
		return masterWallMarks;
	}
	public Vector2 GetMasterWallSize(){
		return wallSize;
	}
	public void CommitIncline(int incline){
		masterIncline = incline;

		SyncIncline();
	}
	public void CommitWallImage(Texture2D tex){
		masterWallImage = tex;
		float r ;
		if (tex.height < tex.width){
			r = tex.height / WALL_MIN;
			wallSize = new Vector2(tex.width / r, WALL_MIN);
		}else{
			r = tex.width / WALL_MIN;
			wallSize = new Vector2(WALL_MIN, tex.height / r);
		}
		Debug.Log("r =" +r);
		Debug.Log("wallsize:"+wallSize);
		SyncWallImage();
	}
	//3d→2dはscaleがバグる
	public void CommitWallMarks(GameObject rootMarks){
		SyncWallMarkObjs(rootMarks);
		SyncWallMarks();
	}

	private void SyncWallMarkObjs(GameObject rootMarks){
		foreach(Transform t in masterWallMarks.transform){
			t.name ="";
			Destroy(t.gameObject);
		}

		foreach(Transform t in rootMarks.transform){
			GameObject obj = new GameObject(t.name);
			obj.transform.parent = masterWallMarks.transform;
			obj.transform.localPosition = t.localPosition;
			obj.transform.localRotation = t.localRotation;
			obj.transform.localScale = t.localScale;
		}
	}

	public void SyncIncline(){
		foreach(BaseWall w in walls){
			w.SetIncline(masterIncline);
		}
	}
	public void SyncWallImage(){
		foreach(BaseWall w in walls){
			w.SetWallImage(masterWallImage);
		}
	}
	public void SyncWallMarks(){
		foreach(BaseWall w in walls){
			w.SetWallMarks(masterWallMarks);
		}
	}

	public void InitMarks(){
		foreach(Transform t in masterWallMarks.transform){
			t.name ="";
			Destroy(t.gameObject);
		}
		TwoDWallMarks.SetNum(0);
		SyncWallMarks();		
	}

	public void LoadMarks(List<MyUtility.Mark> marks){
		foreach(Transform t in masterWallMarks.transform){
			t.name ="";
			Destroy(t.gameObject);
		}

		int max = -1;
		foreach(MyUtility.Mark mark in marks){
			GameObject obj = new GameObject(mark.name);
			obj.transform.parent = masterWallMarks.transform;
			obj.transform.localPosition = new Vector3(mark.x, mark.y, mark.z);
			//obj.transform.localRotation = t.localRotation;
			obj.transform.localScale = Vector3.one * mark.scale;

			int n = int.Parse(mark.name);
			if (n > max){
				max = n;
			}
		}
		TwoDWallMarks.SetNum(max+1);
		SyncWallMarks();
	}

	public List<MyUtility.Mark> GetMarks(){
		MyUtility.Mark mark;
		List<MyUtility.Mark> marks = new List<MyUtility.Mark>();

		foreach(Transform child in masterWallMarks.transform){
			mark = new MyUtility.Mark();
			mark.name = child.gameObject.name;
			mark.x = child.position.x;
			mark.y = child.position.y;
			mark.z = child.position.z;
			mark.scale = child.localScale.x;

			marks.Add(mark);
		}
		return marks;
	}

	public void ShowTranslucentWall(){
		foreach(BaseWall wall in walls){
			wall.ShowTranslucentWall();
		}
		isShowTranslucentWall = true;
	}
	public void HideTranslucentWall(){
		foreach(BaseWall wall in walls){
			wall.HideTranslucentWall();
		}
		isShowTranslucentWall = false;
	}

	public bool IsShowTranslucentWall(){
		return isShowTranslucentWall;
	}
}
