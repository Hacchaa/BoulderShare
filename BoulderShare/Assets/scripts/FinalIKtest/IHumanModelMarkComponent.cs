using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHumanModelMarkComponent
{
	Vector3 GetPosition();
	void SetPosition(Vector3 p);
	void InitPosition();
	Vector3 GetInitPosition();
	void SetCamera(Camera camera);
	void SetAvatar(Transform t);
	MyUtility.FullBodyMark GetBodyID();
}
