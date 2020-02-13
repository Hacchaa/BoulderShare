using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoulderNotes{
    public class CVTabUnderbar : MonoBehaviour
    {
        [SerializeField] private RectTransform bar;
        private float currentX;
        private float targetX;

        public void Init(float width, float posX){
            currentX = posX;
            targetX = posX;

            bar.sizeDelta = new Vector2(width, 0.0f);

            Lerp(0.0f);
        }

        public void SetTargetX(float f){
            targetX = f;
        }
        public void SetCurrentX(float f){
            currentX = f;
        }

        public void Lerp(float t){
            bar.anchoredPosition = new Vector2(currentX + ((targetX - currentX) * t), bar.anchoredPosition.y);
        }
    }
}
