using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeSet : MonoBehaviour
{
    private Dictionary<IBNode, GameObject> masterNodeMap;
	[SerializeField] private GameObject nodePrefab;
	[SerializeField] private GameObject dummyNodePrefab;
	[SerializeField] private GameObject nodeSet;
	private int dummyNum;

    void Awake(){
    	masterNodeMap = new Dictionary<IBNode, GameObject>();
    	dummyNum = 0;
    }

    public void Init(){
    	masterNodeMap.Clear();
    	foreach(Transform t in nodeSet.transform){
    		Destroy(t.gameObject);
    	}
    	dummyNum = 0;
    }

	public IBNode MakeNode(IBNodeData data, IBNode parent, IBNode child){
		GameObject obj = Instantiate(nodePrefab, nodeSet.transform);
		obj.name = ""+data.GetID();
		obj.SetActive(true);
		
		IBNode node = obj.GetComponent(typeof(IBNode)) as IBNode;
		masterNodeMap.Add(node, obj);
		node.SetData(data);
		if (parent != null){
			node.AddParent(parent);
		}
		if (child != null){
			node.AddChild(child);
		}
		return node;
	}
	public IBNode MakeDummyNode(IBNodeData data, IBNode parent, IBNode child){
		dummyNum++;
		GameObject obj = Instantiate(dummyNodePrefab, nodeSet.transform);
		obj.name = ""+(-dummyNum);
		obj.SetActive(true);

		IBNode node = obj.GetComponent(typeof(IBNode)) as IBNode;
		masterNodeMap.Add(node, obj);
		node.SetData(data);
		if (parent != null){
			node.AddParent(parent);
		}
		if (child != null){
			node.AddChild(child);
		}
		return node;
	}
	public void RemoveNode(IBNode node){
		if (masterNodeMap.ContainsKey(node)){
			GameObject obj = masterNodeMap[node];
			masterNodeMap.Remove(node);
			Destroy(obj);
		}
	}

	public void DontFocusAll(){
		foreach(GameObject obj in masterNodeMap.Values){
			DAGNode node = obj.GetComponent<DAGNode>();
			node.DontFocus();
		}		
	}

}
