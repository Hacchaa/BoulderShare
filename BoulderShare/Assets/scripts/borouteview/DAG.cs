using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DAG : MonoBehaviour
{
    [SerializeField] GameObject edgeSet;
    [SerializeField] GameObject nodeSet;
    [SerializeField] GameObject nodePrefab;
    private List<IBNode> rootList;
    private IBNode currentTarget;

    void Awake(){
    	rootList = new List<IBNode>();
    	currentTarget = null;
    }

    public void Add(IBNodeData data){
    	if (currentTarget == null){
    		return ;
    	}

    	GameObject obj = Instantiate(nodePrefab, nodeSet.transform);
    	DAGNode n = obj.GetComponent<DAGNode>();
    	if (n != null){
    		n.SetData(data);
    		currentTarget.AddParent(n);
    	}
    }

    public void Remove(){
    	/*
    	if ()
		foreach(IBEdge eParent in parentEdgeList){
			IBNode parent = eParent.GetAnsectorNode();
			foreach(IBEdge eChild in childEdgeList){
				IBNode child = eChild.GetDescendantNode();
				parent.AddChild(child);
			}
		}
		foreach(IBEdge e in parentEdgeList){
			e.GetAnsectorNode().GetChildEdgeList().Remove(e);
		}
		parentEdgeList.Clear();

		foreach(IBEdge e in childEdgeList){
			e.GetDescendantNode().GetParentEdgeList().Remove(e);
		}
		childEdgeList.Clear();

		Destory(this.gameObject);*/
	}

	public void InsertParent(IBNodeData data){
		if (currentTarget == null){
    		return ;
    	}

    	GameObject obj = Instantiate(nodePrefab, nodeSet.transform);
    	DAGNode parent = obj.GetComponent<DAGNode>();
    	if (parent != null){
    		parent.SetData(data);
    		foreach(IBEdge e in currentTarget.GetParentEdgeList()){
				e.SetDescendantNode(parent);
			}
			parent.GetParentEdgeList().AddRange(currentTarget.GetParentEdgeList());
			currentTarget.GetParentEdgeList().Clear();
			IBEdge edge = new BEdge(parent, currentTarget);
			parent.GetChildEdgeList().Add(edge);
			currentTarget.GetParentEdgeList().Add(edge);
    	}
	}
}
