using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class STAnimation_ModifyMarks_ATMenu : STAnimationFromBotToTopReverse
{
    [SerializeField] private RectTransform cavnasRoot;
    [SerializeField] private RectTransform ss2DFrom;
    [SerializeField] private RectTransform ss3DTo;
    [SerializeField] private CameraManager cManager;

    private RectTransform fromImage;
    private RectTransform toImage;

    public override void Play(){
        OnPostCompleteAction = CompleteAction;
    	StartCoroutine(TakeScreenShot());
    }

    private IEnumerator TakeScreenShot(){
 
        cManager.StartSS2DFrom();
 
    	yield return new WaitForEndOfFrame();
        //from用rendertexture終了
        fromImage = ss2DFrom;
        cManager.EndSS2DFrom();

        fromImage.SetParent(fromRect);
        fromImage.SetAsFirstSibling();
        ResetRectPos(fromImage);
        fromImage.gameObject.SetActive(true);
        PrioritizeFrom();
        from.OnPreHide();
        to.ShowScreen();

        cManager.StartSS3DTo();

        yield return new WaitForEndOfFrame();

        toImage = ss3DTo;
        cManager.EndSS3DTo();
   
        toImage.SetParent(toRect);
        toImage.SetAsFirstSibling();
        ResetRectPos(toImage);
        toImage.gameObject.SetActive(true);

    	Animate();
    }

    private void CompleteAction(){
        from.HideScreen();
        //to.Show();

        if (fromImage != null){
            fromImage.SetParent(cavnasRoot);
            fromImage.SetAsFirstSibling();
            ResetRectPos(fromImage);
            fromImage.gameObject.SetActive(false);
        }
        if (toImage != null){
            toImage.SetParent(cavnasRoot);
            toImage.SetAsFirstSibling();
            ResetRectPos(toImage);
            toImage.gameObject.SetActive(false);
        }
    }
}
