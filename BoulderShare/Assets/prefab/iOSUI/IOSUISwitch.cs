using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace BoulderNotes{
public class IOSUISwitch : MonoBehaviour
{
    [SerializeField] private Color onColor;
    [SerializeField] private Color offColor;
    [SerializeField] private RectTransform mark;
    private Button button;
    private Image image;
    private Sequence sequence;
    [SerializeField] private bool isCurrentOn;
    private float markX = 9.7f;
    private float animDur = 0.1f;
    public void Init(bool isOn){
        if (button == null){
            button = GetComponent<Button>();
            button.onClick.AddListener(OnClicked);            
        }
        if (image == null){
            image = GetComponent<Image>();
        }
        sequence = null;
        ChangeImmediately(isOn);
    }

    private void OnClicked(){
        ChangeWithAnim(!isCurrentOn);
    }

    private void ChangeWithAnim(bool b){
        if (sequence != null || isCurrentOn == b){
            return ;
        }

        sequence = DOTween.Sequence();
        sequence.OnStart(()=>{
            isCurrentOn = b;
        });
        if (b){
            sequence.Append(image.DOColor(onColor, animDur));
            sequence.Join(mark.DOAnchorPos(new Vector2(markX, 0f), animDur));
        }else{
            sequence.Append(image.DOColor(offColor, animDur));
            sequence.Join(mark.DOAnchorPos(new Vector2(-markX, 0f), animDur));
        }
        sequence.OnComplete(()=>{
            sequence = null;
        });
        sequence.Play();
    }

    private void ChangeImmediately(bool b){
        isCurrentOn = b;

        if (isCurrentOn){
            image.color = onColor;
            mark.anchoredPosition = new Vector2(markX, 0f);
        }else{
            image.color = offColor;
            mark.anchoredPosition = new Vector2(-markX, 0f);
        }
    }

    public bool IsOn(){
        return isCurrentOn;
    }
    public void SetIsOn(bool b){
        ChangeImmediately(b);
    }
}
}
