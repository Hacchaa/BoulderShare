using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//sceneGraphのノードを表すクラス

public class SGNode : MonoBehaviour {
	private SGData data;
	//子供と子供をつなぐ枝を持つ
	//このノードはchildList[i]とedgeList[i]でつながっている
	private List<SGEdge> edgeList;
	private List<SGNode> childList;
	private List<SGNode> parentList;
	//prefab
	[SerializeField]
	private GameObject sGNodePrefab;
	[SerializeField]
	private GameObject sGEdgePrefab;
	[SerializeField]
	private GameObject nodeSet;
	[SerializeField]
	private GameObject edgeSet;

	private static int searchID = 0;
	private int alreadySearchedID;
	private int depth = -1;
	private int breadth = -1;

	void Awake(){
		edgeList = new List<SGEdge>();
		childList = new List<SGNode>();
		parentList = new List<SGNode>();
		alreadySearchedID = searchID;

		nodeSet = GameObject.FindGameObjectsWithTag("SGNodeSet")[0];
		edgeSet = GameObject.FindGameObjectsWithTag("SGEdgeSet")[0];
	}
	void Start(){
	}

	public void Init(SGData d){
		data = d;
		gameObject.name = d.GetID() + "";
	}

	public SGData GetData(){
		return data;
	}

	public static SGNode Search(SGData d, List<SGNode> nodes){
		SGNode target = null;
		SGNode.searchID++;
		foreach(SGNode node in nodes){
			target = node.DepthFirstSearch(d);
			if (target != null){
				return target;
			}
		}

		return null;
	}

	public static SGNode Search(SGData d, SGNode node){
		SGNode.searchID++;
		return node.DepthFirstSearch(d);
	}

	public static void ShowAll(List<SGNode> nodes){
		SGNode.searchID++;
		foreach(SGNode node in nodes){
			node.Show();
		}
	}
	//引数と等しいdataを持つノードを探して返す
	private SGNode DepthFirstSearch(SGData d){
		//探索済みのノードの場合、何もしない
		if(alreadySearchedID == searchID){
			return null;
		}
		//探索済みにする
		alreadySearchedID = searchID;


		//探していたノードだった場合
		if (data.CompareTo(d) == 0){
			return this;
		}

		//再帰的に子ノードを探す
		foreach(SGNode node in childList){
			SGNode rNode = node.DepthFirstSearch(d);
			if (rNode != null){
				return rNode;
			}
		}
		return null;
	}
	public bool IsParent(SGNode child){
		foreach(SGNode node in childList){
			if (node.CompareTo(child) == 0){
				return true;
			}
		}
		return false;
	}
	public int CompareTo(SGNode another){
		return data.CompareTo(another.data);
	}

	public void AddParent(SGNode parent){
		parentList.Add(parent);
	}

	//子ノードを作成する
	public SGNode AddChildNode(SGData d){
		GameObject child = Instantiate(sGNodePrefab, nodeSet.transform);
		GameObject edge = Instantiate(sGEdgePrefab, edgeSet.transform);
		SGNode cNode = child.GetComponent<SGNode>();
		SGEdge cEdge = edge.GetComponent<SGEdge>();
		cNode.Init(d);
		edge.name = "from_"+ data.GetID() + "_to_" + d.GetID();
		//子供とエッジをリストに追加
		childList.Add(cNode);
		edgeList.Add(cEdge);
		cNode.AddParent(this);
		//子供と自分自身をエッジで結ぶ
		cEdge.RegistrateDescendantNode(cNode);
		cEdge.RegistrateAncestorNode(this);

		return cNode;
	}

	public void AddChildNode(SGNode node){
		GameObject edge = Instantiate(sGEdgePrefab, edgeSet.transform);
		SGEdge cEdge = edge.GetComponent<SGEdge>();
		edge.name = "from_"+ data.GetID() + "_to_" + node.GetData().GetID();
		//子供とエッジをリストに追加
		childList.Add(node);
		edgeList.Add(cEdge);
		node.AddParent(this);
		//子供と自分自身をエッジで結ぶ
		cEdge.RegistrateDescendantNode(node);
		cEdge.RegistrateAncestorNode(this);
	}

