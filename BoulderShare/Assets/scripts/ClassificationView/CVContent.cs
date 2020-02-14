using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CVContent : MonoBehaviour
{
        private RectTransform rectTransform;
        private float width;
        private float startX;
        private float endX;
        public virtual void Init(float w){
            width = w;
            startX = 0.0f;
            endX = 0.0f;
            rectTransform = GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0.0f, rectTransform.anchoredPosition.y);
        }

        public void Active(){
            gameObject.SetActive(true);
        }

        public void Deactive(){
            gameObject.SetActive(false);
        }

        public void SetMoveVec(bool isTarget, bool moveToRight){
            if (isTarget){
                startX = width;
                endX = 0.0f;

                if (moveToRight){
                    startX *= -1;
                }
            }else{
                startX = 0.0f;
                endX = width;

                if (!moveToRight){
                    endX *= -1;
                }
            }
        }

        public void Lerp(float t){
            rectTransform.anchoredPosition = new Vector2(startX + ((endX - startX) * t), rectTransform.anchoredPosition.y);
        }
}
