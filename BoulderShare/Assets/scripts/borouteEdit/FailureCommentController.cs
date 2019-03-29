using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FailureCommentController : MonoBehaviour
{
	[SerializeField] private GameObject commentPrefab;
	[SerializeField] private GameObject commentRoot;
	[SerializeField] private GameObject commentView;

    public void SetFailureComments(List<string> list){
    	DeleteComments();

    	if(!list.Any()){
    		commentView.SetActive(false);
    		return ;
    	}

    	commentView.SetActive(true);

    	foreach(string str in list){
    		if (!string.IsNullOrEmpty(str)){
    			GameObject obj = Instantiate(commentPrefab, commentRoot.transform);
    			obj.SetActive(true);
    			obj.GetComponent<FailureComment>().SetText(str);
    		}
    	}
    }

    public void DeleteComments(){
    	foreach(Transform t in commentRoot.transform){
    		Destroy(t.gameObject);
    	}
    }
}
