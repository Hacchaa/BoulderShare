using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommentButton : MonoBehaviour {
	public GameObject comment;
	public SceneFocus sf;

	public void OpenComment(){
		comment.SetActive(true);
		sf.SetCommentToIF();
	}

	public void Submit(){
		sf.GetCommentFromIF();
		comment.SetActive(false);
	}

	public void Cancel(){
		comment.SetActive(false);
	}
}
