using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedUI.EnhancedScroller;

namespace BoulderNotes{
public class GymWallScrollerController : MonoBehaviour, IEnhancedScrollerDelegate
{
    [SerializeField] private GymView view;
    private List<GymWallScrollerDataBase> _data;

    public EnhancedScroller myScroller;
    public GymWallCellView gymWallCellViewPrefab;
    public GymWallAddCellView gymWallAddCellViewPrefab;
    public GymWallPastCellView gymWallPastCellViewPrefab;
/*
    void Start(){
        _data = new List<GymWallScrollerDataBase>();

        _data.Add(new GymWallScrollerData() { gymWallName = "オーバーハング", gymWallPeriod = "2020/1/21～"});
        _data.Add(new GymWallScrollerData() { gymWallName = "垂壁", gymWallPeriod = "2020/1/21～"});
        _data.Add(new GymWallScrollerData() { gymWallName = "スラブ", gymWallPeriod = "2020/1/21～"});
        _data.Add(new GymWallAddScrollerData());
        _data.Add(new GymWallPastScrollerData());

        myScroller.Delegate = this;
        myScroller.ReloadData();
    }*/
    public void Init(){
        _data = new List<GymWallScrollerDataBase>();
        myScroller.Delegate = this;        
    }

    public void FetchData(IReadOnlyList<BNWall> walls){
        //Debug.Log("wall.COunt;"+walls.Count);
        _data.Clear();

        foreach(BNWall wall in walls){
            if (wall.IsFinished()){
                continue;
            }
            GymWallScrollerData data = new GymWallScrollerData();
            data.gymWallTypeName = WallTypeMap.Entity.GetWallTypeName(wall.GetWallType());
            data.gymWallPeriod = wall.GetPeriod();
            data.wall = wall;
            _data.Add(data);
        }
        _data.Add(new GymWallAddScrollerData());
        _data.Add(new GymWallPastScrollerData());
        myScroller.ReloadData();
    }

    public int GetNumberOfCells(EnhancedScroller scroller){
        return _data.Count;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex){
        if (_data[dataIndex] is GymWallScrollerData){
            return 240f;
        }

        return 50f;
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex){
        if (_data[dataIndex] is GymWallScrollerData){
            GymWallCellView cellView = scroller.GetCellView(gymWallCellViewPrefab) as GymWallCellView;
            cellView.SetData((_data[dataIndex] as GymWallScrollerData));
            cellView.clickDel = ToGymWallView;
            return cellView;
        }else if (_data[dataIndex] is GymWallAddScrollerData){
            GymWallAddCellView view = scroller.GetCellView(gymWallAddCellViewPrefab) as GymWallAddCellView;
            view.clickDel = ToRegisterView;
            return view;
        }else{
            GymWallPastCellView pastCellView = scroller.GetCellView(gymWallPastCellViewPrefab) as GymWallPastCellView;
            pastCellView.clickDel = ToGymPastView;
            return pastCellView;
        }
    }

    public void ToGymWallView(BNWall wall){
        view.SaveTargetWallInStack(wall);
        view.ToGymWallView();
    }
    public void ToGymPastView(){
        view.ToGymPastView();
    }

    public void ToRegisterView(){
        view.ToRegisterView();
    }
}
}