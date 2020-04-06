using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedUI.EnhancedScroller;
using System.Linq;

namespace BoulderNotes{
public class GymRouteScrollerController : MonoBehaviour, IEnhancedScrollerDelegate
{
    [SerializeField] private GymView view;
    private List<GymRouteScrollerDataBase> _data;

    public EnhancedScroller myScroller;
    public GymRouteCellView gymRouteCellViewPrefab;

    public void Init(){
        _data = new List<GymRouteScrollerDataBase>();
        myScroller.Delegate = this;        
    }

    private List<BNRoute> SortByRouteID(IReadOnlyList<BNRoute> routes){
        return routes.OrderByDescending(x=>x.GetID()).ToList();
    }

    public void FetchData(IReadOnlyList<BNRoute> routes){
        _data.Clear();
        List<BNRoute> sorted = SortByRouteID(routes);
        //Debug.Log("rotues:"+routes.Count);
        foreach(BNRoute route in sorted){
            GymRouteScrollerData data = new GymRouteScrollerData();
            data.routeID = route.GetID();
            data.wallImages = route.GetAllWallImageFileNames();
            data.tags = route.GetTags();
            data.clearRate = route.GetTotalClearRate();
            data.clearStatus = route.GetTotalClearStatus();
            data.period = route.GetPeriod();
            data.grade = route.GetGrade();
            data.isFavorite = route.IsFavorite();
            data.isFinished = route.IsFinished();
            data.routeTape = route.GetTape();
            _data.Add(data);
        }

        myScroller.ReloadData();
    }

    public int GetNumberOfCells(EnhancedScroller scroller){
        return _data.Count;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex){
        if (_data[dataIndex] is GymRouteScrollerData){
            return 150f;
        }

        return 50f;
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex){
        if (_data[dataIndex] is GymRouteScrollerData){
            GymRouteCellView cellView = scroller.GetCellView(gymRouteCellViewPrefab) as GymRouteCellView;
            cellView.SetData(_data[dataIndex] as GymRouteScrollerData, view.GetBelongingStack() as BNScreenStackWithTargetGym, ToRouteView);
            return cellView;
        }
        return null;
    }

    public void ToRouteView(string routeID){
        view.SaveTargetRouteInStack(routeID);
        view.ToRouteView();
    }

    public void ToRegisterView(){
        view.ToRegisterView();
    }

}
}