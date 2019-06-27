using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class SceneEditorMark : SceneEditorComponent
{
	[SerializeField] private TwoDWallMarks twoDWallMarks;
	[SerializeField] private Slider scaleSlider;
	[SerializeField] private WallManager wallManager;
	[SerializeField] private TwoDWall twoDWall;
	[SerializeField] private RectTransform garbageArea;
	[SerializeField] private GameObject makeMarkButton;
	[SerializeField] private List<Image> garbageFadeList;
	[SerializeField] private List<Image> makeMarkFadeList;
	[SerializeField] private float duration = 0.2f;
	[SerializeField] private Generate2DMark generator;

	private Sequence openSeq;
	private Sequence closeSeq;

	[SerializeField] private bool isExistOpen;
	[SerializeField] private bool isExistClose;

	void Update(){
		if (openSeq != null){
			isExistOpen = true;
		}else{
			isExistOpen = false;
		}

		if (closeSeq != null){
			isExistClose = true;
		}else{
			isExistClose = false;
		}
	}

	//画面遷移時の前処理
	public override void OnPreShow(){
		if (!makeAT.IsWallMarkSet()){
			makeAT.SetWallMarks(wallManager.GetMasterWallMarks());
		}

		twoDWallMarks.IgnoreEvents();
		twoDWallMarks.MakeMarksNoActive();
		twoDWallMarks.ClearTouch();

		if (makeAT.IsWallMarkSet()){
			List<string> list = makeAT.GetDiffMarks();
			twoDWallMarks.MakeMarksActive(list);
			twoDWallMarks.AcceptEvents(list);
		}
		garbageArea.gameObject.SetActive(false);
		makeMarkButton.SetActive(true);

		TwoDMark.AddOnBeginDragAction(OpenGarbage);
		TwoDMark.AddOnBeginDragAction(sceneEditor.MoveHeadIconsWidthWithAnim);
		TwoDMark.AddOnPointerUpAction(CloseGarbage);

		generator.AddOnGenerateMarkAction(sceneEditor.MoveHeadIconsWidthWithAnim);
	}

	//画面遷移でこの画面を消す時の後処理
	public override void OnPreHide(){
		twoDWallMarks.IgnoreEvents();
		twoDWallMarks.ReleaseFocus();
		twoDWallMarks.MakeMarksActive();

		makeAT.StoreDiffMarks(twoDWall.GetWallMarks());

		garbageArea.gameObject.SetActive(false);
		makeMarkButton.SetActive(true);

		TwoDMark.ResetOnPointerUpAction();
		TwoDMark.ResetOnBeginDragAction();

		generator.ResetOnGenerateMarkAction();
	}

	public override void Regist(){
		wallManager.CommitWallMarks(twoDWall.GetWallMarks());
	}

	//2dmarkを選択する
	//EditWallMark/Selecter
	public void NextMark(){
		twoDWallMarks.Next();
	}

	public void PrevMark(){
		twoDWallMarks.Prev();
	}

	//2dマークの大きさを決めるスライダー
	//EditWallMark/MarkOptions/ScaleSlider
	public void OnScaleSlider(float val){
		TwoDMark mark = twoDWallMarks.GetFocus();
		mark.transform.localScale = Vector3.one * val;
	}

	public void SetScaleSliderVal(){
		TwoDMark mark = twoDWallMarks.GetFocus();
		scaleSlider.value = mark.transform.localScale.x;
	}

	public void OpenGarbage(){
		if (openSeq != null){
			return ;
		}

		if (closeSeq == null && garbageArea.gameObject.activeSelf){
			return ;
		}
		Sequence seq = DOTween.Sequence();
		float moveY = garbageArea.rect.height;
		moveY = 0.0f;
		seq.OnStart(()=>
		{
			if(closeSeq != null){
				closeSeq.Kill();
				closeSeq = null;
			}
			openSeq = seq;

			makeMarkButton.SetActive(true);
			garbageArea.gameObject.SetActive(true);
			garbageArea.localPosition = new Vector3(0.0f, -moveY, 0.0f);
			foreach(Image img in garbageFadeList){
				Color c = img.color;
				c = new Color(c.r, c.g, c.b, 0.0f);
				img.color = c;
			}
			foreach(Image img in makeMarkFadeList){
				Color c = img.color;
				c = new Color(c.r, c.g, c.b, 1.0f);
				img.color = c;
			}			
		})
		.Append(garbageArea.DOLocalMoveY(moveY, duration).SetEase(Ease.OutQuad).SetRelative());

		foreach(Image img in garbageFadeList){
			seq.Join(img.DOFade(1.0f, duration).SetEase(Ease.OutQuad));
		}
		foreach(Image img in makeMarkFadeList){
			seq.Join(img.DOFade(0.0f, duration).SetEase(Ease.OutQuad));
		}

		seq.OnComplete(()=>
		{
			foreach(Image img in makeMarkFadeList){
				Color c = img.color;
				c = new Color(c.r, c.g, c.b, 1.0f);
				img.color = c;
			}
			makeMarkButton.SetActive(false);	
			openSeq = null;		
		});

		seq.Play();
	}
	
	public void CloseGarbage(){
		if (closeSeq != null){
			return ;
		}
		if (openSeq == null && !garbageArea.gameObject.activeSelf){
			return ;
		}
		Sequence seq = DOTween.Sequence();
		float moveY = garbageArea.rect.height;
		moveY = 0.0f;

		seq.OnStart(()=>
		{
			if (openSeq != null){
				openSeq.Kill();
				openSeq = null;
			}
			closeSeq = seq;

			makeMarkButton.SetActive(true);
			garbageArea.gameObject.SetActive(true);
			garbageArea.localPosition = Vector3.zero;
			foreach(Image img in garbageFadeList){
				Color c = img.color;
				c = new Color(c.r, c.g, c.b, 1.0f);
				img.color = c;
			}
			foreach(Image img in makeMarkFadeList){
				Color c = img.color;
				c = new Color(c.r, c.g, c.b, 0.0f);
				img.color = c;
			}				
		})
		.Append(garbageArea.DOLocalMoveY(-moveY, duration).SetEase(Ease.OutQuad).SetRelative());
		foreach(Image img in garbageFadeList){
			seq.Join(img.DOFade(0.0f, duration).SetEase(Ease.OutQuad));
		}
		foreach(Image img in makeMarkFadeList){
			seq.Join(img.DOFade(1.0f, duration).SetEase(Ease.OutQuad));
		}
		seq.OnComplete(()=>
		{
			garbageArea.gameObject.SetActive(false);
			foreach(Image img in garbageFadeList){
				Color c = img.color;
				c = new Color(c.r, c.g, c.b, 1.0f);
				img.color = c;
			}
			closeSeq = null;	
		});

		seq.Play();
	}
}
