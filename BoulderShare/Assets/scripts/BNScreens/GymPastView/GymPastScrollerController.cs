using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedUI.EnhancedScroller;

namespace BoulderNotes{
public class GymPastScrollerController : MonoBehaviour, IEnhancedScrollerDelegate
{
    [SerializeField] private GymPastView view;
    private List<GymWallScrollerData> _data;
    public EnhancedScroller myScroller;
    public GymWallCellView gymWallCellViewPrefab;
/*
    void Start(){
        _data = new List<GymWallScrollerData>();

        _data.Add(new GymWallScrollerData() { gymWallName = "オーバーハング", gymWallPeriod = "2020/1/21～"});
        _data.Add(new GymWallScrollerData() { gymWallName = "垂壁", gymWallPeriod = "2020/1/21～"});
        _data.Add(new GymWallScrollerData() { gymWallName = "スラブ", gymWallPeriod = "2020/1/21～"});

        myScroller.Delegate = this;
        myScroller.ReloadData();
    }*/
    public void Init(){
        _data = new List<GymWallScrollerData>();
        myScroller.Delegate = this;        
    }

    public void FetchData(IReadOnlyList<BNWall> walls){
        _data.Clear();

        foreach(BNWall wall in walls){
            if (!wall.IsFinished()){
                continue;
            }
            GymWallScrollerData data = new GymWallScrollerData();
            data.gymWallTypeName = WallTypeMap.Entity.GetWallTypeName(wall.GetWallType());
            data.gymWallPeriod = wall.GetPeriod();
            data.wall = wall;
            _data.Add(data);
        }

        myScroller.ReloadData();
    }

    public int GetNumberOfCells(EnhancedScroller scroller){
        return _data.Count;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex){
        return 240f;
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex){
        GymWallCellView cellView = scroller.GetCellView(gymWallCellViewPrefab) as GymWallCellView;
        cellView.SetData((_data[dataIndex] as GymWallScrollerData));
        cellView.clickDel = ToGymWallView;
        return cellView;
    }

    public void ToGymWallView(BNWall wall){
        view.SaveTargetWallInStack(wall);
        view.ToGymWallView();
    }
}
}