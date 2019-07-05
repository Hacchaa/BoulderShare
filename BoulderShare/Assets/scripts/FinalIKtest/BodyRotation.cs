using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BodyRotation : FBBIKBase
{
	[SerializeField] private float rotateWeight = 0.5f;
	[SerializeField] private Transform spineAvatar;
	[SerializeField] private Transform neckAvatar;
	[SerializeField] private Transform model;
    public override void OnDrag(PointerEventData data){
    	if (finger == data.pointerId){
			//カメラから見た軸の傾きを求める
			Vector3 camAxis = cam.WorldToScreenPoint(neckAvatar.position) - cam.WorldToScreenPoint(spineAvatar.position);
			camAxis = camAxis.normalized;
			float dot = Vector2.Dot(new Vector2(-camAxis.y, camAxis.x), data.delta);
			model.Rotate(Vector3.up * dot * rotateWeight);
		}
    }
}
