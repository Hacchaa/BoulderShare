using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBNode 
{
	IBNode SearchDFS(IBNodeData d);
	int CompareTo(IBNode another);
	bool IsParent(IBNode child);
	void AddParent(IBNode parent);
	void AddChild(IBNode child);
	void RemoveEdge(IBEdge e);
	void RemoveEdge(List<IBEdge> list);
	List<IBEdge> GetParentEdgeList();
	List<IBEdge> GetChildEdgeList();
	IBNodeData GetData();
	void SetData(IBNodeData data);
	int GetPosition();
	bool SetPosition(int p);
	int GetLayer();
	void SetLayer(int l);
	float CalcUpperCOG();
	float CalcLowerCOG();
	int SetLayerRecursively(int l);
	void AssignNodeRecursively(List<IBNode>[] arrangeLists);
	void ClearArrangeRecursively();
	void SetUpPriority(int p);
	void SetDownPriority(int p);
	int GetUpPriority();
	int GetDownPriority();
}
