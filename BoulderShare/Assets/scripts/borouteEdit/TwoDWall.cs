using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoDWall : MonoBehaviour {
	[SerializeField]
	private SpriteRenderer wallImg;
	[SerializeField]
	private ThreeDWall threeDWall;

	public Bounds GetWallBounds(){
		return wallImg.bounds;
	}

	public SpriteRenderer GetWall(){
		return wallImg;
	}

	public Texture2D GetWallTexture(){
		return wallImg.sprite.texture;
	}

	public void SetWallSprite(Sprite spr){
		wallImg.sprite = spr;
		AfterLoadingImage();
	}

	private void AfterLoadingImage(){
		threeDWall.SetWall();
	}
}
