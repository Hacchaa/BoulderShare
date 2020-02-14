using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BoulderNotes{
    public class CVTab : MonoBehaviour
    {
        [SerializeField] private CVTabItem[] tabs;
        [SerializeField] private CVTabUnderbar underbar;
        private float tabWidth;

        public void Init(Color normal, Color select, int index){
            for(int i = 0 ; i < tabs.Length ; i++){
                tabs[i].Init(normal, select);
            }
           // Debug.Log("1:"+tabs[1].GetLocalX());
            //Debug.Log("0:"+tabs[0].GetLocalX());
            tabWidth = tabs[1].GetLocalX() - tabs[0].GetLocalX();
            //
            underbar.Init(tabWidth, tabs[0].GetLocalX());
            FocusTab(0);
        }

        public void FocusTab(int index){
            for(int i = 0 ; i < tabs.Length ; i++){
                if (i == index){
                    tabs[i].Focus();
                }else{
                    tabs[i].Defocus();
                }
            }            
        }

        public void SelectTargetTab(int currentIndex, int targetIndex){
            underbar.SetCurrentX(tabs[currentIndex].GetLocalX());

            if (currentIndex == 0 && targetIndex < 0){
                underbar.SetTargetX(-tabWidth);
            }else if(currentIndex == tabs.Length-1 && targetIndex > tabs.Length - 1){
                underbar.SetTargetX(tabs[tabs.Length-1].GetLocalX() + tabWidth);
            }else if(targetIndex < 0 || targetIndex > tabs.Length - 1){
                underbar.SetTargetX(tabs[currentIndex].GetLocalX());
            }else{
                underbar.SetTargetX(tabs[targetIndex].GetLocalX());
            }
        }

        public void StartSelection(int currentIndex, int targetIndex){
            FocusTab(targetIndex);
            SelectTargetTab(currentIndex, targetIndex);
        }
/*
        public void CompleteSelection(bool changedSelection){
            underbar.Complete(changedSelection);
      }*/

        public void Lerp(float t){
            t = Mathf.Lerp(0.0f, 1.0f, t);

            underbar.Lerp(t);
        }
    }
}
