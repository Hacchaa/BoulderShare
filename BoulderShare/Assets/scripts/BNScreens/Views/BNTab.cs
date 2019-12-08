using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BNTab : MonoBehaviour
{
    public void ToHomeTab(){
        BNScreens.Instance.ChangeScreenStack(BNScreens.BNTabName.Home);
    }
    public void ToFavoriteTab(){
        BNScreens.Instance.ChangeScreenStack(BNScreens.BNTabName.Favorite);
    }
    public void ToStatisticsTab(){
        BNScreens.Instance.ChangeScreenStack(BNScreens.BNTabName.Statistics);
    }
    public void ToOtherTab(){
        BNScreens.Instance.ChangeScreenStack(BNScreens.BNTabName.Other);
    }
}
