using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class RouteView : MonoBehaviour {
	public GameObject routePrefab;
	public GameObject root;

	// Use this for initialization
	void Start () {
		LoadRoute();
	}
	
	public void LoadRoute(){
		string routePath = Application.persistentDataPath + Observer.ROUTEPATH;
		string[] files = Directory.GetDirectories(routePath, "*");
		Debug.Log(routePath);

		foreach(string path in files){
			GameObject route = Instantiate(routePrefab, root.transform);
			GameObject routeInfo = route.transform.Find("RouteInfo").gameObject;

			Text title = routeInfo.transform.Find("Title").gameObject.GetComponent<Text>();
			Text time = routeInfo.transform.Find("Time").gameObject.GetComponent<Text>();
			Image img = route.transform.Find("Image").GetComponent<Image>();

			if (File.Exists(path + "/Wall.png")){
				img.sprite = LoadImage(path + "/Wall.png");
			}
			string json = File.ReadAllText(path + "/route.txt");
			DRoute.Data rData = JsonUtility.FromJson<DRoute.Data>(json);
			title.text = rData.title;
			time.text = rData.time;
		}
	}

	private Sprite LoadImage(string path){
		Texture2D texture = new Texture2D(0, 0);
		texture.LoadImage(LoadBytes(path));
		return Sprite.Create(
			texture, 
            new Rect(0.0f, 0.0f, texture.width, texture.height), 
            new Vector2(0.5f, 0.5f),
            texture.height/4);
	}

	private byte[] LoadBytes(string path) {
		FileStream fs = new FileStream(path, FileMode.Open);
		BinaryReader bin = new BinaryReader(fs);
		byte[] result = bin.ReadBytes((int)bin.BaseStream.Length);
		bin.Close();
		return result;
	}
}
