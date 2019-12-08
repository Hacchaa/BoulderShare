using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SceneFocus : MonoBehaviour {
	private int choiced ;
	public SceneFocusElem[] focusElems;
	private Hold[] curFocusHolds;
	public AvatarControl ac;

	// Use this for initialization
	void Awake () {
		choiced = (int)AvatarControl.BODYS.NONE;
		curFocusHolds = new Hold[4];
	}
	
	public int GetChoice(){
		return choiced;
	}

	public void SetChoice(int choice){
		choiced = choice;
	}

	//holdが左右手足に触れているかどうかを表示する
	public void LoadOnHold(Hold hold){
		for(int i = (int)AvatarControl.BODYS.RH ; i <= (int)AvatarControl.BODYS.LF ; i++){
			if (curFocusHolds[i] == hold){
				focusElems[i].Emphasis();
			}else{
				focusElems[i].DeEmphasis();
			}
		}
		choiced = (int)AvatarControl.BODYS.NONE;

		if (hold.gameObject.tag == "Hold_Normal"){
			focusElems[0].gameObject.SetActive(true);
			focusElems[1].gameObject.SetActive(true);
		}else if(hold.gameObject.tag == "Hold_Foot"){
			focusElems[0].gameObject.SetActive(false);
			focusElems[1].gameObject.SetActive(false);
		}
	}

	public void Registration(Hold hold){
		if (choiced != (int)AvatarControl.BODYS.NONE){
			ac.SetFixed(choiced, false);
			if (curFocusHolds[choiced] == hold){
				hold.SetBodyActive(choiced, false);
				curFocusHolds[choiced] = null;
			}else{
				SetFocusHold(choiced, hold);
			}
		}
	}

	public void Reset(){
		for(int i = 0 ; i < focusElems.Length ; i++){
			focusElems[i].DeEmphasis();
		}

		for(int i = (int)AvatarControl.BODYS.RH ; i <= (int)AvatarControl.BODYS.LF ; i++){
			if (curFocusHolds[i] != null){
				curFocusHolds[i].SetBodyActive(i, false);
				curFocusHolds[i] = null;
			}
		}
	}

	public void SetFocusHold(int index, Hold hold){
		hold.SetBodyActive(index, true);
		if (curFocusHolds[index] != null){
			curFocusHolds[index].SetBodyActive(index, false);
		}
		curFocusHolds[index] = hold;
	}

	public Hold[] GetFocusHold(){
		return curFocusHolds;
	}
}
