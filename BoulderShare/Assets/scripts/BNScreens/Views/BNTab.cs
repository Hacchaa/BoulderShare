using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoulderNotes{
public class BNTab : SingletonMonoBehaviour<BNTab>
{
    [SerializeField] private List<BNTabItem> tabItems;
    [SerializeField] private Color focusColor;
    [SerializeField] private Color defocusColor;

    public void Init(){
        FocusTab(BNScreens.BNTabName.Home);
    }

    public void FocusTab(BNScreens.BNTabName tabName){
        foreach(BNTabItem item in tabItems){
            if (tabName == item.GetTabName()){
                item.Focus(focusColor);
            }else{
                item.DeFocus(defocusColor);
            }
        }
    }
    public void ToHomeTab(){
        FocusTab(BNScreens.BNTabName.Home);
        BNScreens.Instance.ChangeScreenStack(BNScreens.BNTabName.Home);
    }
    public void ToFavoriteTab(){
        FocusTab(BNScreens.BNTabName.Favorite);
        BNScreens.Instance.ChangeScreenStack(BNScreens.BNTabName.Favorite);
    }
    public void ToAddTab(){
        FocusTab(BNScreens.BNTabName.Add);
        BNScreens.Instance.ChangeScreenStack(BNScreens.BNTabName.Add);
    }
    public void ToStatisticsTab(){
        FocusTab(BNScreens.BNTabName.Statistics);
        BNScreens.Instance.ChangeScreenStack(BNScreens.BNTabName.Statistics);
    }
    public void ToOtherTab(){
        FocusTab(BNScreens.BNTabName.Other);
        BNScreens.Instance.ChangeScreenStack(BNScreens.BNTabName.Other);
    }
}
}