	//幅と深さを求める
	//listのサイズが深さ、値が幅
	public int CalcBreadthAndDepth(List<int> list, int maxDepth, int curDepth){
		//探索済みのノードの場合、何もしない
		if(alreadySearchedID == searchID){
			return -1;
		}
		//探索済みにする
		alreadySearchedID = searchID;

		if(curDepth > maxDepth){
			list.Add(1);
			maxDepth++;
		}else{
			list[curDepth-1] = list[curDepth-1] + 1;
		}

		foreach(SGNode node in childList){
			int tmp = node.CalcBreadthAndDepth(list, maxDepth, curDepth+1);
			if(maxDepth < tmp){
				maxDepth = tmp;
			}
		}
		return maxDepth;
	}

	private void CalcBreadth(List<int> list){
		//探索済みのノードの場合、何もしない
		if(alreadySearchedID == searchID){
			return ;
		}
		//探索済みにする
		alreadySearchedID = searchID;

		list[depth-1] = list[depth-1] + 1;

		foreach(SGNode node in childList){
			node.CalcBreadth(list);
		}
	}

	private void CalcBreadth2(List<List<SGNode>> list, bool isSaveBreadth){
		//探索済みのノードの場合、何もしない
		if(alreadySearchedID == searchID){
			return ;
		}
		//探索済みにする
		alreadySearchedID = searchID;

		list[depth-1].Add(this);
		if (isSaveBreadth){
			breadth = list[depth-1].Count;
		}

		foreach(SGNode node in childList){
			node.CalcBreadth2(list, isSaveBreadth);
		}
	}


	public static int Breadth(List<int> list, List<SGNode> nodes){
		searchID++;
		foreach(SGNode node in nodes){
			node.CalcBreadth(list);
		}
		int max = -1;
		foreach(int i in list){
			if (max < i){
				max = i;
			}
		}
		return max;
	}

	public static void Breadth2(List<List<SGNode>> list, List<SGNode> nodes, bool isSaveBreadth = true){
		searchID++;
		foreach(SGNode node in nodes){
			node.CalcBreadth2(list, isSaveBreadth);
		}
	}

	public static int Depth(List<SGNode> nodes){
		int max = -1;
		foreach(SGNode node in nodes){
			int tmp = node.CalcDepth(1);
			if (max < tmp){
				max = tmp;
			}
		}

		return max;
	}

	private int CalcDepth(int d){
		int max = -1;
		if (d > depth){
			depth = d;
		}

		if (!childList.Any()){
			return depth;
		}

		foreach(SGNode node in childList){
			int tmp = node.CalcDepth(d+1);
			if (max < tmp){
				max = tmp;
			}
		}

		return max;
	}

	private bool Modify(List<List<SGNode>> list){
		//探索済みのノードの場合、何もしない
		if(alreadySearchedID == searchID){
			return true;
		}
		//探索済みにする
		alreadySearchedID = searchID;
		if (!childList.Any()){
			return true;
		}
		bool isNoMod = true;
		Dictionary<float, List<SGNode>> map = new Dictionary<float, List<SGNode>>();

		foreach(SGNode child in childList){
			int x = child.breadth - this.breadth;
			int y = child.depth - this.depth;
			float angle = Vector2.Angle(Vector2.up, new Vector2(x, y));
			if (map.ContainsKey(angle)){
				map[angle].Add(child);
			}else{
				//Debug.Log("angle" + angle);
				List<SGNode> l = new List<SGNode>();
				l.Add(child);
				map.Add(angle, l);
			}
		}

		foreach(List<SGNode> l in map.Values){
			int n = l.Count;
			if (n >= 2){
				isNoMod = false;
				SGNode[] arr = l.OrderBy(x => x.depth).ToArray();
				for(int i = 0 ; i < arr.Length-1 ; i++){
					int d = arr[i].depth;
					foreach(SGNode node in list[d-1]){
						int b = node.breadth;
						node.breadth = b + 1;
						//Debug.Log("change"+node.data.GetID()+" d="+node.depth+", b="+node.breadth);
					}
				}
			}
		}

		if(parentList.Count > 1){
			Dictionary<float, List<SGNode>> map2 = new Dictionary<float, List<SGNode>>();

			foreach(SGNode parent in parentList){
				int x = this.breadth - parent.breadth ;
				int y = this.depth - parent.depth ;
				float angle = Vector2.Angle(Vector2.up, new Vector2(x, y));
				if (map2.ContainsKey(angle)){
					map2[angle].Add(parent);
				}else{
					//Debug.Log("angle" + angle);
					List<SGNode> l = new List<SGNode>();
					l.Add(parent);
					map2.Add(angle, l);
				}
			}

			foreach(List<SGNode> l in map2.Values){
				int n = l.Count;
				if (n >= 2){
					isNoMod = false;
					SGNode[] arr = l.OrderByDescending(x => x.depth).ToArray();
					for(int i = 0 ; i < arr.Length-1 ; i++){
						int d = arr[i].depth;
						foreach(SGNode node in list[d-1]){
							int b = node.breadth;
							node.breadth = b + 1;
							//Debug.Log("change"+node.data.GetID()+" d="+node.depth+", b="+node.breadth);
						}
					}
				}
			}
		}
	

		foreach(SGNode node in childList){
			isNoMod = node.Modify(list);
		}
		return isNoMod;


	}

