using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallImg : MonoBehaviour {
	[SerializeField]
	private SpriteRenderer sRenderer;

	void Start(){
	}
	public Texture2D GetTexture(){
		return sRenderer.sprite.texture;
	}

	public void SetSprite(Sprite spr){
		sRenderer.sprite = spr;
	}
}
