using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoDWall : BaseWall{
	[SerializeField] private WallManager wallManager;
	[SerializeField] private TwoDWallMarks twoDWallMarks;
	[SerializeField] private TwoDWallImage twoDWallImage;
	[SerializeField] private SpriteRenderer translucentWall;
	private int incline;

	public override void ShowTranslucentWall(){
		translucentWall.gameObject.SetActive(true);
	}

	public override void HideTranslucentWall(){
		translucentWall.gameObject.SetActive(false);
	}

	public override void SetWallImage(Texture2D texture){
		twoDWallImage.SetWallImage(texture);
	}

	public override void SetIncline(int inc){
		incline = inc;
	}

	public override void SetWallMarks(GameObject rootMarks){
		twoDWallMarks.Synchronize(rootMarks);
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
