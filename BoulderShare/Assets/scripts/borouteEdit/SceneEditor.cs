using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class SceneEditor : SEComponentBase{
	[SerializeField] private CameraManager cManager;
	[SerializeField] private ScreenTransitionManager sManager;
	[SerializeField] private SceneEditorComponent[] uiComs;
	[SerializeField] private int curIndex;
	[SerializeField] private List<RectTransform> syncImages;
	[SerializeField] private List<RectTransform> icons;
	[SerializeField] private float moveDuration = 0.2f;
	[SerializeField] private float fadeDuration = 0.1f;
	[SerializeField] private MakeAttemptTree makeAT;
	[SerializeField] private WallManager wallManager;
	[SerializeField] private HumanModel humanModel;
	[SerializeField] private float iconWidth = 80.0f;
	[SerializeField] private float iconSpace = 20.0f;
	[SerializeField] private RectTransform iconParent;
	[SerializeField] private List<Image> headExitIcons;
	[SerializeField] private List<Image> headSubmitIcons;
	[SerializeField] private ThreeDView threeDView;
	[SerializeField] private SceneCommentController3D scc;
	[SerializeField] private Image fadeCover;
	[SerializeField] private Transform prevButton;
	[SerializeField] private Transform nextButton;
	[SerializeField] private Image submitButton;
	[SerializeField] private Image cancelButton;

	private int maxIndex;

	public void Init(){
		curIndex = 0;
		maxIndex = 0;
		foreach(SceneEditorComponent com in uiComs){
			com.Hide();
			com.SetMakeAT(makeAT);
		}
		iconParent.sizeDelta = new Vector2(iconWidth, iconParent.sizeDelta.y);
		uiComs[curIndex].OnPreShow();
		uiComs[curIndex].Show();
		cManager.Active2D();

	}
	//画面遷移時の前処理
	public override void OnPreShow(){
		Init();
		threeDView.SetFocusOutAction(ReleaseFocus3D);

		if (makeAT.GetMode() == MakeAttemptTree.Mode.Edit){
			maxIndex = uiComs.Length - 1;
			MoveHeadIconsWidthWithAnim(maxIndex);
		}

		wallManager.ShowTranslucentWall();
	}

	private void ReleaseFocus3D(){
		scc.Release();
	}

	//画面遷移でこの画面を消す時の後処理
	public override void OnPreHide(){
		int n = syncImages.Count;
		for(int i = 0 ; i < n ; i++){
			syncImages[i].localPosition = icons[0].localPosition - icons[i].localPosition;
		}
		threeDView.SetFocusOutAction(null);
	}

	public void RegistCurComponent(){
		if (curIndex < 0 || curIndex > uiComs.Length - 1){
			return ;
		}
		uiComs[curIndex].Regist();
	}

	public void ToATV(){
		RegistCurComponent();
		AttemptTreeMenu.mode = AttemptTreeMenu.Mode.View;
		sManager.Transition(ScreenTransitionManager.Screen.AttemptTreeMenu);
	}

	public void ToATMenu(){
		AttemptTreeMenu.mode = AttemptTreeMenu.Mode.Menu;
		sManager.Transition(ScreenTransitionManager.Screen.AttemptTreeMenu);
	}

	public void SwitchTranslucentWall(){
		if (wallManager.IsShowTranslucentWall()){
			wallManager.HideTranslucentWall();
		}else{
			wallManager.ShowTranslucentWall();
		}
	}

	public void Submit(){
		RegistCurComponent();
		makeAT.Make();
		makeAT.Init();
		ToATMenu();
	}

	public void EditCancel(){
		wallManager.CommitWallMarks(makeAT.GetWallMarks());
		makeAT.Init();
		ToATMenu();
	}

	public void Next(){
		if (curIndex < 0){
			return ;
		}
		if (curIndex == uiComs.Length - 1){
			Submit();
			return ;
		}

		Transition(curIndex + 1);
	}

	public void Prev(){
		if (curIndex > uiComs.Length - 1){
			return ;
		}
		if (curIndex == 0){
			ToATMenu();
			return ;
		}

		Transition(curIndex - 1);
	}

	public void GoTo(int index){
		if (index < 0 || index > uiComs.Length - 1){
			return ;
		}

		Transition(index);
	}

	private void InitIcon(){
		int n = icons.Count;
		for(int i = 0 ; i < n ; i++){
			syncImages[i].localPosition = icons[0].localPosition - icons[i].localPosition;
		}
	}

	private Sequence GetSyncMove(int index){
		return GetSyncMove(icons[curIndex].localPosition, icons[index].localPosition);
	}

	private Sequence GetSyncMove(Vector3 start, Vector3 end){
		Sequence seq = DOTween.Sequence();
		Vector3 v = end - start;
		int n = syncImages.Count;
		int i;
		//Debug.Log("v:"+v);
		seq.OnStart(() =>
			{	
				for(i = 0 ; i < n ; i++){
					syncImages[i].localPosition = start - icons[i].localPosition;
				}
			})
		.Append(syncImages[0].DOLocalMove(v, moveDuration));

		for(i = 1 ; i < n ; i++){
			seq.Join(syncImages[i].DOLocalMove(v, moveDuration));
		}

		seq.SetRelative();
		return seq;
	}

	private Sequence GetFadeCoverSeq(bool isFadeOut){
		Sequence seq = DOTween.Sequence();
		float distAlpha, startAlpha;

		if (isFadeOut){
			distAlpha = 1.0f;
			startAlpha = 0.0f;
		}else{
			distAlpha = 0.0f;
			startAlpha = 1.0f;
		}
		seq.OnStart(()=>
		{
			Color c = fadeCover.color;
			c = new Color(c.r, c.g, c.b, startAlpha);

			fadeCover.color = c;
			fadeCover.gameObject.SetActive(true);
		});
		if (isFadeOut){
			seq.Append(fadeCover.DOFade(distAlpha, fadeDuration).SetEase(Ease.OutQuad));
		}else{
			seq.Append(fadeCover.DOFade(distAlpha, fadeDuration).SetEase(Ease.InQuad));
		}

		seq.OnComplete(()=>
		{
			fadeCover.gameObject.SetActive(false);
		});

		return seq;
	}

	private Sequence GetIconsWidthSeq(float endWidth){
		Sequence seq = DOTween.Sequence();

		seq.Append(iconParent.DOSizeDelta(new Vector2(endWidth, iconParent.sizeDelta.y), moveDuration).SetEase(Ease.InQuad));

		return seq;
	}

	private void MoveHeadIconsWidthWithAnim(int index){
		Sequence seq = DOTween.Sequence();
		float width = GetHeadIconsWidth(index);
		maxIndex = index;
		
		seq.Append(iconParent.DOSizeDelta(new Vector2(width, iconParent.sizeDelta.y), moveDuration).SetEase(Ease.InQuad));

		seq.Play();
	}

	public void MoveHeadIconsWidthWithAnim(){
		if (maxIndex != curIndex){
			MoveHeadIconsWidthWithAnim(curIndex);
		}
	}

	private Sequence GetUIMove(int index){
		Sequence seq = DOTween.Sequence();

		if (index < curIndex){
			seq.Append(uiComs[curIndex].GetHideToRightSeq())
			.Join(GetFadeCoverSeq(true))
			.Append(uiComs[index].GetShowFromLeftSeq())
			.Join(GetFadeCoverSeq(false));
		}else if(index > curIndex){
			seq.Append(uiComs[curIndex].GetHideToLeftSeq())
			.Join(GetFadeCoverSeq(true))
			.Append(uiComs[index].GetShowFromRightSeq())
			.Join(GetFadeCoverSeq(false));
		}

		return seq;
	}

	private Sequence GetUIOutSeq(int index){
		Sequence seq = DOTween.Sequence();

		if (index < curIndex){
			seq.Append(uiComs[curIndex].GetHideToRightSeq())
			.Join(GetFadeCoverSeq(true));
		}else if(index > curIndex){
			seq.Append(uiComs[curIndex].GetHideToLeftSeq())
			.Join(GetFadeCoverSeq(true));
		}

		return seq;		
	}

	private Sequence GetUIInSeq(int index){
		Sequence seq = DOTween.Sequence();

		if (index < curIndex){
			seq.Append(uiComs[index].GetShowFromLeftSeq())
			.Join(GetFadeCoverSeq(false));
		}else if(index > curIndex){
			seq.Append(uiComs[index].GetShowFromRightSeq())
			.Join(GetFadeCoverSeq(false));
		}

		return seq;		
	}

	private Sequence GetIntervalProcSeq(int hideIndex, int showIndex, bool isSwitch){
		Sequence seq = DOTween.Sequence();

		seq.OnStart(() =>
		{	
			uiComs[hideIndex].OnPreHide();
			uiComs[showIndex].OnPreShow();

			if (isSwitch){
				if(cManager.Is2DActive()){
					cManager.Active3D();
				}else{
					cManager.Active2D();
				}
			}
		});

		return seq;
	}

	private Sequence GetHeadNavigationOutSeq(Image targetImage){
		Sequence seq = DOTween.Sequence();

		Image from = targetImage;

		seq.OnStart(() =>
		{
			from.gameObject.SetActive(true);
			from.transform.localScale = Vector3.one;
			Color c = from.color;
			from.color = new Color(c.r, c.g, c.b, 1.0f);
		})
		.Append(from.DOFade(0.0f, fadeDuration)).SetEase(Ease.InQuad)
		.Join(from.transform.DOScale(0.0f, fadeDuration)).SetEase(Ease.InQuad)
		.OnComplete(() =>
		{
			from.transform.localScale = Vector3.one;
			Color c = from.color;
			from.color = new Color(c.r, c.g, c.b, 1.0f);
			from.gameObject.SetActive(false);
		});

		return seq;
	}
	private Sequence GetHeadNavigationInSeq(Image targetImage){
		Sequence seq = DOTween.Sequence();

		Image to;

		to = targetImage;


		seq.OnStart(() =>
		{
			to.gameObject.SetActive(true);
			Color c = to.color;
			to.color = new Color(c.r, c.g, c.b, 0.0f);

			to.transform.localScale = Vector3.zero;
		})
		.Append(to.DOFade(1.0f, fadeDuration)).SetEase(Ease.OutQuad)
		.Join(to.transform.DOScale(1.0f, fadeDuration)).SetEase(Ease.OutQuad)
		.OnComplete(() =>
		{
			to.transform.localScale = Vector3.one;
			Color c = to.color;
			to.color = new Color(c.r, c.g, c.b, 1.0f);
		});
		return seq;
	}

	private Sequence GetFootNavigationOutSeq(int index){
		Sequence seq = DOTween.Sequence();

		if (index != 0){
			Sequence seqPrev = DOTween.Sequence();
			seqPrev.OnStart(() =>
			{
				prevButton.gameObject.SetActive(true);
				prevButton.transform.localScale = Vector3.one;
			})
			.Append(prevButton.transform.DOScale(0.0f, fadeDuration)).SetEase(Ease.OutQuad)
			.OnComplete(() =>
			{
				prevButton.transform.localScale = Vector3.one;
				prevButton.gameObject.SetActive(false);
			});

			seq.Append(seqPrev);
		}

		if (index != uiComs.Length - 1){
			Sequence seqNext = DOTween.Sequence();
			seqNext.OnStart(() =>
			{
				nextButton.gameObject.SetActive(true);
				nextButton.transform.localScale = Vector3.zero;
			})
			.Append(nextButton.transform.DOScale(0.0f, fadeDuration)).SetEase(Ease.OutQuad)
			.OnComplete(() =>
			{
				nextButton.transform.localScale = Vector3.one;
				nextButton.gameObject.SetActive(false);
			});

			if (index != 0){
				seq.Join(seqNext);
			}else{
				seq.Append(seqNext);
			}
		}

		return seq;
	}

	private Sequence GetFootNavigationInSeq(int index){
		Sequence seq = DOTween.Sequence();

		if (index != 0){
			Sequence seqPrev = DOTween.Sequence();
			seqPrev.OnStart(() =>
			{
				prevButton.gameObject.SetActive(true);
				prevButton.transform.localScale = Vector3.zero;
			})
			.Append(prevButton.transform.DOScale(1.0f, fadeDuration)).SetEase(Ease.OutQuad)
			.OnComplete(() =>
			{
				prevButton.transform.localScale = Vector3.one;
			});

			seq.Append(seqPrev);
		}

		if (index != uiComs.Length - 1){
			Sequence seqNext = DOTween.Sequence();
			seqNext.OnStart(() =>
			{
				nextButton.gameObject.SetActive(true);
				nextButton.transform.localScale = Vector3.zero;
			})
			.Append(nextButton.transform.DOScale(1.0f, fadeDuration)).SetEase(Ease.OutQuad)
			.OnComplete(() =>
			{
				nextButton.transform.localScale = Vector3.one;
			});

			if (index != 0){
				seq.Join(seqNext);
			}else{
				seq.Append(seqNext);
			}
		}

		return seq;
	}

	private float GetHeadIconsWidth(int index){
		return (index*2 + 1) * iconWidth + index * iconSpace * 2;
	}

	private void Transition(int index){
		Sequence seq = DOTween.Sequence();

		if (index < 0 || index > uiComs.Length - 1){
			return ;
		}
		//index == curIndexは何もしない
		if (curIndex == -1 || curIndex == index){
			return ;
		}

		uiComs[curIndex].Regist();

		float width = GetHeadIconsWidth(index);
		bool isCur2D = uiComs[curIndex].Is2D();
		bool isTo2D = uiComs[index].Is2D();
		bool isRight ;

		if (index > curIndex){
			isRight = false;
		}else{
			isRight = true;
		}

		//fadeOutIn処理
		seq.Append(GetSyncMove(index));
		if (index > maxIndex){
			seq.Join(GetIconsWidthSeq(width));
			maxIndex = index;
		}

		//fadeout処理
		seq.Join(GetUIOutSeq(index));
		if (isCur2D){
			seq.Join(cManager.GetFadeOut2DSeq(isRight));
		}else{
			seq.Join(cManager.GetFadeOut3DSeq(isRight));
		}
		if (curIndex == uiComs.Length - 1){
			seq.Join(GetHeadNavigationOutSeq(submitButton));
		}
		seq.Join(GetFootNavigationOutSeq(curIndex));

		//interverl処理
		seq.Append(GetIntervalProcSeq(curIndex, index, isCur2D ^ isTo2D));
		
		//fadeIn処理
		seq.Join(GetUIInSeq(index));
		if (isTo2D){
			seq.Join(cManager.GetFadeIn2DSeq(!isRight));
		}else{
			Vector3 pos ;
			//posの取得
			if (isCur2D){
				if(makeAT.IsPoseSet()){
					humanModel.SetModelPose(makeAT.GetPositions(), makeAT.GetRotations(), makeAT.GetRightHandAnim(), makeAT.GetLeftHandAnim());
				}else{
					humanModel.CorrectModelPose();
				}
			}
			pos = humanModel.GetModelBodyPosition();
			seq.Join(cManager.GetFadeIn3DSeq(!isRight, pos));
		}
		if (index == uiComs.Length - 1){
			seq.Join(GetHeadNavigationInSeq(submitButton));
		}
		seq.Join(GetFootNavigationInSeq(index));

		seq.Play();
		curIndex = index;
	}
}
