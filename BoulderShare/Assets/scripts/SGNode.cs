using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//sceneGraphのノードを表すクラス

public class SGNode : MonoBehaviour {
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
	private GameObject nodeSet;
	private GameObject edgeSet;

	void Awake(){
		edgeList = new List<SGEdge>();
		childList = new List<SGNode>();
		parentList = new List<SGNode>();
	}
	void Start(){
		nodeSet = GameObject.FindGameObjectsWithTag("SGNodeSet")[0];
		edgeSet = GameObject.FindGameObjectsWithTag("SGEdgeSet")[0];
	}

	//子ノードを作成する
	public void AddChildNode(){
		GameObject child = Instantiate(sGNodePrefab, nodeSet.transform);
		GameObject edge = Instantiate(sGEdgePrefab, edgeSet.transform);

		//子供と自分自身をエッジで結ぶ
	}
}
