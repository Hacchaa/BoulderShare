using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SceneFocus : MonoBehaviour {
	private int choiced ;
	public SceneFocusElem[] focusElems;
	public enum Choice{None=-1,RH, LH, RF, LF};
	private Hold[] curFocusHolds;
	private Transform trans_Holds;
	//public Comment comment;
	private string curComment;

	// Use this for initialization
	void Start () {
		choiced = (int)Choice.None;
		curFocusHolds = new Hold[4];
		trans_Holds = GameObject.Find("Wall").transform.Find("Holds");
	}
	
	public int GetChoice(){
		return choiced;
	}

	public void SetChoice(int choice){
		choiced = choice;
	}

	public void LoadOnHold(Hold hold){
		bool[] onHolds = hold.GetOnHold();
		for(int i = (int)Choice.RH ; i <= (int)Choice.LF ; i++){
			if (onHolds[i]){
				focusElems[i].Emphasis();
			}else{
				focusElems[i].DeEmphasis();
			}
		}
		choiced = (int)Choice.None;

		if (hold.gameObject.tag == "Hold_Normal"){
			focusElems[0].gameObject.SetActive(true);
			focusElems[1].gameObject.SetActive(true);
		}else if(hold.gameObject.tag == "Hold_Foot"){
			focusElems[0].gameObject.SetActive(false);
			focusElems[1].gameObject.SetActive(false);
		}
	}

	public void Registration(Hold hold){
		if (choiced != (int)Choice.None){
			bool[] onHolds = hold.GetOnHold();

			if (onHolds[choiced]){
				hold.SetOnHold(choiced, false);
				curFocusHolds[choiced] = null;
			}else{
				hold.SetOnHold(choiced, true);
				if (curFocusHolds[choiced] != null){
					curFocusHolds[choiced].SetOnHold(choiced, false);
				}
				curFocusHolds[choiced] = hold;
			}
		}
	}

	private void Reset(){
		for(int i = 0 ; i < focusElems.Length ; i++){
			focusElems[i].DeEmphasis();
		}

		for(int i = (int)Choice.RH ; i <= (int)Choice.LF ; i++){
			if (curFocusHolds[i] != null){
				curFocusHolds[i].SetOnHold(i, false);
				curFocusHolds[i] = null;
			}
		}
	}

	public void LoadScene(HScene scene){
		Reset();

		string[] holds = scene.GetOnHolds();
		//Debug.Log(holds);
		for(int i = (int)Choice.RH ; i <= (int)Choice.LF ; i++){
			if (holds[i] != null && holds[i].Length > 0){
				Transform t = trans_Holds.Find(holds[i]);
				if (t != null){
					curFocusHolds[i] = t.gameObject.GetComponent<Hold>();
					curFocusHolds[i].SetOnHold(i, true);
				}
			}
		}
		//curComment = scene.GetComment();
	}

	public void SaveScene(HScene scene){
		scene.SaveOnHolds(curFocusHolds);
		//scene.SaveComment(curComment);
	}

	public void SetCommentToIF(){
		//comment.SetComment(curComment);
	}

	public void GetCommentFromIF(){
		//curComment = comment.GetComment();
	}
}
