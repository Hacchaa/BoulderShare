using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BoulderNotes{
    public class BGBar : MonoBehaviour
    {
        [SerializeField] private GameObject categoryPrefab;
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private Transform categoryRoot;
        private int catNum;
        [SerializeField] private float[] prevValues;
        [SerializeField] private float[] curValues;
        private RectTransform[] cats;
        private float heightPerValue;
        private RectTransform rectTransform;

        public void Init(string lab, Color[] colors){
            label.text = lab;
            catNum = colors.Length;
            prevValues = new float[catNum];
            curValues = new float[catNum];
            cats = new RectTransform[catNum];
            rectTransform = GetComponent<RectTransform>();
            heightPerValue = 0f;
            
            for(int i = 0 ; i < catNum ; i++){
                GameObject obj = Instantiate(categoryPrefab, categoryRoot);
                obj.name = "caterogy " + i;
                obj.transform.SetAsFirstSibling();
                cats[i] = obj.GetComponent<RectTransform>();
                cats[i].anchorMin = Vector2.zero;
                cats[i].anchorMax = Vector2.right;
                cats[i].pivot = new Vector2(0.5f, 0f);
                cats[i].sizeDelta = new Vector2(0.0f, 0.0f);
                cats[i].anchoredPosition = Vector2.zero;
                obj.GetComponent<Image>().color = new Color(colors[i].r, colors[i].g, colors[i].b, colors[i].a);
                obj.SetActive(true);
            }
        }

        public void StoreData(float[] v){
            curValues.CopyTo(prevValues, 0);
            v.CopyTo(curValues, 0);
        }

        public void SetHeightPerValue(float hpv){
            heightPerValue = hpv;
        }
        
        public void Lerp(float t){
            t = Mathf.Lerp(0.0f, 1.0f, t);
            float baseH = 0.0f;

            for(int i = 0 ; i < catNum ; i++){
                float prev = prevValues[i];
                float cur = curValues[i];
                float dif = cur - prev;
                float h = (prev + dif * t) * heightPerValue;
                cats[i].sizeDelta = new Vector2(0.0f, h + baseH);
                baseH += h;
            }
        }
    }
}