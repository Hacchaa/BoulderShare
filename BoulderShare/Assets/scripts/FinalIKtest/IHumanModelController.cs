using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHumanModelController
{
	void InitMarks();
	Vector3 GetInitPosition(MyUtility.FullBodyMark mark);
	Vector3[] GetPositions();
	Quaternion[] GetRotations();
	void SetPose(Vector3[] vec, Quaternion[] rots);

}
