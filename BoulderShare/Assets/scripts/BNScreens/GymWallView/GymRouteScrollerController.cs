using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedUI.EnhancedScroller;

namespace BoulderNotes{
public class GymRouteScrollerController : MonoBehaviour, IEnhancedScrollerDelegate
{
    [SerializeField] private GymWallView view;
    private List<GymRouteScrollerDataBase> _data;

    public EnhancedScroller myScroller;
    public GymRouteCellView gymRouteCellViewPrefab;
    public GymRoutePastCellView gymRoutePastCellViewPrefab;
    //実データの個数
    private int dataCount;
    public int numberOfCellsPerRow = 3;
    /*
    void Start(){
        _data = new List<GymRouteScrollerDataBase>();

        _data.Add(new GymRouteScrollerData() { period = "2020/1/4～", grade = 1, isRoute = true, isAddButton = false});
        _data.Add(new GymRouteScrollerData() { period = "2020/1/8～", grade = 2, isRoute = true, isAddButton = false});
        _data.Add(new GymRouteScrollerData() { period = "2020/1/8～", grade = 3, isRoute = true, isAddButton = false});
        _data.Add(new GymRouteScrollerData() { period = "2020/1/4～", grade = 4, isRoute = true, isAddButton = false});
        _data.Add(new GymRouteScrollerData() { period = "2020/1/4～", grade = 5, isRoute = true, isAddButton = false});
        _data.Add(new GymRouteScrollerData() { isAddButton = true});
        _data.Add(new GymRoutePastScrollerData());

        //grid表示するセルの個数
        dataCount = _data.Count-1;
        myScroller.Delegate = this;
        myScroller.ReloadData();
    }*/

    public void Init(){
        _data = new List<GymRouteScrollerDataBase>();
        myScroller.Delegate = this;        
    }

    public void FetchData(IReadOnlyList<BNRoute> routes){
        _data.Clear();
        //Debug.Log("rotues:"+routes.Count);
        foreach(BNRoute route in routes){
            GymRouteScrollerData data = new GymRouteScrollerData();
            data.route = route;
            data.period = route.GetPeriod();
            data.grade = route.GetGrade();
            data.isRoute = true;
            data.isAddButton = false;
            data.isFinished = route.IsFinished();
            _data.Add(data);
        }
        _data.Add(new GymRouteScrollerData() { isAddButton = true});
        _data.Add(new GymRoutePastScrollerData());
        dataCount = _data.Count-1;
        myScroller.ReloadData();
    }

    public int GetNumberOfCells(EnhancedScroller scroller){
        return Mathf.CeilToInt((float)(dataCount) / (float)numberOfCellsPerRow) + 1;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex){
        //Debug.Log("dataIndex:"+dataIndex);
        //Debug.Log("_data.Count:"+_data.Count);
        //Debug.Log("index:"+CalcIndex(dataIndex));

        if (_data[CalcIndex(dataIndex)] is GymRouteScrollerData){
            return 132f;
        }

        return 50f;
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex){
        int ind = CalcIndex(dataIndex);
        if (_data[ind] is GymRouteScrollerData){
            GymRouteCellView cellView = scroller.GetCellView(gymRouteCellViewPrefab) as GymRouteCellView;
            cellView.SetData(ref _data, ind, dataCount, ToRouteView, ToRegisterView);
            return cellView;
        }else{
            return scroller.GetCellView(gymRoutePastCellViewPrefab) as GymRoutePastCellView;
        }
    }

    public int CalcIndex(int dataIndex){
        //実データの最後のインデックス
        int acDataIndex = Mathf.CeilToInt((float)(dataCount) / (float)numberOfCellsPerRow) - 1;
        

        if (dataIndex <= acDataIndex){
            return dataIndex * numberOfCellsPerRow;
        }

        return (dataCount-1) + (dataIndex - acDataIndex);

    }

    public void ToRouteView(BNRoute route){
        view.SaveTargetRouteInStack(route);
        view.ToRouteView();
    }

    public void ToRegisterView(){
        view.ToRegisterView();
    }

}
}