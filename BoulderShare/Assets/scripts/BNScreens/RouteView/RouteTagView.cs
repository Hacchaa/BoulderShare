using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace BoulderNotes{
    public delegate void RouteTagDelegate(RouteTagView view); 
public class RouteTagView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tagNameText;
    [SerializeField] private GameObject deleteObj;
    private RouteTagDelegate deleteAction;
    public void SetData(string str){
        tagNameText.text = str;
        deleteAction = null;
    }

    public string GetTagName(){
        return tagNameText.text;
    }

    public void ActiveDeleteButton(RouteTagDelegate action){
        deleteAction = action;
        deleteObj.SetActive(true);
    }

    public void DeactiveDeleteButton(){
        deleteAction = null;
        deleteObj.SetActive(false);
    }

    public void OnClickDelete(){
        if (deleteAction != null){
            deleteAction(this);
        }
    }
}
}