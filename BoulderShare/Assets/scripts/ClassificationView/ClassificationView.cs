using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace BoulderNotes{
    public class ClassificationView : MonoBehaviour
    {
        [SerializeField] private CVTab tab;
        [SerializeField] private Color normal;
        [SerializeField] private Color select;
        [SerializeField] private CVContents contents;
        [SerializeField] private float duration = 0.5f;
        private int current;
        private int target;
        private float width;
        private int contentNum;

        public void Init(){
            current = 0;
            tab.Init(normal, select, current);
            width = GetComponent<RectTransform>().rect.width;
            contentNum = contents.GetSize();
            contents.Init(width);            
        }
        public bool NeededOverShooting(){
            if (current == 0 && target == -1){
                return true;
            }

            if (current == contentNum-1 && target == contentNum){
                return true;
            }
            return false;
        }
        public float GetWidth(){
            return width;
        }

        public void MoveTo(int index){
            //Debug.Log("moveTO");
            if (current == index){
                return ;
            }
            target = index;

            DOVirtual.Float(0.0f, 1.0f, duration, value => {
                tab.Lerp(value);
                contents.Lerp(value);
            })
            .SetEase(Ease.OutQuart)
            .OnStart(() =>{
                BNScreens.Instance.Interactive(false);
                tab.StartSelection(current, target);   
                contents.StartSelection(current, target);
            })
            .OnComplete(() =>{
                contents.CompleteSelection(true);
                current = target;
                BNScreens.Instance.Interactive(true);
            });
        }

        public void SelectTarget(bool moveToRight){
            if (moveToRight){
                target = current - 1;
            }else{
                target = current + 1;
            }
     
            tab.SelectTargetTab(current, target);   
            contents.StartSelection(current, target);  
        }

        public void SelectTabFocus(bool focusCurrent){
            if (focusCurrent){
                tab.FocusTab(current);
            }else{
                tab.FocusTab(target);
            }
        }

        public void MoveStartByHand(bool moveToRight){
            SelectTarget(moveToRight);
            SelectTabFocus(true);
            BNScreens.Instance.Interactive(false);          
        }

        public void MoveByHand(float t){
            tab.Lerp(t);
            contents.Lerp(t);
        }

        public void MoveEndByHand(float t, bool changedSelection){
            t = Mathf.Lerp(0.0f, 1.0f, t);
            float dist = 1.0f;
            if (!changedSelection){
                dist = 0.0f;
            }
            SelectTabFocus(!changedSelection);

            DOVirtual.Float(t, dist, duration, value => {
                tab.Lerp(value);
                contents.Lerp(value);
            })
            .SetEase(Ease.OutQuart)
            .OnComplete(() =>{
                contents.CompleteSelection(changedSelection);
                if (changedSelection){
                    current = target;
                }
                BNScreens.Instance.Interactive(true);
            });            
        }
    }
}
