using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class SceneEditor : SEComponentBase{
	enum UI {Mark, TwoDPose, ThreeDPose, Comment};
	[SerializeField] private CameraManager cManager;
	[SerializeField] private ScreenTransitionManager sManager;
	[SerializeField] private SceneEditorComponent[] uiComs;
	[SerializeField] private int curIndex;
	[SerializeField] private int toIndex;
	[SerializeField] private bool isHide = false;
	[SerializeField] private bool isShow = false;
	[SerializeField] private bool isTrans = false;
	[SerializeField] private bool isRight = false;
	[SerializeField] private List<RectTransform> syncImages;
	[SerializeField] private List<RectTransform> icons;
	[SerializeField] private float moveDuration = 0.2f;
	[SerializeField] private float fadeDuration = 0.1f;
	[SerializeField] private bool isIconMove = false;
	[SerializeField] private bool initICons = false;
	[SerializeField] private MakeAttemptTree makeAT;
	[SerializeField] private WallManager wallManager;
	[SerializeField] private bool init = false;
	[SerializeField] private HumanModel humanModel;
	[SerializeField] private float iconWidth = 80.0f;
	[SerializeField] private float iconSpace = 20.0f;
	[SerializeField] private RectTransform iconParent;
	[SerializeField] private List<Image> headExitIcons;
	[SerializeField] private List<Image> headSubmitIcons;
	private int maxIndex;

	public void Init(){
		curIndex = 0;
		maxIndex = 0;
		foreach(SceneEditorComponent com in uiComs){
			com.Hide();
			com.SetMakeAT(makeAT);
		}
		iconParent.sizeDelta = new Vector2(iconWidth, iconParent.sizeDelta.y);
		uiComs[curIndex].Show();
		cManager.Active2D();

	}
	//画面遷移時の前処理
	public override void OnPreShow(){
		Init();
	}

	//画面遷移でこの画面を消す時の後処理
	public override void OnPreHide(){
		int n = syncImages.Count;
		for(int i = 0 ; i < n ; i++){
			syncImages[i].localPosition = icons[0].localPosition - icons[i].localPosition;
		}
		wallManager.CommitWallMarks(makeAT.GetWallMarks());
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

	public void Submit(){
		RegistCurComponent();
		makeAT.Make();
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

	void Update(){
		if (isTrans){
			isTrans = false;
			Transition(toIndex);
		}

		if (isHide){
			isHide = false;
			if (isRight){
				uiComs[toIndex].GetHideToRightSeq().Play();
			}else{
				uiComs[toIndex].GetHideToLeftSeq().Play();
			}
		}
		if (isShow){
			isShow = false;
			if (isRight){
				uiComs[toIndex].GetShowFromRightSeq().Play();
			}else{
				uiComs[toIndex].GetShowFromLeftSeq().Play();
			}
		}

		if (isIconMove){
			isIconMove = false;
			IconMove();
		}
		if (initICons){
			initICons = false;
			InitIcon();
		}
	}

	private void InitIcon(){
		int n = icons.Count;
		for(int i = 0 ; i < n ; i++){
			syncImages[i].localPosition = icons[0].localPosition - icons[i].localPosition;
		}
	}

	private void IconMove(){
		Vector3 start, end;

		start = icons[curIndex].localPosition;
		end = icons[toIndex].localPosition;
		//Debug.Log("Start:"+ start);
		//Debug.Log("end:"+end);
		Sequence seq = GetSyncMove(start, end);
		seq.Play();
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

	private Sequence GetIconsWidthSeq(float endWidth){
		Sequence seq = DOTween.Sequence();

		seq.Append(iconParent.DOSizeDelta(new Vector2(endWidth, iconParent.sizeDelta.y), moveDuration).SetEase(Ease.InQuad));

		return seq;
	}

	private Sequence GetUIMove(int index){
		Sequence seq = DOTween.Sequence();

		if (index < curIndex){
			seq.Append(uiComs[curIndex].GetHideToRightSeq())
			.Append(uiComs[index].GetShowFromLeftSeq());
		}else if(index > curIndex){
			seq.Append(uiComs[curIndex].GetHideToLeftSeq())
			.Append(uiComs[index].GetShowFromRightSeq());
		}

		return seq;
	}

	private Sequence GetUIOutSeq(int index){
		Sequence seq = DOTween.Sequence();

		if (index < curIndex){
			seq.Append(uiComs[curIndex].GetHideToRightSeq());
		}else if(index > curIndex){
			seq.Append(uiComs[curIndex].GetHideToLeftSeq());
		}

		return seq;		
	}

	private Sequence GetUIInSeq(int index){
		Sequence seq = DOTween.Sequence();

		if (index < curIndex){
			seq.Append(uiComs[index].GetShowFromLeftSeq());
		}else if(index > curIndex){
			seq.Append(uiComs[index].GetShowFromRightSeq());
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

	private Sequence GetHeadNavigationSeq(bool isArrowShow, bool isExit){
		Sequence seqOut = DOTween.Sequence();
		Sequence seqIn = DOTween.Sequence();

		Image from, to;

		if (isExit){
			if (isArrowShow){
				from = headExitIcons[0];
				to = headExitIcons[1];
			}else{
				from = headExitIcons[1];
				to = headExitIcons[0];
			}
		}else{
			if (isArrowShow){
				from = headSubmitIcons[0];
				to = headSubmitIcons[1];
			}else{
				from = headSubmitIcons[1];
				to = headSubmitIcons[0];
			}
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

		float width = (index*2 + 1) * iconWidth + index * iconSpace * 2;
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

		if (curIndex == 0){
			seq.Join(GetHeadNavigationSeq(false, true));
		}else if(curIndex == uiComs.Length - 1){
			seq.Join(GetHeadNavigationSeq(false, false));
		}

		if (index == 0){
			seq.Join(GetHeadNavigationSeq(true, true));
		}else if(index == uiComs.Length - 1){
			seq.Join(GetHeadNavigationSeq(true, false));
		}

		//fadeout処理
		seq.Join(GetUIOutSeq(index));
		if (isCur2D){
			seq.Join(cManager.GetFadeOut2DSeq(isRight));
		}else{
			seq.Join(cManager.GetFadeOut3DSeq(isRight));
		}

		//interverl処理
		seq.Append(GetIntervalProcSeq(curIndex, index, isCur2D ^ isTo2D));
		
		//fadeIn処理
		Vector3 pos = humanModel.GetModelBodyPosition();
		seq.Join(GetUIInSeq(index));
		if (isCur2D){
			seq.Join(cManager.GetFadeIn2DSeq(!isRight));
		}else{
			seq.Join(cManager.GetFadeIn3DSeq(!isRight, pos));
		}

		seq.Play();
		curIndex = index;
	}
}
