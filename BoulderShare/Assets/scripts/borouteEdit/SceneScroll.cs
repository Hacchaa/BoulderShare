using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneScroll : MonoBehaviour {
	[SerializeField]
	private GameObject sceneScrollIconPrefab;
	private RectTransform child;
	private Image curFocus;
	[SerializeField]
	private Color BaseColor ;
	private float iconWidth;
	private float space;
	private int num;
	[SerializeField]
	private Text text;

	void Awake(){
		num = 0;
		child = transform.GetChild(0).gameObject.GetComponent<RectTransform>();
		space = child.gameObject.GetComponent<HorizontalLayoutGroup>().spacing;
		iconWidth = sceneScrollIconPrefab.GetComponent<RectTransform>().rect.width;

		Focus(0);
	}

	public void Delete(){
		num = 0;
		foreach(Transform t in child){
			Destroy(t.gameObject);
		}

		text.text = "0 / 0";
	}

	//index番目のアイコンにフォーカスをあてる
	public void Focus(int index){
		if (num == 0){
			text.text = "0 / 0";
			return ;
		}
		if (index < 0 || index > num - 1){
			return ;
		}
		text.text = (index + 1) + " / " + num;
		/*
		if (curFocus != null){
			curFocus.color = BaseColor;
			curFocus = null;
		}
		if (index < 0 || index > child.childCount - 1){
			return ;
		}

		GameObject icon = child.GetChild(index).gameObject;
		if (icon != null){
			//iconを画面中央に配置する
			float w = (num - 1) * (iconWidth + space) + iconWidth;
			Vector2 v = new Vector2(w / 2 - (index * (iconWidth + space) + iconWidth / 2), 0.0f);
			child.anchoredPosition = v;
			curFocus = icon.GetComponent<Image>();
			curFocus.color = Color.white;
		}*/
	}

	public void Add(int index){
		/*
		GameObject obj = Instantiate(sceneScrollIconPrefab, child);
		obj.transform.SetSiblingIndex(index);*/
		num++;
	}

	public void Remove(int index){
		if (index < 0){
			return ;
		}
		//Destroy(child.GetChild(index).gameObject);
		num--;
	}
}
