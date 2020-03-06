using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BoulderNotes{
    public class BarGraph : MonoBehaviour
    {
        [SerializeField] private Color[] colors;
        [SerializeField] private BGBar barPrefab;
        [SerializeField] private string[] labels;
   
        [SerializeField] private Slider slider;
        [SerializeField] private Transform contentRoot;

        private BGBar bar;
        private BGBar[] bars;

        private RectTransform rectTransform;

        public void Test(){
            Init(BNGradeMap.Entity.GetGradeNames(), colors);
        }
        public void Init(string[] labels, Color[] colors){
            this.labels = labels;
            rectTransform = GetComponent<RectTransform>();
        
            bars = new BGBar[labels.Length];
            for(int i = 0 ; i < bars.Length ; i++){
                bars[i] = Instantiate<BGBar>(barPrefab, contentRoot);

                bars[i].gameObject.SetActive(true);
                bars[i].Init(labels[i], colors);
                bars[i].SetHeightPerValue(rectTransform.rect.height / 20.0f);
            }
        }

        public void SetData(float[][] arr){
            float maxBarValue = 0f;
            for(int i = 0 ; i < arr.Length ; i++){
                float barValue = 0f;
                for(int j = 0 ; j < arr[i].Length ; j++){
                    barValue += arr[i][j];
                }
                if (maxBarValue < barValue){
                    maxBarValue = barValue;
                }
            }
            for(int i = 0 ; i < bars.Length ; i++){
                bars[i].StoreData(arr[i]);
            }
        }        

        public void Lerp(float t){
            t = Mathf.Lerp(0.0f, 1.0f, t);

            for(int i = 0 ; i < bars.Length ; i++){
                bars[i].Lerp(t);
            }
        }

        public void OnSliderValue(float v){
            Debug.Log("OnsliderValue " + v);
            Lerp(v);
        }

        public void SetRandomValues(){
            float[][] arr = new float[bars.Length][];
            for(int i = 0 ; i < arr.Length ; i++){
                arr[i] = new float[colors.Length];
            }

            for(int i = 0 ; i < arr.Length ; i++){
                for(int j = 0 ; j < arr[0].Length ; j++){
                    arr[i][j] = Random.Range(0.0f, 10.0f);
                }
            }
            SetData(arr);
            slider.value = 0.0f;
        }

    }
}
