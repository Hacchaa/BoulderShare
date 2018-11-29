using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class BoRouteReader : MonoBehaviour {
	private string PATH ;
	[SerializeField]
	private List<string> hScenesList;

	void Awake(){
		PATH = Application.persistentDataPath + "/route/20181119185144/" ;
	}

	void Start(){
		Load();
	}

	public void Load(){
		string json = "";

		if (File.Exists(PATH + "hscenes.json")){
			json = File.ReadAllText(PATH + "hscenes.json");
		}

		hScenesList = HScenesList.ConvertJsonToList(json);
	}

	public List<SceneGraph.SceneTree> Make(){
		List<SceneGraph.SceneTree> hsList = new List<SceneGraph.SceneTree>();

		foreach(string str in hScenesList){
			List<SGData> dhcList = new List<SGData>();
			DHScenes.DataArray array = DHScenes.ConvertJsonToDHScenes(str);
			foreach(DHScenes.Data data in array.arr){
				SGData node = new SGData(data.id);
				dhcList.Add(node);
			}

			hsList.Add(new SceneGraph.SceneTree(dhcList, array.numOfCreatingHScene));
		}

		return hsList;
	}

}
