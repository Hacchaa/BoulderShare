using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATWarning : MonoBehaviour
{
	[SerializeField] EditorPopup popup;

    private string warningTitle;
    private string warningSup;

    public void SetWarning(string title, string sup){
    	warningTitle = title;
    	warningSup = sup;
    }

    public void Clear(){
    	warningTitle = "";
    	warningSup = "";
    }

    public void OpenWarning(){
    	popup.Open(null, null, warningTitle, warningSup, "OK", "");
    }
}
