using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BEdge : MonoBehaviour, IBEdge, IBDrawable
{
	private IBNode parent;
	private IBNode child;

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

    public void Draw(){

	}

	public void DontDraw(){

	}
	
	public void Focus(){

	}

	public void DontFocus(){
		
	}
}
