using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//sceneGraphのノードを表すクラス

public class DAGNode : MonoBehaviour , IBDrawable, IBNode{
	private IBNodeData data;
	private List<IBEdge> parentEdgeList;
	private List<IBEdge> childEdgeList;
	private int searchID ;
	[SerializeField] private EdgeSet edgeSet;
	private int x;
	private int layer;
	private int upPriority;
	private int downPriority;
	private static int GlobalSearchID = 0;
	[SerializeField] private MeshRenderer rend;
	[SerializeField] private Color basicColor;
	[SerializeField] private Color focusColor;

	void Awake(){
		Debug.Log("Awake");
		parentEdgeList = new List<IBEdge>();
		childEdgeList = new List<IBEdge>();
		x = -1;
		layer = -1;
		upPriority = -1;
		downPriority = -1;
		searchID = -1;
	}

	public void SetData(IBNodeData d){
		data = d;
		gameObject.name = d.GetID() + "";		
	}

	public IBNodeData GetData(){
		return data;
	}

	public void ClearArrangeRecursively(){
		x = -1;
		layer = -1;
		upPriority = -1;
		downPriority = -1;
		foreach(IBEdge e in childEdgeList){
			IBNode n = e.GetDescendantNode();
			if (n != null){
				n.ClearArrangeRecursively();
			}
		}
	}

	public static IBNode Search(List<IBNode> list, IBNodeData data){
		GlobalSearchID++;
		foreach(IBNode node in list){
			IBNode cur = node.SearchDFS(data);
			if (cur != null){
				return cur;
			}
		}
		return null;
	}

	public static IBNode Search(IBNode node, IBNodeData data){
		GlobalSearchID++;
		return node.SearchDFS(data);
	}

	//引数と等しいdataを持つノードを探して返す
	public IBNode SearchDFS(IBNodeData d){
		//探索済みのノードの場合、何もしない
		if(searchID == GlobalSearchID){
			return null;
		}
		//探索済みにする
		searchID = GlobalSearchID;


		//探していたノードだった場合
		if (data.CompareTo(d) == 0){
			return this;
		}

		//再帰的に子ノードを探す
		foreach(IBEdge e in childEdgeList){
			IBNode node = e.GetDescendantNode();
			if (node != null){
				IBNode rNode = node.SearchDFS(d);
				if (rNode != null){
					return rNode;
				}
			}
		}
		return null;
	}

	//自分から見て引数childの親かどうか
	public bool IsParent(IBNode child){
		foreach(IBEdge e in childEdgeList){
			IBNode node = e.GetDescendantNode();
			if (node != null && node.CompareTo(child) == 0){
				return true;
			}
		}
		return false;
	}
	public int CompareTo(IBNode another){
		return data.CompareTo(another.GetData());
	}

	public void AddParent(IBNode parent){
		Debug.Log("addparent");
		IBEdge edge = edgeSet.MakeEdge(parent, this);
		parent.GetChildEdgeList().Add(edge);
		parentEdgeList.Add(edge);
	}

	//子ノードを追加する
	public void AddChild(IBNode child){
		IBEdge edge = edgeSet.MakeEdge(this, child);
		child.GetParentEdgeList().Add(edge);
		childEdgeList.Add(edge);
	}

	public void Remove(){
		//削除するエッジを一時保存
		List<IBEdge> remList = new List<IBEdge>();
		remList.AddRange(parentEdgeList);
		remList.AddRange(childEdgeList);
		
		//親と子の自分へ向かうエッジの参照の削除
		foreach(IBEdge e in parentEdgeList){
			e.GetAncestorNode().GetChildEdgeList().Remove(e);
		}
		foreach(IBEdge e in childEdgeList){
			e.GetDescendantNode().GetParentEdgeList().Remove(e);
		}
		//自分が持つエッジの参照の削除
		parentEdgeList.Clear();
		childEdgeList.Clear();

		//エッジを削除
		edgeSet.RemoveEdges(remList);
	
		Destroy(this.gameObject);
	}

	public void RemoveEdge(List<IBEdge> list){
		foreach(IBEdge e in list){
			RemoveEdge(e);
		}
	}

	public void RemoveEdge(IBEdge edge){
		if (childEdgeList.Contains(edge)){
			edge.GetDescendantNode().GetParentEdgeList().Remove(edge);
			childEdgeList.Remove(edge);
		}else if (parentEdgeList.Contains(edge)){
			edge.GetAncestorNode().GetChildEdgeList().Remove(edge);
			parentEdgeList.Remove(edge);
		}

		edgeSet.RemoveEdge(edge);
	}

	public List<IBEdge> GetParentEdgeList(){
		return parentEdgeList;
	}
	public List<IBEdge> GetChildEdgeList(){
		return childEdgeList;
	}

//IBArrangeable
	public int GetPosition(){
		return x;
	}
	public bool SetPosition(int p){
		x = p;
		return true;
	}
	public int GetLayer(){
		return layer;
	}
	public void SetLayer(int l){
		layer = l;
	}
	public void SetUpPriority(int p){
		upPriority = p;
	}
	public void SetDownPriority(int p){
		downPriority = p;
	}
	public int GetUpPriority(){
		return upPriority;
	}
	public int GetDownPriority(){
		return downPriority;
	}
	//上重心を計算
	public float CalcUpperCOG(){
		float cog = 0.0f;
		foreach(IBEdge e in parentEdgeList){
			cog += e.GetAncestorNode().GetPosition();
		}

		return cog / parentEdgeList.Count;
	}

	//下重心を計算
	public float CalcLowerCOG(){
		float cog = 0.0f;
		foreach(IBEdge e in childEdgeList){
			cog += e.GetDescendantNode().GetPosition();
		}

		return cog / childEdgeList.Count;
	}

	public int SetLayerRecursively(int l){
		int max ;

		if (layer < l){
			layer = l;
		}
		max = layer;

		foreach(IBEdge e in childEdgeList){
			int tmp = e.GetDescendantNode().SetLayerRecursively(l+1);
			if (max < tmp){
				max = tmp;
			}
		}
		return max;
	}

	public static void AssignNodeRecursively(List<IBNode> list, List<IBNode>[] layers){
		GlobalSearchID++;
		foreach(IBNode node in list){
			node.AssignNodeRecursively(layers);
		}
	}

	public void AssignNodeRecursively(List<IBNode>[] arrangeLists){
		if (searchID == GlobalSearchID){
			return ;
		}
		searchID = GlobalSearchID;

		arrangeLists[layer].Add(this);

		foreach(IBEdge e in childEdgeList){
			e.GetDescendantNode().AssignNodeRecursively(arrangeLists);
		}
	}

//IDwarbale
	public void Draw(){
		gameObject.SetActive(true);
		DontFocus();
	}

	public void DontDraw(){
		gameObject.SetActive(false);
	}
	
	public void Focus(){
		rend.material.color = focusColor;
	}

	public void DontFocus(){
		rend.material.color = basicColor;
	}
}
