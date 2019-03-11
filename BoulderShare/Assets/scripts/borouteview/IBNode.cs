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
	List<IBEdge> GetParentEdgeList();
	List<IBEdge> GetChildEdgeList();
	void ResetAlreadySearched();
	IBNodeData GetData();
}
