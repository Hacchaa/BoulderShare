using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorSetter : MonoBehaviour
{
   [SerializeField]
   private SceneCommentController scc;
   [SerializeField]
   private Slider alphaSlider;

   	public void ChangeAlphaFromSlider(float v){
   		scc.ChangeAlpha(v);
   	}

   	public void SetAlphaSliderVal(float v){
   		alphaSlider.value = v;
   	}

   	public void SetColor(Color c){
   		scc.ChangeColor(c);
   	}
}
