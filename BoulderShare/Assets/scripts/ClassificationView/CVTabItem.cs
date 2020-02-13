using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BoulderNotes{
    public class CVTabItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI tabName;
        private Color normal;
        private Color select;
        private RectTransform rectTransform;

        public void Init(Color n, Color s){
            normal = n;
            select = s;

            rectTransform = GetComponent<RectTransform>();
        }

        public float GetLocalX(){
            return rectTransform.anchoredPosition.x;
        }

        public void Focus(){
            tabName.color = select;
        }

        public void Defocus(){
            tabName.color = normal;
        }
    }
}
