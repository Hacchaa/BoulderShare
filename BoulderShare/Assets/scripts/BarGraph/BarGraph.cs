using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BoulderNotes{
    public class BarGraph : MonoBehaviour
    {
        [SerializeField] private Color[] colors;
        [SerializeField] private GameObject barPrefab;
        [SerializeField] private BarLabelsBase labels;
        [SerializeField] private int barNum;
        [SerializeField] private int startIndex;
        [SerializeField] private float barWidth ;
        [SerializeField] private float space ;
        [SerializeField] private Slider slider;

        private BGBar bar;
        private BGBar[] bars;

        private RectTransform rectTransform;

        public void Test(){
            Init(5, 3);
        }
        public void Init(int barNum, int startIndex){
            int i;
            this.barNum = barNum;
            this.startIndex = startIndex;

            bars = new BGBar[barNum];

            rectTransform = GetComponent<RectTransform>();
            float baseWidth = space;
            for(i = 0 ; i < barNum ; i++){
                GameObject obj = Instantiate(barPrefab, this.transform);
                obj.name = labels.GetLabelName(i + startIndex);
                obj.SetActive(true);
                
                RectTransform rec = obj.GetComponent<RectTransform>();
                rec.anchorMin = Vector2.zero;
                rec.anchorMax = Vector2.zero;
                rec.sizeDelta = new Vector2(barWidth, rectTransform.rect.height);
                rec.anchoredPosition = new Vector2(baseWidth + (barWidth / 2.0f), rec.sizeDelta.y / 2.0f);
                baseWidth += barWidth + space;

                bars[i] = obj.GetComponent<BGBar>();
                bars[i].Init(colors);
                bars[i].SetHeightPerValue(rectTransform.rect.height / 20.0f);

            }
        }
        public void SetBarNum(int n){
            barNum = n;
        }
        public void SetStartLabelIndex(int ind){
            startIndex = ind;
        }
        public void SetData(float[][] arr){
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
                    arr[i][j] = Random.Range(0.0f, 5.0f);
                }
            }
            SetData(arr);
            slider.value = 0.0f;
        }

    }
}
