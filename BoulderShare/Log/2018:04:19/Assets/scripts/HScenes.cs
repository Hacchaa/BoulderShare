using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class HScenes : MonoBehaviour {
	public Text curNum;
	public Text num;
	private List<HScene> list ;
	public SceneFocus sf;
	public SceneNum sn;

	// Use this for initialization
	void Start () {
		list = new List<HScene>();
		list.Add(new HScene());
		curNum.text = "1";
		num.text = "1";
	}

	public void InitScenes(){
		Start();
	}


	public void AddScene(){
		int index = int.Parse(curNum.text) - 1;
		HScene scene = new HScene();

		sf.SaveScene(list[index]);
		sf.LoadScene(scene);
		list.Insert(index+1, scene);
		sn.Add(index+1);

		curNum.text = index + 2 + "";
		num.text = int.Parse(num.text) + 1 + "";
	}

	public void RemoveScene(){
		int index = int.Parse(curNum.text) - 1;
		int nextIndex ;
		int n = int.Parse(num.text);
		HScene next ;
		if (n == 1){
			next = new HScene();
			list.Add(next);
			sn.Add(1);
			nextIndex = 1;
		}else if (index == 0){
			next = list[1];
			nextIndex = 1;
		}else{
			next = list[index-1];
			nextIndex = index ;
		}

		list.RemoveAt(index);
		sf.LoadScene(next);
		sn.Remove(index);

		curNum.text = nextIndex + "";
		num.text = list.Count + "";
	}

	public void NextScene(){
		int index = int.Parse(curNum.text) - 1;

		if (index+1 < list.Count){
			sf.SaveScene(list[index]);
			sf.LoadScene(list[index+1]);
			sn.Next();

			curNum.text = index + 2 + "";
		}
	}

	public void PrevScene(){
		int index = int.Parse(curNum.text) - 1;

		if (index > 0){
			sf.SaveScene(list[index]);
			sf.LoadScene(list[index-1]);
			sn.Prev();

			curNum.text = index + "";
		}
	}

	public void NextScene2(){
		int index = int.Parse(curNum.text) - 1;

		if (index+1 < list.Count){
			sf.SaveScene(list[index]);
			sf.LoadScene(list[index+1]);
			sn.Next();

			curNum.text = index + 2 + "";
		}else if (index+1 == list.Count){
			AddScene();
		}
	}

	public void PrevScene2(){
		int index = int.Parse(curNum.text) - 1;

		if (index > 0){
			sf.SaveScene(list[index]);
			sf.LoadScene(list[index-1]);
			sn.Prev();

			curNum.text = index + "";
		}else if(index == 0){
			HScene newScene = new HScene();

			sf.SaveScene(list[index]);
			sf.LoadScene(newScene);
			list.Insert(index, newScene);

			curNum.text = index + 1 + "";
			num.text = list.Count + "";
		}
	}
	
}
