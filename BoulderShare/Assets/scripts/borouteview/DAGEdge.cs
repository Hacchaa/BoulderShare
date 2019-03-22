using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DAGEdge: MonoBehaviour, IBEdge, IBDrawable
{
	private IBNode parent;
	private IBNode child;
	[SerializeField] private LineRenderer line;
	[SerializeField] private Color basicColor;
	[SerializeField] private Color focusColor;

	public int parentID = -1;
	public int childID = -1;

	public IBNode GetDescendantNode(){
		return child;
	}
    public IBNode GetAncestorNode(){
    	return parent;
    }
    public void SetDescendantNode(IBNode node){
    	child = node;
    	if (child != null){
    		childID = child.GetData().GetID();
    	}
    }
    public void SetAncestorNode(IBNode node){
    	parent = node;
    	if (parent != null){
    		parentID = parent.GetData().GetID();
    	}
    }

    public void Draw(){
    	Vector3[] p = new Vector3[2];
    	p[0] = new Vector3(parent.GetPosition(), parent.GetLayer(), 0.0f);
    	p[1] = new Vector3(child.GetPosition(), child.GetLayer(), 0.0f);
    	line.SetPositions(p);
		gameObject.SetActive(true);
		line.gameObject.SetActive(true);
		DontFocus();
	}

	public void DontDraw(){
		gameObject.SetActive(false);
		line.gameObject.SetActive(false);
	}
	
	public void Focus(){
		line.startColor = focusColor;
		line.endColor = focusColor;
	}

	public void DontFocus(){
		line.startColor = basicColor;
		line.endColor = basicColor;		
	}
}
