using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class MyUtility {
	public static int FINGER_NONE = -10;

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
        Look,
        Other
    }
    public const string PPName_ModelFigure = "Info_ModelFigure";
/*
    public static bool IsMFMark(FullBodyMark mark){
    	return IsMFHandMark(mark) || IsMFFootMark(mark);
    }

    public static bool IsMFHandMark(FullBodyMark mark){
		return mark == FullBodyMark.LeftHand ||
			mark == FullBodyMark.RightHand ||
			mark == FullBodyMark.LeftElbow || 
			mark == FullBodyMark.RightElbow ;
	}

   	public static bool IsMFHandMark(FullBodyMark mark){
		return mark == FullBodyMark.LeftElbow || 
			mark == FullBodyMark.RightElbow ;
	}
	public static bool IsMFFootMark(FullBodyMark mark){
		return mark == FullBodyMark.LeftPelvis ||
			mark == FullBodyMark.RightPelvis ||
			mark == FullBodyMark.LeftKnee || 
			mark == FullBodyMark.RightKnee ;
	}
	public static bool IsMFRightMark(FullBodyMark mark){
		return mark == FullBodyMark.RightHand ||
			mark == FullBodyMark.RightPelvis ||
			mark == FullBodyMark.RightElbow || 
			mark == FullBodyMark.RightKnee ;
	}*/

	public static int Gcd(int a, int b){
		if (a < b){
			return Gcd(b, a);
		}

		while(b != 0){
			int rem = a % b;
			a = b;
			b = rem;
		}

		return a;
	}

	public static void SetLayerRecursively(GameObject self, int layer){
		self.layer = layer;

		foreach(Transform t in self.transform){
			SetLayerRecursively(t.gameObject, layer);
		}
	}

	public static void WriteImage(Texture2D tex, string filepath){
		byte[] pngData = tex.EncodeToPNG();
        Debug.Log("copy texture at "+ filepath);
        File.WriteAllBytes(filepath, pngData);
	}


	public static Texture2D LoadImage(string path){
		Texture2D texture = new Texture2D(0, 0);
		texture.LoadImage(LoadBytes(path));
		return texture;
	}

	public static Sprite CreateSprite(Texture2D texture){
	    return Sprite.Create(
	        texture, 
	        new Rect(0.0f, 0.0f, texture.width, texture.height), 
	        new Vector2(0.5f, 0.5f),
	        texture.height/WallManager.WALL_H);
	}

	private static byte[] LoadBytes(string path) {
		FileStream fs = new FileStream(path, FileMode.Open);
		BinaryReader bin = new BinaryReader(fs);
		byte[] result = bin.ReadBytes((int)bin.BaseStream.Length);
		bin.Close();
		return result;
	}

	[Serializable]
	public class BorouteInfoForSearching{
		public List<BorouteInformation> data;
	}

	[Serializable]
	public class BorouteInformation{
		public string timestamp;
		public string date;
		public string place;
		public string globalComment;
		public int incline;
		public int grade;
		public int tryCount;
		public bool isComplete;
		public float scaleH2M;
	}

	[Serializable]
	public class Boroute{
		public BorouteInformation borouteInfo;
		public List<ClimbRecord> records;

		public Boroute(){
			borouteInfo = new BorouteInformation();
			records = new List<ClimbRecord>();
		}
	}

	[Serializable]
	public class ClimbRecord{
		public List<AttemptTree> attempts;
		public List<Mark> marks;
		public List<Scene> masterScenes;

		public ClimbRecord(){
			attempts = new List<AttemptTree>();
			marks = new List<Mark>();
			masterScenes = new List<Scene>();
		}
	}

	[Serializable]
	public class AttemptTrees{
		public List<string> trees;
	}

	[Serializable]
	public class AttemptTree{
		//public Scene[] data;
		//public List<string> failedList;
		//最初からこのシーンが保存されるまでに作られたシーンの数
		public List<int> idList;
		public int numOfCreatingHScene;
	}

	[Serializable]
	public class Scene {
		public int id;
		public string[] holdsOnHand;
		public List<SceneCommentData3D> comments;
		public Vector3[] pose;
		public Quaternion[] rots;
		public List<string> failureList;
		public int leftHandAnim;
		public int rightHandAnim;
	}

	[Serializable]
	public class SceneCommentData{
		public string text;
		public float fontSize;
		public Vector3 pos;
		public float rotDeg;
		public float width;
		public Color color;
	}

	[Serializable]
	public class SceneCommentData3D{
		public string text;
		public float fontSize;
		public Vector3 pos;
		public Quaternion rot;
		public float width;
		public Color color;
	}

	[Serializable]
	public class Marks{
		public Mark[] data;
	}

	[Serializable]
	public class Mark{
		public float x, y, z;
		public float scale;
		public string name;
	}

	[Serializable]
	public class ModelFigure{
		public float height;
		public float reach;
		public float leg;
	}
}