	private void Modify2(bool[][] nodeMap, List<List<SGNode>> list){
		//探索済みのノードの場合、何もしない
		if(alreadySearchedID == searchID){
			return ;
		}
		//探索済みにする
		alreadySearchedID = searchID;
		//breadth, depthにノードを配置できるかどうか調べる

	}

	private void Modify3(SGNode parent, bool[,] arrangeableMap, SGNode[,] nodeMap, List<List<SGNode>> list){
		//探索済みのノードの場合、何もしない
		if(alreadySearchedID == searchID){
			return ;
		}
		//探索済みにする
		alreadySearchedID = searchID;
		
		//すでに配置されている子と親を探す
		List<SGNode> arrangeList = new List<SGNode>();

		if (parent != null){
			arrangeList.Add(parent);
		}
		foreach(SGNode child in childList){
			if(child.breadth != -1){
				arrangeList.Add(child);
			}
		}

		//現在のノードを配置できる場所を探す
		int n = arrangeableMap.GetLength(0);
		for(int i = 0 ; i < n ; i++){
			bool isArrangeable = true;
			int aSlopeB = -1;
			int aSlopeD = -1;
			//オブジェクトを配置できる場所がある場合
			if (!arrangeableMap[depth-1, i]){
				//仮に幅を設定
				breadth = i + 1;
				foreach(SGNode aNode in arrangeList){
					//自身と配置されている子供のうち、深さの浅い方をfrom, 深いほうをtoとする
					SGNode from, to;
					if (aNode.depth < depth){
						from = aNode;
						to = this;
					}else{
						from = this;
						to = aNode;
					}

					//fromとtoをつなぐ枝と同じ角度(縦横比)を求める
					int b = to.breadth - parent.breadth;
					int d = to.depth - parent.depth;

					if (b == 0){
						d = 1;
					}else{
						bool flip = false;
						if (b < 0){
							flip = true;
							b *= -1;
						}
						int gcd = MyUtility.Gcd(b, d);
						b /= gcd;
						d /= gcd;

						if (flip){
							b *= -1;
						}
					}
					if(aNode.depth < depth){
						aSlopeD = d;
						aSlopeB = b;
						//枝に角度を記録
						int index = parent.childList.IndexOf(this);
						SGEdge e = parent.edgeList[index];
						e.SetSlopeD(d);
						e.SetSlopeB(b);
					}else{
						//枝に角度を記録
						int index = childList.IndexOf(to);
						SGEdge e = edgeList[index];
						e.SetSlopeD(d);
						e.SetSlopeB(b);
					}

					//fromとtoを結んだ直線上かつtoより深さが下に配置されているオブジェクトを探す
					for(int j = to.depth-1 - d, k = to.breadth-1 - b; j >= 0 && k >= 0 && k < n ; j -= d, k -= b){
						//見つかった場合
						if(nodeMap[j, k] != null){
							//配置されているすべての子供との角度を調べる
							List<SGEdge> eList = nodeMap[j, k].edgeList;
							List<SGNode> cList = nodeMap[j, k].childList;
							int m = nodeMap[j,k].childList.Count;
							
							for(int l = 0 ; l < m ; l++){
								if(cList[l] == to || cList[l].breadth == -1){
									continue;
								}
								//角度がfromとtoの角度と等しく、fromより深さが上に子供がいる場合
								if (eList[l].GetSlopeB() == b && eList[l].GetSlopeD() == d
										&& from.depth < cList[l].depth){
									//枝を配置できない
									isArrangeable = false;
									break;
								}
							}
						}
						if (!isArrangeable){
							break;
						}
					}
					if (!isArrangeable){
						break;
					}
				}
				//配置可能だった場合
				if(isArrangeable){
					//nodeMapとarrangeableMapに登録
					nodeMap[depth-1, breadth-1] = this;
					arrangeableMap[depth-1,breadth-1] = true;
					if(parent != null){
						for(int b = parent.breadth-1, d = parent.depth-1 
							; d < this.depth 
							; b += aSlopeB, d += aSlopeD){

							arrangeableMap[d,b] = true;
						}
					}
					break;
				}
			}
		}


		foreach(SGNode c in childList){
			c.Modify3(this, arrangeableMap, nodeMap, list);
		}
	}
/*
	private bool IsArrangeable(bool[][] nodeMap, int b, int d){
	}*/

