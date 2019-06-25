using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class STAnimation_ATMenu_SceneEditor : STAnimationFromBotToTop
{
    [SerializeField] private RectTransform cavnasRoot;
    [SerializeField] private RectTransform ss2DTo;
    [SerializeField] private RectTransform ss2DFrom;
    [SerializeField] private RectTransform ss3DFrom;
    [SerializeField] private CameraManager cManager;

    private RectTransform fromImage;
    private RectTransform toImage;

    public override void Play(){
        OnPostCompleteAction = CompleteAction;
    	StartCoroutine(TakeScreenShot());
    }

    private IEnumerator TakeScreenShot(){
        if(cManager.Is2DActive()){
            cManager.StartSS2DFrom();
        }else{
            cManager.StartSS3DFrom();
        }
    	yield return new WaitForEndOfFrame();
        //from用rendertexture終了
        if(cManager.Is2DActive()){
            fromImage = ss2DFrom;
            cManager.EndSS2DFrom();
        }else{
            fromImage = ss3DFrom;
            cManager.EndSS3DFrom();
        }

        fromImage.SetParent(fromRect);
        fromImage.SetAsFirstSibling();
        ResetRectPos(fromImage);
        fromImage.gameObject.SetActive(true);
        PrioritizeFrom();
        to.Show();

        cManager.StartSS2DTo();

        yield return new WaitForEndOfFrame();

        toImage = ss2DTo;
        cManager.EndSS2DTo();

        toImage.SetParent(toRect);
        toImage.SetAsFirstSibling();
        ResetRectPos(toImage);
        toImage.gameObject.SetActive(true);

    	Animate();
    }

    private void CompleteAction(){
        from.Hide();
        to.Show();

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
