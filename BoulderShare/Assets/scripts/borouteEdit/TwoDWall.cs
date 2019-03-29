using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoDWall : BaseWall{
	[SerializeField] private SpriteRenderer wallImg;
	[SerializeField] private SpriteRenderer wallImgGS;
	[SerializeField] private WallManager wallManager;
	[SerializeField] private TwoDWallMarks twoDWallMarks;
	private int incline;

	public override void SetWallImage(Texture2D texture){
		Vector2 size = wallManager.GetMasterWallSize();
       	wallImg.sprite = Sprite.Create(
	        texture, 
	        new Rect(0.0f, 0.0f, texture.width, texture.height), 
	        new Vector2(0.5f, 0.5f),
	        texture.height/size.y);
	}
	public Texture2D GetWallImage(){
		return wallImg.sprite.texture;
	}

	public override void SetIncline(int inc){
		incline = inc;
	}

	public override void SetWallMarks(GameObject rootMarks, int n){
		twoDWallMarks.Synchronize(rootMarks, n);
	}
	public GameObject GetWallMarks(){
		return twoDWallMarks.GetWallMarks();
	}
/*
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
	}*/
}
