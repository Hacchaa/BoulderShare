using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBArrangeableNode : IBNode
{
	int GetPosition();
	void SetPosition(int p);
	int GetLayer();
	void SetLayer(int l);
	float CalcUpperCOG();
	float CalcLowerCOG();
}
