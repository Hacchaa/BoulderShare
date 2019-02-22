using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoDWall : MonoBehaviour {
	[SerializeField]
	private SpriteRenderer wallImg;
	[SerializeField]
	private SpriteRenderer wallImgGS;
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
		wallImgGS.sprite = spr;
		AfterLoadingImage();
	}

	public void OverWriteWallTexture(Texture2D texture){
		wallImg.sprite = Sprite.Create(
	        texture, 
	        new Rect(0.0f, 0.0f, texture.width, texture.height), 
	        new Vector2(0.5f, 0.5f),
	        wallImg.sprite.pixelsPerUnit);
		wallImgGS.sprite = wallImg.sprite;
		AfterLoadingImage();
	}

	private void AfterLoadingImage(){
		threeDWall.SetWall();
	}
}