	public static void ModPos(List<List<SGNode>> list, List<SGNode> nodes){
		bool isNoMod = false;
		while(!isNoMod){
			SGNode.searchID++;
			foreach(SGNode node in nodes){
				isNoMod = node.Modify(list);
			}
		}
	}

	public static void ModPos2(bool[][] edgeMap, List<List<SGNode>> list, List<SGNode> nodes){
		SGNode.searchID++;
		foreach(SGNode node in nodes){
			node.Modify2(edgeMap, list);
		}
	}

	public static void ModPos3(bool[,] arrangeableMap, SGNode[,] nodeMap, List<List<SGNode>> list, List<SGNode> nodes){
		SGNode.searchID++;
		foreach(SGNode node in nodes){
			node.Modify3(null, arrangeableMap, nodeMap, list);
		}
	}

	public static void DeployNodes(int width, int height, int maxB, int maxD, List<SGNode> nodes){
		List<int> list = Enumerable.Repeat(1, maxD).ToList();
		SGNode.searchID++;
		foreach(SGNode node in nodes){
			node.Deploy(width, height, maxB, maxD, list);
		}
	}

	public static void DeployNodes2(int width, int height, int maxB, int maxD, List<SGNode> nodes){
		SGNode.searchID++;
		foreach(SGNode node in nodes){
			node.Deploy2(width, height, maxB, maxD);
		}
	}

	private void Deploy2(int width, int height, int maxB, int maxD){
		//探索済みのノードの場合、何もしない
		if(alreadySearchedID == searchID){
			return ;
		}
		//探索済みにする
		alreadySearchedID = searchID;

		//オブジェクトを移動
		Vector3 v = new Vector3(width * (breadth-1) / (float)maxB, height * (depth-1) / (float)maxD, 0.0f);
		//Debug.Log(v);
		gameObject.transform.localPosition = v;

		foreach(SGNode node in childList){
			node.Deploy2(width, height, maxB, maxD);
		}
	}

	private void Deploy(int width, int height, int maxB, int maxD, List<int> list){
		//探索済みのノードの場合、何もしない
		if(alreadySearchedID == searchID){
			return ;
		}
		//探索済みにする
		alreadySearchedID = searchID;

		//オブジェクトを移動
		int curB = list[depth-1];
		list[depth-1] = curB + 1 ;

		Vector3 v = new Vector3(width * (curB-1) / (float)maxB, height * (depth-1) / (float)maxD, 0.0f);
		Debug.Log(v);
		gameObject.transform.localPosition = v;

		foreach(SGNode node in childList){
			node.Deploy(width, height, maxB, maxD, list);
		}
	}

	public void Show(){
		//探索済みのノードの場合、何もしない
		if(alreadySearchedID == searchID){
			return;
		}
		//探索済みにする
		alreadySearchedID = searchID;

		foreach(SGEdge e in edgeList){
			e.DrawLine();
		}

		foreach(SGNode n in childList){
			n.Show();
		}
	}

	//枝を選択した色にする
	public void ShowEdge(SGNode node){
		foreach(SGEdge e in edgeList){
			if(e.IsDesNode(node)){
				e.gameObject.GetComponent<Renderer>().material.color = Color.blue;
				break;
			}
		}
	}

	public void HideEdge(SGNode node){
		foreach(SGEdge e in edgeList){
			if(e.IsDesNode(node)){
				e.gameObject.GetComponent<Renderer>().material.color = Color.green;
				break;
			}
		}
	}
}
