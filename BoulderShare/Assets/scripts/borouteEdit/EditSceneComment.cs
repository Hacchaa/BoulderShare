using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditSceneComment : MonoBehaviour, IUIComponent {
	[SerializeField]
	private List<GameObject> externalUIComponents;
	[SerializeField]
	private SceneCommentController scc;
	[SerializeField]
	private TwoDWallImage twoDWallImage;
	[SerializeField]
	private GameObject raycastBlocker;
	[SerializeField]
	private GameObject colorSetter;
	[SerializeField]
	private GameObject FontSizeSetter;
	[SerializeField]
	private ScreenTransitionManager sManager;

	public void Submit(){
		sManager.Transition("EditScene");
	}

	public void ShowProc(){
		foreach(GameObject obj in externalUIComponents){
			obj.SetActive(true);
		}

		this.gameObject.SetActive(true);

		twoDWallImage.ResetCamPosAndDepth();
		twoDWallImage.IgnoreEvents();
		raycastBlocker.SetActive(true);
		colorSetter.SetActive(false);
		FontSizeSetter.SetActive(false);
		scc.AcceptEvents();
	}

	public void HideProc(){
		Hide();
	}

	public void Hide(){
		foreach(GameObject obj in externalUIComponents){
			obj.SetActive(false);
		}
		this.gameObject.SetActive(false);
		twoDWallImage.AcceptEvents();
		scc.Release();
		scc.IgnoreEvents();
		raycastBlocker.SetActive(false);
	}

	public void AddComment(){
		scc.MakeComment();
	}
}
