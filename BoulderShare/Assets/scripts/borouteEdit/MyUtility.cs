using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class MyUtility {

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
		public string borouteInfo;
		public string hScenesList;
		public string marks;
	}

	[Serializable]
	public class AttemptTrees{
		public List<string> trees;
	}

	[Serializable]
	public class AttemptTree{
		public Scene[] data;
		public List<string> failedList;
		//最初からこのシーンが保存されるまでに作られたシーンの数
		public int numOfCreatingHScene;
	}

	[Serializable]
	public class Scene {
		public int id;
		public string[] holdsOnHand;
		public List<SceneCommentData3D> comments;
		public Vector3[] pose;
		public Quaternion[] rots;
		public bool isLookingActivate;
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
		public double x, y, z;
		public double scale;
		public string name;
	}
}
