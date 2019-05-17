using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class ColorSetter3D : MonoBehaviour
{
   [SerializeField] private SceneComment3D sceneComment;
   [SerializeField] private Slider alphaSlider;
   [SerializeField] private RectTransform rectT;
   [SerializeField] private float fadeDuration = 0.1f;
   [SerializeField] private float duration = 0.2f;
   [SerializeField] private Vector2 hidePosition = new Vector2(15.0f, -30.0f);
   [SerializeField] private float defaultHeight = 30.0f;
   [SerializeField] private Image openImage;
   [SerializeField] private Image closeImage;
   [SerializeField] private ScrollRect scrollRect;

	public void ChangeAlphaFromSlider(float v){
		sceneComment.SetAlpha(v);
	}

	public void SetAlphaSliderVal(float v){
      if (alphaSlider != null){
         alphaSlider.value = v;
      }
	}

	public void SetColor(Color c){
		sceneComment.SetColor(c);
	}

   public void OpenCloseColor(){
      if (openImage.gameObject.activeSelf){
         Open();
      }else{
         Close();
      }
   }

   public void InitObjs(){
      openImage.gameObject.SetActive(true);
      closeImage.gameObject.SetActive(false);
      rectT.sizeDelta = new Vector2(0.0f, defaultHeight);
   }

   public void Open(){
      Sequence seq = DOTween.Sequence();
      float width = sceneComment.GetWidth() / 3 * 2;
      Vector2 destiny = new Vector2(width, rectT.sizeDelta.y);
      float dist = destiny.x / 2.0f + hidePosition.x;

      seq.OnStart(() =>
      {
         rectT.sizeDelta = new Vector2(0.0f, rectT.sizeDelta.y);
         rectT.localPosition = hidePosition;
         scrollRect.horizontalNormalizedPosition = 0.0f;
      })
      .Append(rectT.DOSizeDelta(destiny, duration))
      .Join(rectT.DOLocalMoveX(dist, duration))
      .Join(GetOpenImageSeq(true))
      .Play();
   }

   public void Close(){
      Sequence seq = DOTween.Sequence();

      seq.Append(rectT.DOSizeDelta(new Vector2(0.0f, rectT.sizeDelta.y), duration))
      .Join(rectT.DOLocalMoveX(hidePosition.x, duration))
      .Join(GetOpenImageSeq(false))
      .Play();     
   }

   private Sequence GetOpenImageSeq(bool isOpen){
      Sequence seqOut = DOTween.Sequence();
      Sequence seqIn = DOTween.Sequence();

      Image from, to;

      if (isOpen){
         from = openImage;
         to = closeImage;
      }else{
         from = closeImage;
         to = openImage;
      }

      seqOut.OnStart(() =>
      {
         from.gameObject.SetActive(true);
         to.gameObject.SetActive(false);
      })
      .Append(from.DOFade(0.0f, fadeDuration)).SetEase(Ease.InQuad)
      .Join(from.transform.DOScale(0.0f, fadeDuration)).SetEase(Ease.InQuad)
      .OnComplete(() =>
      {
         from.transform.localScale = Vector3.one;
         Color c = from.color;
         from.color = new Color(c.r, c.g, c.b, 1.0f);
      });

      seqIn.OnStart(() =>
      {
         from.gameObject.SetActive(false);
         to.gameObject.SetActive(true);
         Color c = to.color;
         to.color = new Color(c.r, c.g, c.b, 0.0f);

         to.transform.localScale = Vector3.zero;
      })
      .Append(to.DOFade(1.0f, fadeDuration)).SetEase(Ease.OutQuad)
      .Join(to.transform.DOScale(1.0f, fadeDuration)).SetEase(Ease.OutQuad);

      Sequence seq = DOTween.Sequence();

      seq.Append(seqOut)
      .Append(seqIn);

      return seq;
   }
}
