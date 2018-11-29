using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class SceneGraph : MonoBehaviour {
	private List<SGNode> rootList;
	[SerializeField]
	private GameObject sGNodePrefab;
	private GameObject nodeSet;
	private List<SceneGraph.SceneTree> hscenesList;
	[SerializeField]
	private BoRouteReader bReader;
	private int showIndex;

	void Start(){
		 nodeSet = GameObject.FindGameObjectsWithTag("SGNodeSet")[0];
		 rootList = new List<SGNode>();
	}

	public void FirstProc(){
		Load();
		Construction();
	}

	public void Load(){
		hscenesList = bReader.Make();
	}


	private void MakeFirstTree(List<SGData> list){
		SGNode root = MakeNode(list[0]);
		int n = list.Count;

		SGNode curNode = root;
		for(int i = 1 ; i < n ; i++){
			curNode = curNode.AddChildNode(list[i]);
		}
		rootList.Add(root);
	}


	public void Construction(){
		int n = hscenesList.Count;
		int oldNOCH = -1;
		Debug.Log("n="+n);
		for(int i = 0 ; i < n ; i++){
			Debug.Log("i="+i);
			List<SGData> list = hscenesList[i].GetScenes();
			int nOCH = hscenesList[i].GetNOCH();

			if (i == 0){
				Debug.Log("makeFirstTree()");
				//baseの木を構成
				MakeFirstTree(list);
			}else{
				//初めにbaseのどの根からスタートするか求める
				SGNode baseCurNode = null;
				SGNode curNode = null;
				SGData curData = list[0];
				Debug.Log("oldNOCH <= curData.GetID() == "+oldNOCH + " <= " + curData.GetID());
				//curNodeの値を持つシーンがbaseに存在するかどうか
				if (oldNOCH <= curData.GetID()){
					//存在しない場合
					//curNodeをbaseの根に追加する
					curNode = MakeNode(curData);
					rootList.Add(curNode);
				}else{
					//存在する場合
					//curNodeと値が等しいbaseのノードを探してそこから始める
					baseCurNode = SGNode.Search(curData, rootList);
					curNode = baseCurNode;
				}
				int m = list.Count;
				Debug.Log("m="+m);
				for(int j = 1 ; j < m ; j++){
					Debug.Log("j="+j);
					curData = list[j];
					Debug.Log("oldNOCH <= curData.GetID() == "+oldNOCH + " <= " + curData.GetID());
					//curDataを持つシーンがbaseに存在するかどうか
					if(oldNOCH <= curData.GetID()){
						//存在しない場合、
						//curNodeから分岐する
						curNode = curNode.AddChildNode(curData);
					}else{
						//存在する場合
						//baseCurNode以降の子供を探す
						if (baseCurNode == null){
							baseCurNode = SGNode.Search(curData, rootList);
						}else{
							baseCurNode = SGNode.Search(curData, baseCurNode);
						}

						//baseCurNodeがcurNodeの子供でない場合
						if (!curNode.IsParent(baseCurNode)){
							//新しく子供に設定する
							curNode.AddChildNode(baseCurNode);
						}
						//次に進む
						curNode = baseCurNode;

						//baseCurNodeがrootの場合、rootから削除する
						bool isRoot = false;
						foreach(SGNode root in rootList){
							if(root == baseCurNode){
								isRoot = true;
								break;
							}
						}
						if(isRoot){
							rootList.Remove(baseCurNode);
						}
					}
				}
			}
			oldNOCH = nOCH;
		}
	}

	public void ShowNext(){
		if (showIndex >= hscenesList.Count - 1){
			return ;
		}
		HideHScenes(showIndex);
		showIndex++;
		ShowHScenes(showIndex);
	}

	public void ShowPrev(){
		if (showIndex <= 0){
			return ;
		}
		HideHScenes(showIndex);
		showIndex--;
		ShowHScenes(showIndex);
	}
	public void HideHScenes(int index){
		List<SGData> list = hscenesList[index].GetScenes();
		SGNode node = null;
		SGNode old = null;
		foreach(SGData d in list){
			if (node == null){
				node = SGNode.Search(d, rootList);;
			}else{
				node = SGNode.Search(d, node);
				//エッジの色の変更
				old.HideEdge(node);
			}

			node.gameObject.GetComponent<Renderer>().material.color = Color.red;
			old = node;
		}
	}

	public void ShowHScenes(int index){
		List<SGData> list = hscenesList[index].GetScenes();
		SGNode node = null;
		SGNode old = null;

		foreach(SGData d in list){
			if (node == null){
				node = SGNode.Search(d, rootList);
			}else{
				node = SGNode.Search(d, node);
				//エッジの色の変更
				old.ShowEdge(node);
			}

			node.gameObject.GetComponent<Renderer>().material.color = Color.blue;
			old = node;
		}

		showIndex = index;
	}

	private SGNode MakeNode(SGData d){
		GameObject obj = Instantiate(sGNodePrefab, nodeSet.transform);
		SGNode node = obj.GetComponent<SGNode>();
		node.Init(d);
		return node;
	}


	public void Show(){
		SGNode.ShowAll(rootList);
	}

	public void DeployObjs(){
		//グラフの最大幅と深さを求める
		int maxDepth = SGNode.Depth(rootList);
		List<int> list = Enumerable.Repeat(0, maxDepth).ToList();
		int maxBreadh = SGNode.Breadth(list, rootList);
	
		Debug.Log("depth:"+maxDepth + ", breadth:"+maxBreadh);

		//配置mapをつくる
		int width = 40, height = 100;
		/*
		float[][] map = new float[list.Count][maxBreadh];
		int width = 10, height = 10;
		for(int i = 0 ; i < map.Length ; i++){
			for (int j = 0 ; j < map[0].Length ; j++){
				map[i][j] = 
			}
		}*/

		SGNode.DeployNodes(width, height, maxBreadh, maxDepth, rootList);
	}

	public void DeployObjs2(){
		//グラフの最大幅と深さを求める
		int maxDepth = SGNode.Depth(rootList);
		List<List<SGNode>> list = new List<List<SGNode>>();
		for(int i = 0 ; i < maxDepth ; i++){
			list.Add(new List<SGNode>());
		}
		SGNode.Breadth2(list, rootList);
		
		SGNode.ModPos(list, rootList);
		int maxBreadh = -1;
		foreach(List<SGNode> l in list){
			int temp = l.Count;
			if (maxBreadh < temp){
				maxBreadh = temp;
			}
		}
		//Debug.Log("depth:"+maxDepth + ", breadth:"+maxBreadh);

		//配置mapをつくる
		int width = 20, height = 60;

		SGNode.DeployNodes2(width, height, maxBreadh, maxDepth, rootList);
	}

	public void DeployObj3(){
		int maxDepth = SGNode.Depth(rootList);
		bool[,] arrangeableMap = new bool[maxDepth, maxDepth];
		SGNode[,] nodeMap = new SGNode[maxDepth, maxDepth];

		List<List<SGNode>> list = new List<List<SGNode>>();
		for(int i = 0 ; i < maxDepth ; i++){
			list.Add(new List<SGNode>());
		}
		SGNode.Breadth2(list, rootList, false);

		SGNode.ModPos3(arrangeableMap, nodeMap, list, rootList);

		//配置mapをつくる
		int width = 20, height = 60;
		int maxBreadh = -1;
		for(int i = 0 ; i < arrangeableMap.GetLength(0) ; i++){
			for(int j = 0 ; j < arrangeableMap.GetLength(0) ; j++){
				if(arrangeableMap[i, j] && j > maxBreadh){
					maxBreadh = j;
				}
			}
		}
		SGNode.DeployNodes2(width, height, maxBreadh, maxDepth, rootList);

	}

	public class SceneTree{
		private List<SGData> scenes;
		private int numOfCreatingHScene;

		public SceneTree(List<SGData> s, int n){
			scenes = s;
			numOfCreatingHScene = n;
		}

		public List<SGData> GetScenes(){
			return scenes;
		}

		public int GetNOCH(){
			return numOfCreatingHScene;
		}
	}
}
