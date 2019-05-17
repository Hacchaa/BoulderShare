using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ModifyMarks : SEComponentBase
{
	[SerializeField] private TwoDWallMarks twoDWallMarks;
	[SerializeField] private RectTransform garbageArea;
	[SerializeField] private float duration = 0.1f;
	[SerializeField] private CameraManager cManager;
	[SerializeField] private ScreenTransitionManager trans;
	[SerializeField] private WallManager wallManager;
	[SerializeField] private TwoDWall twoDWall;
	[SerializeField] private List<Image> fadeList;
	[SerializeField] private EditorPopup popup;
	
	//画面遷移時の前処理
	public override void OnPreShow(){
		cManager.Active2D();
		twoDWallMarks.AcceptEvents();
		twoDWallMarks.SetFocusOnAction(OpenGarbage);
		twoDWallMarks.SetFocusOffAction(CloseGarbage);
	}

	//画面遷移でこの画面を消す時の後処理
	public override void OnPreHide(){
		twoDWallMarks.IgnoreEvents();
		twoDWallMarks.ReleaseFocus();
		twoDWallMarks.SetFocusOnAction(null);
		twoDWallMarks.SetFocusOffAction(null);
	}

	public void OpenGarbage(){
		Sequence seq = DOTween.Sequence();
		float moveY = garbageArea.rect.height;
		moveY = 0.0f;
		seq.OnStart(()=>
		{
			garbageArea.gameObject.SetActive(true);
			garbageArea.localPosition = new Vector3(0.0f, -moveY, 0.0f);
			foreach(Image img in fadeList){
				Color c = img.color;
				c = new Color(c.r, c.g, c.b, 0.0f);
				img.color = c;
			}			
		})
		.Append(garbageArea.DOLocalMoveY(moveY, duration).SetEase(Ease.OutQuad).SetRelative());

		foreach(Image img in fadeList){
			seq.Join(img.DOFade(1.0f, duration).SetEase(Ease.OutQuad));
		}

		seq.Play();
	}
	
	public void CloseGarbage(){
		Sequence seq = DOTween.Sequence();
		float moveY = garbageArea.rect.height;
		moveY = 0.0f;

		seq.OnStart(()=>
		{
			garbageArea.gameObject.SetActive(true);
			garbageArea.localPosition = Vector3.zero;
			foreach(Image img in fadeList){
				Color c = img.color;
				c = new Color(c.r, c.g, c.b, 1.0f);
				img.color = c;
			}	
		})
		.Append(garbageArea.DOLocalMoveY(-moveY, duration).SetEase(Ease.OutQuad).SetRelative());
		foreach(Image img in fadeList){
			seq.Join(img.DOFade(0.0f, duration).SetEase(Ease.OutQuad));
		}
		seq.OnComplete(()=>
		{
			garbageArea.gameObject.SetActive(false);
			foreach(Image img in fadeList){
				Color c = img.color;
				c = new Color(c.r, c.g, c.b, 1.0f);
				img.color = c;
			}	
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
		ToATMenu();
	}

	public void Cancel(){
		wallManager.SyncWallMarks();
		ToATMenu();
	}
}
