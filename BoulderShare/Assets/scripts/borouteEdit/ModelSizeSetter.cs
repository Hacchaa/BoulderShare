using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModelSizeSetter : MonoBehaviour
{
	[SerializeField]
	private ThreeD threeD;
	[SerializeField]
	private Slider modelSizeSlider;

    // Start is called before the first frame update
	public void SyncModelSize(){
		modelSizeSlider.value = threeD.GetModelSize();
	}

	public void SetModelSizeFromSlider(){
		threeD.SetModelSize(modelSizeSlider.value);
	}
}
