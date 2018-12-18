using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AtvDim : MonoBehaviour {
	[SerializeField]
	private GameObject twoDCamera;
	[SerializeField]
	private GameObject threeDCamera;
	[SerializeField]
	private ThreeD threeD;
	[SerializeField]
	private TwoDWallImage twoDWallImage;
	[SerializeField]
	private Text btnText;

	public void SwitchDimension(){
		bool is2dActive = twoDCamera.activeSelf;
		twoDCamera.SetActive(!is2dActive);
		threeDCamera.SetActive(is2dActive);

		if (is2dActive){
			threeD.ResetCamPos();
			btnText.text = "3D";
		}else{
			twoDWallImage.ResetCamPosAndDepth();
			btnText.text = "2D";
		}
	}
}
