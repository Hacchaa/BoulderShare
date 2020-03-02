using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedUI.EnhancedScroller;
using System.Linq;

namespace BoulderNotes{
public class GymPastScrollerController : MonoBehaviour, IEnhancedScrollerDelegate
{
    [SerializeField] private GymPastView view;
    private List<GymWallScrollerData> _data;
    public EnhancedScroller myScroller;
    public GymWallCellView gymWallCellViewPrefab;
    [SerializeField] private float wallCellHeight = 240f;
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

        BNScreenStackWithTargetGym stack = null;
        if (view.GetBelongingStack() is BNScreenStackWithTargetGym){
            stack = view.GetBelongingStack() as BNScreenStackWithTargetGym;
        }
        IEnumerable<BNWall> sortedList ;
        if (walls.Any()){
            sortedList = walls.OrderByDescending(x => x.GetID());
            RectTransform cellRect = myScroller.GetComponent<RectTransform>();
            //Debug.Log("width:"+cellRect.rect.width);
            foreach(BNWall wall in sortedList){
                if (!wall.IsFinished()){
                    continue;
                }
                GymWallScrollerData data = new GymWallScrollerData();
                data.wallID = wall.GetID();
                data.fileNames = wall.GetWallImageFileNames();
                data.period = wall.GetPeriod();
                data.stack = stack;
                data.fitHeight = wallCellHeight;
                data.fitWidth = cellRect.rect.width - (myScroller.padding.left + myScroller.padding.right);
                _data.Add(data);
            }
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

    public void ToGymWallView(string wallID){
        view.SaveTargetWallInStack(wallID);
        view.ToGymWallView();
    }
}
}