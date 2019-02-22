using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorSetter3D : MonoBehaviour
{
   [SerializeField]
   private SceneCommentController3D scc;
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
