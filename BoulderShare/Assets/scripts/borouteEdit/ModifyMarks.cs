using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ModifyMarks : SEComponentBase
{
	[SerializeField] private TwoDWallMarks twoDWallMarks;
	[SerializeField] private RectTransform garbageArea;
	[SerializeField] private List<Image> garbageFadeList;
	[SerializeField] private GarbageFor2DMark garbageFor2DMark;
	[SerializeField] private float duration = 0.1f;
	[SerializeField] private CameraManager cManager;
	[SerializeField] private ScreenTransitionManager trans;
	[SerializeField] private WallManager wallManager;
	[SerializeField] private TwoDWall twoDWall;
	[SerializeField] private List<Image> fadeList;
	[SerializeField] private EditorPopup popup;
	[SerializeField] private bool isNeedChecking = false;
	[SerializeField] private Dictionary<string, int> modifiedMap;
	[SerializeField] private AttemptTreeMenu atMenu;
	private Sequence openSeq;
	private Sequence closeSeq;
	public enum ModType {MOVE = 0, SCALE, REMOVE};
	//画面遷移時の前処理
	public override void OnPreShow(){
		cManager.Active2D();
		twoDWallMarks.AcceptEvents();
		TwoDMark.AddOnPointerDownAction(OpenGarbage);
		TwoDMark.AddOnPointerUpAction(CloseGarbage);
		TwoDMark.AddOnPointerDownAction(UpdateMovementMarkList);

		TwoDMarkScale.AddOnBeginDragAction(UpdateScaleMarkList);
		garbageFor2DMark.AddOnRemoveAction(UpdateRemoveMarkList);

		modifiedMap = new Dictionary<string, int>();

		twoDWallMarks.ClearTouch();
	}

	//画面遷移でこの画面を消す時の後処理
	public override void OnPreHide(){
		twoDWallMarks.IgnoreEvents();
		twoDWallMarks.ReleaseFocus();

		TwoDMark.ResetOnPointerUpAction();
		TwoDMark.ResetOnPointerDownAction();
		TwoDMarkScale.ResetOnBeginDragAction();

		garbageFor2DMark.ResetOnRemoveAction();


		modifiedMap = null;		
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

			garbageArea.gameObject.SetActive(true);
			garbageArea.localPosition = new Vector3(0.0f, -moveY, 0.0f);
			foreach(Image img in garbageFadeList){
				Color c = img.color;
				c = new Color(c.r, c.g, c.b, 0.0f);
				img.color = c;
			}	
		})
		.Append(garbageArea.DOLocalMoveY(moveY, duration).SetEase(Ease.OutQuad).SetRelative());

		foreach(Image img in garbageFadeList){
			seq.Join(img.DOFade(1.0f, duration).SetEase(Ease.OutQuad));
		}
		seq.OnComplete(()=>
		{	
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

			garbageArea.gameObject.SetActive(true);
			garbageArea.localPosition = Vector3.zero;
			foreach(Image img in garbageFadeList){
				Color c = img.color;
				c = new Color(c.r, c.g, c.b, 1.0f);
				img.color = c;
			}				
		})
		.Append(garbageArea.DOLocalMoveY(-moveY, duration).SetEase(Ease.OutQuad).SetRelative());
		foreach(Image img in garbageFadeList){
			seq.Join(img.DOFade(0.0f, duration).SetEase(Ease.OutQuad));
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

	private void ToATMenu(){
		trans.Transition(ScreenTransitionManager.Screen.AttemptTreeMenu);
	}

	public void OpenDialog(){
		string title = "マークの編集を決定しますか？";
		string support = "マークの位置や大きさの変更、削除は複数シーンに影響を及ぼします。";
		popup.Open(Submit, null, title, support, "決定", "キャンセル");
	}

	public void Submit(){
		wallManager.CommitWallMarks(twoDWall.GetWallMarks());
		atMenu.UpdateWarning(modifiedMap);
		ToATMenu();
	}



	public void Cancel(){
		wallManager.SyncWallMarks();
		ToATMenu();
	}

	private void UpdateMovementMarkList(){
		TwoDMark mark = twoDWallMarks.GetFocus();
		int type = 1;
		if (mark != null){
			string name = mark.gameObject.name;
			if (!modifiedMap.ContainsKey(name)){
				modifiedMap.Add(name, type);
			}else{
				int cur = modifiedMap[name];
				modifiedMap[name] = cur | type;
			}
		}
	}

	private void UpdateScaleMarkList(){
		TwoDMark mark = twoDWallMarks.GetFocus();
		int type = 2;
		if (mark != null){
			string name = mark.gameObject.name;
			if (!modifiedMap.ContainsKey(name)){
				modifiedMap.Add(name, type);
			}else{
				int cur = modifiedMap[name];
				modifiedMap[name] = cur | type;
			}
		}
	}

	private void UpdateRemoveMarkList(){
		TwoDMark mark = twoDWallMarks.GetFocus();
		int type = 4;
		if (mark != null){
			string name = mark.gameObject.name;
			if (!modifiedMap.ContainsKey(name)){
				modifiedMap.Add(name, type);
			}else{
				int cur = modifiedMap[name];
				modifiedMap[name] = cur | type;
			}
		}
	}

	public string GetWarningTailText(int type){
		int max = 1 << Enum.GetNames(typeof(ModType)).Length;
		if (type <= 0 || type > max - 1){
			return "";
		}else if(type == 1){
			return "が動かされました。";
		}else if (type == 2){
			return "の大きさが変更されました。";
		}else if (type == 3){
			return "が動かされ、大きさが変更されました。";
		}
		return "が削除されました。";
	}

	public bool IsMarkModified(int n, ModType type){
		int typeNum = 1 << (int)type;
		return (n & typeNum) == typeNum;
	}
}
