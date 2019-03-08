using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModelSizeSetter : MonoBehaviour
{
	[SerializeField]
	private HumanModel humanModel;
	[SerializeField]
	private Slider modelSizeSlider;

    // Start is called before the first frame update
	public void SyncModelSize(){
		modelSizeSlider.value = humanModel.GetModelSize();
	}

	public void SetModelSizeFromSlider(){
		humanModel.SetModelSize(modelSizeSlider.value);
	}
}
