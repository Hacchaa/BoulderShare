using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FontSizeSlider3D : MonoBehaviour
{
   [SerializeField]
   private Slider slider;
   [SerializeField]
   private SceneCommentController3D scc;

   public void ChangeFontSizeFromSlider(float v){
   		scc.ChangeFontSize(v);
   }

   public void SetFontSizeSliderVal(float v){
   		slider.value = v;
   }
}
