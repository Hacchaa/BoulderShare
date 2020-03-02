using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BoulderNotes{
public class FavoriteGymView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TextMeshProUGUI daysText;

    public void SetData(BNGym gym, int days){
       text.text = gym.GetGymName();
       daysText.text = days + "日";
    }
}
}