using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EditModelFigureView: SEComponentBase
{
	[SerializeField] private ScreenTransitionManager trans;
	[SerializeField] private CameraManager cManager;
	[SerializeField] private Slider height;
	[SerializeField] private Slider reach;
	[SerializeField] private Slider footSlider;
	[SerializeField] private TextMeshProUGUI textHeight;
	[SerializeField] private TextMeshProUGUI textReach;
	[SerializeField] private TextMeshProUGUI textFoot;
	[SerializeField] private float reachRange;
	[SerializeField] private float footRateMin;
	[SerializeField] private float footRateMax;
	[SerializeField] private float countPerCM;
	[SerializeField] private HumanModel humanModel;
	[SerializeField] private float fov = 20.0f;
	[SerializeField] private GameObject ikController;
	private const string PPName_ModelFigure = "Info_ModelFigure";

	public override void OnPreShow(){
		cManager.Active3D();
		cManager.Set3DFOV(fov);
		humanModel.LookAtModel();
		//モデル初期化

		//fbbikのdisable
		//humanModel.SetIKEnable(false);
		ikController.SetActive(false);

		MyUtility.ModelFigure data;
		if(PlayerPrefs.HasKey(PPName_ModelFigure)){
			string json = PlayerPrefs.GetString(PPName_ModelFigure);
			//Debug.Log("model:"+json);
			data = JsonUtility.FromJson<MyUtility.ModelFigure>(json);
		}else{
			data = humanModel.GetDefaultModelFigure();
		}

		LoadModelFigure(data);
	}
	public override void OnPreHide(){
		//humanModel.SetIKEnable(true);
		//humanModel.ReInitIK();
		//humanModel.SetIKEnable(true);
	}

	public void LoadModelFigure(MyUtility.ModelFigure fig){
		height.value = fig.height * countPerCM;
		textHeight.text = ""+ fig.height;

		float min = (fig.height - reachRange) * countPerCM;
		float max = (fig.height + reachRange) * countPerCM;
		reach.minValue = min;
		reach.maxValue = max;
		reach.value = fig.reach * countPerCM;
		textReach.text = "" + fig.reach;

		min = fig.height * footRateMin * countPerCM;
		max = fig.height * footRateMax * countPerCM;
		footSlider.minValue = min;
		footSlider.maxValue = max;
		footSlider.value = fig.leg * countPerCM;
		textFoot.text = "" + fig.leg;

		UpdateModel();
	}

	public MyUtility.ModelFigure GetModelFigure(){
		MyUtility.ModelFigure fig = new MyUtility.ModelFigure();

		fig.height = height.value / countPerCM;
		fig.leg = footSlider.value / countPerCM;
		fig.reach = reach.value / countPerCM;

		return fig;
	}

	public void ToMainView(){
		trans.Transition(ScreenTransitionManager.Screen.MainView);
	}

	public void Submit(){
		//設定をファイルに保存 PlayerPrefs
		MyUtility.ModelFigure fig = GetModelFigure();

		PlayerPrefs.SetString(PPName_ModelFigure, JsonUtility.ToJson(fig));
		PlayerPrefs.Save();

		ToMainView();
	}

	public void OnChangeValueWithHeight(float v){
		float h = v / countPerCM;
		textHeight.text = ""+ h;

		float val = reach.value;
		float min = v - reachRange * countPerCM;
		float max = v + reachRange * countPerCM;

		reach.minValue = min;
		reach.maxValue = max;
		reach.value = Mathf.Clamp(val, min, max);
		textReach.text = "" + (reach.value / countPerCM);

		val = footSlider.value;
		min = v * footRateMin;
		max = v * footRateMax;

		footSlider.minValue = min;
		footSlider.maxValue = max;
		footSlider.value = Mathf.Clamp(val, min, max);
		textFoot.text = "" + (footSlider.value / countPerCM);

		UpdateModel();
	}

	public void OnChangeValueWithReach(float v){
		textReach.text = "" + (v / countPerCM);

		UpdateModel();
	}

	public void OnChangeValueWithFoot(float v){
		textFoot.text = "" + (v / countPerCM);

		UpdateModel();
	}

	private void UpdateModel(){
		float h = height.value / countPerCM;
		float f = footSlider.value / countPerCM;
		float r = reach.value / countPerCM;

		humanModel.UpdateFigure(h, r, f);
	}
}
