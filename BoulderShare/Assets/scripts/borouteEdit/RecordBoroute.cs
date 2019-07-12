using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityObject = UnityEngine.Object;


[RequireComponent(typeof(Camera)), DisallowMultipleComponent]
public class RecordeBoroute : MonoBehaviour
{
	private List<RenderTexture> sceneRT;
	private bool isNeedToSaveRT = false;
	private bool isSuccess = false;
	private int width;
	private int height;

	public void SetSize(int w){
		width = w;
		height = Mathf.RoundToInt(width / GetComponent<Camera>().aspect);
	}

	public void SaveRT(){
		isNeedToSaveRT = true;
		isSuccess = false;
	}
	public bool IsSuccess(){
		return isSuccess;
	}
	public List<RenderTexture> GetSceneRT(){
		return sceneRT;
	}

	public void Init()
	{
		sceneRT = new List<RenderTexture>();
		isNeedToSaveRT = false;
		isSuccess = false;
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!isNeedToSaveRT){
			Graphics.Blit(source, destination);
			return;
		}

		RenderTexture rt = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
		rt.wrapMode = TextureWrapMode.Clamp;
		rt.filterMode = FilterMode.Bilinear;

		Graphics.Blit(source, rt);
		sceneRT.Add(rt);

		Graphics.Blit(source, destination);
		isNeedToSaveRT = false;
		isSuccess = true;
	}
}
