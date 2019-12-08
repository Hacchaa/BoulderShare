using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SceneNumIcon : MonoBehaviour {
	private Image myImage ;
	private static float alpha = 100.0f / 255.0f;

	// Use this for initialization
	void Awake () {
		myImage = GetComponent<Image>();
	}

	public void DeSelect(){
		myImage.color = new Color(1.0f, 1.0f, 1.0f, alpha);
	}

	public void Select(){
		myImage.color = Color.white;
	}

}
