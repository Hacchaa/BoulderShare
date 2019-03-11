using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//sceneGraphのノードを表すクラス

public class DAGNode : MonoBehaviour , IBNode{
	private IBNodeData data;
	private List<IBEdge> parentEdgeList;
	private List<IBEdge> childEdgeList;
	private bool alreadySearched ;

	void Awake(){
		parentEdgeList = new List<IBEdge>();
		childEdgeList = new List<IBEdge>();
		alreadySearched = false;
	}

	public void SetData(IBNodeData d){
		data = d;
		gameObject.name = d.GetID() + "";		
	}

	public IBNodeData GetData(){
		return data;
	}

	public void ResetAlreadySearched(){
		alreadySearched = false;
		foreach(IBEdge e in childEdgeList){
			IBNode n = e.GetDescendantNode();
			if (n != null){
				n.ResetAlreadySearched();
			}
		}
	}

	//引数と等しいdataを持つノードを探して返す
	public IBNode SearchDFS(IBNodeData d){
		//探索済みのノードの場合、何もしない
		if(alreadySearched){
			return null;
		}
		//探索済みにする
		alreadySearched = true;


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
		IBEdge edge = new BEdge(parent, this);
		parent.GetChildEdgeList().Add(edge);
		parentEdgeList.Add(edge);
	}

	//子ノードを追加する
	public void AddChild(IBNode child){
		IBEdge edge = new BEdge(this, child);
		child.GetParentEdgeList().Add(edge);
		childEdgeList.Add(edge);
	}

	public void Remove(){
		foreach(IBEdge eParent in parentEdgeList){
			IBNode parent = eParent.GetAncestorNode();
			foreach(IBEdge eChild in childEdgeList){
				IBNode child = eChild.GetDescendantNode();
				parent.AddChild(child);
			}
		}
		foreach(IBEdge e in parentEdgeList){
			e.GetAncestorNode().GetChildEdgeList().Remove(e);
		}
		parentEdgeList.Clear();

		foreach(IBEdge e in childEdgeList){
			e.GetDescendantNode().GetParentEdgeList().Remove(e);
		}
		childEdgeList.Clear();

		Destroy(this.gameObject);
	}

	public List<IBEdge> GetParentEdgeList(){
		return parentEdgeList;
	}
	public List<IBEdge> GetChildEdgeList(){
		return childEdgeList;
	}
	
}
