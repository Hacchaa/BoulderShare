using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using System.IO;
using UnityEngine.AddressableAssets;

namespace BoulderNotes {
public class StatisticsView : BNScreen
{
    //gradeMapと同期させる
    [SerializeField] private AssetReference[] medalImagesRef;
    [SerializeField] private SVMedalView[] medals;
    [SerializeField] private ClassificationView cView;
    public override void InitForFirstTransition(){
        cView.Init();
    }

    public override void UpdateScreen(){
        BNGradeMap.Grade[] bestGrades = AggregateBestGrade();

        //NoAchievementは取り扱わない
        for(int i = 1 ; i < medals.Length ; i++){
            medals[i].SetData(bestGrades[i], medalImagesRef[(int)bestGrades[i]]);
        }
    }

    private BNGradeMap.Grade[] AggregateBestGrade(){
        BNGradeMap.Grade[] bestGrades = new BNGradeMap.Grade[Enum.GetNames(typeof(BNRoute.ClearStatus)).Length];
        IReadOnlyList<BNGym> gyms = BNGymDataCenter.Instance.ReadGyms();

        foreach(BNGym gym in gyms){
            foreach(BNWall wall in gym.GetWalls()){
                foreach(BNRoute route in wall.GetRoutes()){
                    int status = (int)route.GetTotalClearStatus();
                    int grade = (int)route.GetGrade();

                    if (grade > (int)bestGrades[status]){
                        bestGrades[status] = (BNGradeMap.Grade)grade;
                    }
                }
            }
        }

        return bestGrades;
    }
}
}