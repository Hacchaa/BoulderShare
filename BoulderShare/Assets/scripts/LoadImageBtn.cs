using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadImageBtn : MonoBehaviour {
	public BoRouteLSManager bManager;

	void Start(){
		//ボルートが新規作成でない場合、表示しない
		if (!bManager.IsNew()){
			this.gameObject.SetActive(false);
		}
	}
}