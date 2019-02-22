using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edit3DSceneComment : MonoBehaviour, IUIComponent {
	[SerializeField]
	private List<GameObject> externalUIComponents;
	[SerializeField]
	private SceneCommentController3D scc;
	[SerializeField]
	private ThreeDWall threeDWall;
	[SerializeField]
	private GameObject colorSetter;
	[SerializeField]
	private GameObject FontSizeSetter;
	[SerializeField]
	private ScreenTransitionManager sManager;
	[SerializeField] private SceneComments3D comments;


	public void CommentLookAtCamera(){
		scc.CommentLookAtCamera();
	}
	public void Submit(){
		sManager.Transition("EditPose");
	}

	public void ShowProc(){
		foreach(GameObject obj in externalUIComponents){
			obj.SetActive(true);
		}

		this.gameObject.SetActive(true);

		colorSetter.SetActive(false);
		FontSizeSetter.SetActive(false);
		scc.AcceptEvents();
		comments.ShowDynamically();
		comments.SetShowAngle(SceneComments3D.ANGLE_EDIT);
	}

	public void HideProc(){
		Hide();
	}

	public void Hide(){
		foreach(GameObject obj in externalUIComponents){
			obj.SetActive(false);
		}
		this.gameObject.SetActive(false);
		scc.Release();
		scc.IgnoreEvents();
	}

	public void AddComment(){
		scc.MakeComment();
	}
}
