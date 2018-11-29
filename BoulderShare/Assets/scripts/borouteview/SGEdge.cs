using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//sceneGraphの枝を表すオブジェクト
public class SGEdge : MonoBehaviour {
	//枝から見て子供側のノード
	[SerializeField]
	private SGNode descendantNode;
	//枝から見て祖先側のノード
	[SerializeField]
	private SGNode ancestorNode;
	//エッジを視覚的に表現する SGEdgeがちょうど一つ持つオブジェクト
	[SerializeField]
	private LineRenderer lineRend;

	private int slopeB = -1;
	private int slopeD = -1;

	public void SetSlopeB(int b){
		slopeB = b;
	}

	public void SetSlopeD(int d){
		slopeD = d;
	}

	public int GetSlopeB(){
		return slopeB;
	}

	public int GetSlopeD(){
		return slopeD;
	}

	//ノードを登録するメソッド
	//以前に登録してあったノードの処理はしない
	//SGNode null許容
	public void RegistrateDescendantNode(SGNode node){
		descendantNode = node;
		//グラフィカルな部分の処理
	}
	public void RegistrateAncestorNode(SGNode node){
		ancestorNode = node;
		//グラフィカルな部分の処理
	}

	public bool IsDesNode(SGNode node){
		if (descendantNode == null){
			return false;
		}
		return descendantNode.GetData().GetID() == node.GetData().GetID();
	}

	//線を引く
	public void DrawLine(){
		//どちらかがnullならば、線を引かない
		if(descendantNode == null || ancestorNode == null){
			lineRend.SetPositions(new Vector3[2]);
			return ;
		}
		Vector3[] p = new Vector3[2];
		p[0] = ancestorNode.gameObject.transform.localPosition;
		p[1] = descendantNode.gameObject.transform.localPosition;

		lineRend.SetPositions(p);
	}
}
