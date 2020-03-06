using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using System.IO;
using AdvancedInputFieldPlugin;

namespace BoulderNotes {
public class SelectRouteTagView : BNScreen
{
    [SerializeField] private AdvancedInputField aif;
    [SerializeField] private Transform tagRoot;
    [SerializeField] private RouteTagView tagPrefab;
    private BNRoute route;
    private List<string> tags;
    private BNScreenStackWithTargetGym stack;
    public override void InitForFirstTransition(){

    }

    private void ClearFields(){
        aif.Text = "";
        route = null;
        tags = null;
        stack = null;
        if (tagRoot != null){
            foreach(Transform t in tagRoot){
                Destroy(t.gameObject);
            }
        }
    }

    public override void UpdateScreen(){
        ClearFields();
        if (belongingStack != null && belongingStack is BNScreenStackWithTargetGym){
            stack = belongingStack as BNScreenStackWithTargetGym;
            route = stack.GetTargetRoute();
           
            if (route != null){
                tags = new List<string>();

                foreach(string str in route.GetTags()){
                    AddTag(str);
                }
            }
        }
    }

    public void AddTagFromInputField(){
        if(string.IsNullOrEmpty(aif.Text)){
            return ;
        }
        AddTag(aif.Text);
        aif.Text = "";
    }

    public void AddTag(string str){
        RouteTagView view = Instantiate<RouteTagView>(tagPrefab, tagRoot);
        view.gameObject.SetActive(true);
        view.SetData(str);
        view.ActiveDeleteButton(OnDeleteTag);  
        view.transform.SetAsFirstSibling();
        tags.Insert(0, str);      
    }

    private void OnDeleteTag(RouteTagView view){
        if (tags != null){
            tags.Remove(view.GetTagName());
        }
        Destroy(view.gameObject);
    }

    public void Save(){
        if (route == null || tags == null || stack == null){
            return ;
        }

        route.SetTags(tags);
        stack.ModifyRoute(route);
    }
    public void ReverseTransition(){
        BNScreens.Instance.ReverseTransition();
    }

}
}