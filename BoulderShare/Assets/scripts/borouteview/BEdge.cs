using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BEdge : IBEdge
{
	private IBNode parent;
	private IBNode child;

	public BEdge(){
		parent = null;
		child = null;
	}
	public BEdge(IBNode p, IBNode c){
		parent = p;
		child = c;
	}

	public IBNode GetDescendantNode(){
		return child;
	}
    public IBNode GetAncestorNode(){
    	return parent;
    }
    public void SetDescendantNode(IBNode node){
    	child = node;
    }
    public void SetAncestorNode(IBNode node){
    	parent = node;
    }
}
