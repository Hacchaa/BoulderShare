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
using System.Threading.Tasks;
using System.Linq;

namespace BoulderNotes {
public class StatisticsView : BNScreen
{
    //gradeMapと同期させる
    [SerializeField] private AssetReference[] medalImagesRef;
    [SerializeField] private SVMedalView[] medals;
    [SerializeField] private ClassificationView cView;
    [SerializeField] private BN_CircleGraph clearGraph;
    [SerializeField] private RecommendedRouteController recommended;
    [SerializeField] private FavoriteGymController gymView;
    public override void InitForFirstTransition(){
        cView.Init();
        clearGraph.Init();
    }

    public override void UpdateScreen(){
        ProcBestGrades();
        ProcClearStatus();
        ProcRecommendedRoute();
        ProcFavoriteGym();
    }

    private async void ProcFavoriteGym(){
        List<GymPair> list = await Task<List<GymPair>>.Run(() => AggregateFavoriteGym());

        gymView.SetData(list.Select(x=>x.gym).ToList(), list.Select(x=>x.days).ToList());
    }

    private async void ProcRecommendedRoute(){
        int num = 2;
        BNTriple[] info = await Task<BNTriple[]>.Run(() => AggregateRecommendedRoute(num));

        recommended.SetData(info);
    }

    private async void ProcBestGrades(){
        BNGradeMap.Grade[] bestGrades = await Task<BNGradeMap.Grade[]>.Run(() => AggregateBestGrade());
        //NoAchievementは取り扱わない
        for(int i = 1 ; i < medals.Length ; i++){
            medals[i].SetData(bestGrades[i], medalImagesRef[(int)bestGrades[i]]);
        }
    }

    private async void ProcClearStatus(){
        int[] c = await Task<int[]>.Run(()=>AggregateClears());

        int[] clears = new int [c.Length-1];
        for(int i = 0 ; i < clears.Length ; i++){
            clears[i] = c[i+1];
        } 

        clearGraph.Make(clears);
    }

    private List<GymPair> AggregateFavoriteGym(){
        List<GymPair> list = new List<GymPair>();
        List<string> days = new List<string>();
        IReadOnlyList<BNGym> gyms = BNGymDataCenter.Instance.ReadGyms();

        foreach(BNGym gym in gyms){
            GymPair pair = new GymPair(gym);
            days.Clear();
            string lastTryID = "";
            foreach(BNWall wall in gym.GetWalls()){
                foreach(BNRoute route in wall.GetRoutes()){
                    foreach(BNRecord record in route.GetRecords()){
                        days.Add(record.GetDate());
                        if (string.IsNullOrEmpty(lastTryID) || lastTryID.CompareTo(record.GetID()) < 0){
                            lastTryID = record.GetID();
                        }
                    }
                }
            }
            pair.days = days.Distinct().ToList().Count;
            list.Add(pair);
            pair.lastTryID = lastTryID;
        }        
        return list.OrderByDescending(x=>x.days).ThenByDescending(x=>x.lastTryID).ToList();
    }

    private BNTriple[] AggregateRecommendedRoute(int num){
        if (num < 1){
            return null;
        }
        BNTriple[] info = new BNTriple[num];
        List<BNTriple> list = new List<BNTriple>();
        IReadOnlyList<BNGym> gyms = BNGymDataCenter.Instance.ReadGyms();

        foreach(BNGym gym in gyms){
            foreach(BNWall wall in gym.GetWalls()){
                foreach(BNRoute route in wall.GetRoutes()){
                    list.Add(new BNTriple(gym, wall, route));
                }
            }
        }

        IEnumerable<BNTriple> ite = list
            .Where(x=>x.route.GetTotalClearStatus() == BNRoute.ClearStatus.NoAchievement && !x.route.IsFinished())
            .OrderByDescending(x => x.route.GetTotalClearRate());

        int i = 0;
        foreach(BNTriple t in ite){
            if (i >= num){
                break;
            }
            info[i] = t;
            i++;
        }        

        return info;
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

        //もしRPがFlashかOnsightよりグレードが低い場合、更新する
        int best = (int)bestGrades[(int)BNRoute.ClearStatus.RP];
        if (best < (int)bestGrades[(int)BNRoute.ClearStatus.Flash]){
            best = (int)bestGrades[(int)BNRoute.ClearStatus.Flash];
        }
        if (best < (int)bestGrades[(int)BNRoute.ClearStatus.Onsight]){
            best =(int)bestGrades[(int)BNRoute.ClearStatus.Onsight];
        }

        bestGrades[(int)BNRoute.ClearStatus.RP] = (BNGradeMap.Grade)best;

        return bestGrades;
    }

    private int[] AggregateClears(){
        int[] clears = new int[Enum.GetNames(typeof(BNRoute.ClearStatus)).Length];
        IReadOnlyList<BNGym> gyms = BNGymDataCenter.Instance.ReadGyms();

        foreach(BNGym gym in gyms){
            foreach(BNWall wall in gym.GetWalls()){
                foreach(BNRoute route in wall.GetRoutes()){
                    clears[(int)route.GetTotalClearStatus()]++;
                }
            }
        }

        return clears;       
    }

    private class GymPair{
        public int days;
        public BNGym gym;
        public string lastTryID;

        public GymPair(BNGym g){
            days = 0;
            gym = g;
            lastTryID = "";
        }

    }
}
}