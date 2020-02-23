using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;

namespace BoulderNotes{
public class SVMedalView : MonoBehaviour
{
    [SerializeField] private Image medalImage;
    [SerializeField] private TextMeshProUGUI gradeText;

    public void SetData(BNGradeMap.Grade grade, AssetReference medalImageRef){
        gradeText.text = BNGradeMap.Entity.GetGradeName(grade);
        Addressables.LoadAssetAsync<Sprite>(medalImageRef).Completed += op =>{
            medalImage.sprite = op.Result;
        };
    }
}
}