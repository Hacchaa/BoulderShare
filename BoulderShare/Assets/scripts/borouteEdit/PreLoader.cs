using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PreLoader : MonoBehaviour
{
	[SerializeField] Transform root;
	[SerializeField] Transform loadIcon;
	[SerializeField] float interval= 0.1f;
	[SerializeField] float delta = 45.0f;
	private Coroutine cor = null;

    public void LockScreen(){
    	root.gameObject.SetActive(true);
    	cor = StartCoroutine(LoadAnim());
    }

	private IEnumerator LoadAnim(){
		while(true){
			loadIcon.Rotate(0.0f, 0.0f, delta);
			yield return new WaitForSeconds(interval);
		}
	}

	public void UnLockScreen(){
		if (cor != null){
			StopCoroutine(cor);
		}
		root.gameObject.SetActive(false);
	}
}
