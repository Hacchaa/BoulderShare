using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FontSizeSlider : MonoBehaviour
{
   [SerializeField]
   private Slider slider;
   [SerializeField]
   private SceneCommentController scc;

   public void ChangeFontSizeFromSlider(float v){
   		scc.ChangeFontSize(v);
   }

   public void SetFontSizeSliderVal(float v){
   		slider.value = v;
   }
}
