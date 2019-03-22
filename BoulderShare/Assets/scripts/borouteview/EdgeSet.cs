using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EdgeSet : MonoBehaviour
{
    private Dictionary<IBEdge, GameObject> masterEdgeMap;
	[SerializeField] private GameObject edgePrefab;
	[SerializeField] private GameObject edgeSet;

    void Awake(){
    	masterEdgeMap = new Dictionary<IBEdge, GameObject>();
    }
    public void Init(){
    	masterEdgeMap.Clear();
    	foreach(Transform t in edgeSet.transform){
    		Destroy(t.gameObject);
    	}
    }

	public IBEdge MakeEdge(IBNode from, IBNode to){
		GameObject obj = Instantiate(edgePrefab, edgeSet.transform);
		obj.name = from.GetData().GetID() + "_" + to.GetData().GetID();
		obj.SetActive(true);
		
		IBEdge edge = obj.GetComponent(typeof(IBEdge)) as IBEdge;
		masterEdgeMap.Add(edge, obj);
		edge.SetAncestorNode(from);
		edge.SetDescendantNode(to);
		return edge;
	}

	public void RemoveEdge(IBEdge e){
		if (masterEdgeMap.ContainsKey(e)){
			GameObject obj = masterEdgeMap[e];
			masterEdgeMap.Remove(e);
			Destroy(obj);
		}
	}

	public void RemoveEdges(List<IBEdge> list){
		foreach(IBEdge e in list){
			RemoveEdge(e);
		}
	}

	public void DrawEdges(){
		foreach(GameObject obj in masterEdgeMap.Values){
			DAGEdge edge = obj.GetComponent<DAGEdge>();
			edge.Draw();
		}
	}

	public void DontFocusAll(){
		foreach(GameObject obj in masterEdgeMap.Values){
			DAGEdge edge = obj.GetComponent<DAGEdge>();
			edge.DontFocus();
		}		
	}

}
