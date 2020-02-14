using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoulderNotes{
    public class CVContents : MonoBehaviour
    {
        [SerializeField] private CVContent[] contents;
        private CVContent current;
        private CVContent target;
        
        public void Init(float w){
            Clear();
            for(int i = 0 ; i < contents.Length ; i++){
                contents[i].Init(w);
                contents[i].Deactive();
            }  

            contents[0].Active();
        }

        private void Clear(){
            if (current != null){
                current.Deactive();
                current = null;
            }

            if (target != null){
                target.Deactive();
                target = null;
            }
        }

        public int GetSize(){
            return contents.Length;
        }

        //targetIndexが存在しないcontentをさす場合、現在のcontentの位置をオーバーシュートさせる
        public void StartSelection(int currentIndex, int targetIndex){
            if (currentIndex < 0 || currentIndex > contents.Length - 1 || currentIndex == targetIndex){
                return ;
            }

            Clear();

            current = contents[currentIndex];
            current.Active();
            if (targetIndex >= 0 && targetIndex <= contents.Length - 1){
                target = contents[targetIndex];
                target.Active();
            }

            bool moveToRight = false;
            if (currentIndex > targetIndex){
                moveToRight = true;
            }

            current.SetMoveVec(false, moveToRight);
            if (target != null){
                target.SetMoveVec(true, moveToRight);
            }
        }

        public void CompleteSelection(bool changedSelection){
            if (changedSelection){
                current.Deactive();             
            }else{
                if (target != null){
                    target.Deactive();
                }
            }
        }

        public void Lerp(float t){
            t = Mathf.Lerp(0.0f, 1.0f, t);

            current.Lerp(t);
            if (target != null){
                target.Lerp(t);
            }
        }
    }              
}
