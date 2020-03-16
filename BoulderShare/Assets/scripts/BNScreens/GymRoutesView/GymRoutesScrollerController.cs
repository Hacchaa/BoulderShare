using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedUI.EnhancedScroller;
using System.Linq;

namespace BoulderNotes{
public class GymRoutesScrollerController : MonoBehaviour, IEnhancedScrollerDelegate
{
    [SerializeField] private GymRoutesView view;
    private List<GymRoutesScrollerData> _data;
    public EnhancedScroller myScroller;
    public GymRoutesCellView gymRoutesCellViewPrefab;
    [SerializeField] private int numberOfCellsPerRow = 2;
    private int numOfData;
    public void Init(){
        _data = new List<GymRoutesScrollerData>();
        myScroller.Delegate = this;        
        numOfData = 0;
    }

    private List<BNRoute> SortByRouteID(IReadOnlyList<BNRoute> routes){
        return routes.OrderByDescending(x=>x.GetID()).ToList();
    }

    public void FetchData(IReadOnlyList<BNRoute> routes){
        _data.Clear();
        List<BNRoute> sorted = SortByRouteID(routes);

        foreach(BNRoute route in sorted){
            GymRoutesScrollerData data = new GymRoutesScrollerData();
            data.routeID = route.GetID();
            if (route.GetWallImageFileNames() != null && route.GetWallImageFileNames().Any()){
                data.wallImagePath = route.GetWallImageFileNames()[0];
            }

            data.period = route.GetPeriod();
            data.gradeName = route.GetGradeName();
            data.isFavorite = route.IsFavorite();
            data.routeTape = route.GetTape();
            data.wallTypeName = route.GetWallTypeName();
            _data.Add(data);
        }
        numOfData = _data.Count;
        //Debug.Log("num:"+numOfData);
        //Debug.Log("cells:"+Mathf.CeilToInt((float)(numOfData) / (float)numberOfCellsPerRow));
        myScroller.ReloadData();
    }

    public int GetNumberOfCells(EnhancedScroller scroller){
        return Mathf.CeilToInt((float)(numOfData) / (float)numberOfCellsPerRow) ;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex){
        if (_data[dataIndex] is GymRoutesScrollerData){
            return 175f;
        }

        return 175f;
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex){
        int index = dataIndex*numberOfCellsPerRow;
        if (_data[dataIndex] is GymRoutesScrollerData){
            GymRoutesCellView cellView = scroller.GetCellView(gymRoutesCellViewPrefab) as GymRoutesCellView;
            cellView.SetData(ref _data, index, numOfData, view.GetBelongingStack() as BNScreenStackWithTargetGym, ToRouteView);
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