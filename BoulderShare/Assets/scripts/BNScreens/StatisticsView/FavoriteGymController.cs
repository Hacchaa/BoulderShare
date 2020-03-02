using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BoulderNotes{
public class FavoriteGymController : MonoBehaviour
{
    [SerializeField] private FavoriteGymView[] views;

    public void SetData(List<BNGym> gyms, List<int> days){
        int n = gyms.Count;

        for(int i = 0 ; i < views.Length ; i++){
            if (i < n){
                views[i].gameObject.SetActive(true);
                views[i].SetData(gyms[i], days[i]);
            }else{
                views[i].gameObject.SetActive(false);
            }
        }
    }
}
}