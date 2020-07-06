using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedUI.EnhancedScroller;
using System.Linq;

namespace BoulderNotes{
public class GymRoutesScrollerController : MonoBehaviour, IEnhancedScrollerDelegate
{
    [SerializeField] private GymRoutesView view;
    private List<GymRoutesScrollerDataBase> _data;
    [SerializeField] private List<GymRoutesScrollerData> _fullData;
    public EnhancedScroller myScroller;
    public GymRoutesCellView gymRoutesCellViewPrefab;
    public SortGymRoutesCellView sortGymRoutesCellViewPrefab;
    public GradesGymRoutesCellView gradesGymRoutesCellViewPrefab;
    [SerializeField] private int numberOfCellsPerRow = 2;
    private int singleCellNumber = 2;
    private int numOfData;
    private bool finishedRoutes;
    public void Init(){
        _data = new List<GymRoutesScrollerDataBase>();
        _fullData = new List<GymRoutesScrollerData>();
        myScroller.Delegate = this;        
        numOfData = 0;
        finishedRoutes = false;
    }

    public void SetFinishedRoutes(bool b){
        finishedRoutes = b;
    }

    private List<BNRoute> SortByRouteID(IReadOnlyList<BNRoute> routes){
        return routes.OrderByDescending(x=>x.GetID()).ToList();
    }

    public void FetchData(IReadOnlyList<BNRoute> routes){
        _fullData.Clear();
        List<BNRoute> sorted = SortByRouteID(routes);

        foreach(BNRoute route in sorted){
            GymRoutesScrollerData data = new GymRoutesScrollerData();
            data.routeID = route.GetID();
            if (route.GetWallImageFileNames() != null && route.GetWallImageFileNames().Any()){
                BNWallImageNames names = route.GetFirstWallImageFileNames();
                data.wallImageNames = names;
            }

            data.period = route.GetShortPeriod();
            data.grade = route.GetGrade();
            data.isFavorite = route.IsFavorite();
            data.isFinished = route.IsFinished();
            data.routeTape = route.GetTape();
            data.wallTypeName = route.GetWallTypeName();
            data.clearStatus = route.GetTotalClearStatus();
            data.clearRate = route.GetTotalClearRate();
            _fullData.Add(data);
        }
    }

    public void LookUp(BNGradeMap.Grade grade){
        _data.Clear();
        _data.Add(new GradesGymRoutesScrollerData());
        _data.Add(new SortGymRoutesScrollerData());
        foreach(GymRoutesScrollerData data in _fullData){
            if (finishedRoutes == data.isFinished && (grade == BNGradeMap.Grade.None || data.grade == grade)){
                _data.Add(data);
            }
        }
        numOfData = _data.Count-singleCellNumber;
        myScroller.ReloadData();
    }

    public int[] GetNumSplitedByGrade(){
        int[] routes = new int[BNGradeMap.Entity.GetSize()];
        foreach(GymRoutesScrollerData data in _fullData){
           // Debug.Log("grade:"+data.grade+" "+ (int)data.grade);
           if (data.isFinished == finishedRoutes){
               routes[(int)data.grade]++;
           }
        }

        return routes;
    }

    public int GetNumberOfCells(EnhancedScroller scroller){
        return Mathf.CeilToInt((float)(numOfData) / (float)numberOfCellsPerRow) + singleCellNumber ;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex){
        int index = GetIndex(dataIndex);
        if (_data[index] is GymRoutesScrollerData){
            return 190f;
        }
        if (_data[index] is SortGymRoutesScrollerData){
            return 45f;
        }
        if (_data[index] is GradesGymRoutesScrollerData){
            return 80f;
        }

        return 175f;
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex){
        int index = GetIndex(dataIndex);
        if (_data[index] is GymRoutesScrollerData){
            GymRoutesCellView cellView = scroller.GetCellView(gymRoutesCellViewPrefab) as GymRoutesCellView;
            cellView.SetData(ref _data, index, numOfData+singleCellNumber, view.GetBelongingStack() as BNScreenStackWithTargetGym, ToRouteView, ToDisplayImageView);
            return cellView;
        }

        if (_data[index] is SortGymRoutesScrollerData){
            return scroller.GetCellView(sortGymRoutesCellViewPrefab) as SortGymRoutesCellView;
        }
        if (_data[index] is GradesGymRoutesScrollerData){
            return scroller.GetCellView(gradesGymRoutesCellViewPrefab) as GradesGymRoutesCellView;
        }
        return null;
    }

    public int GetIndex(int dataIndex){
        if (dataIndex < singleCellNumber){
            return dataIndex;
        }
        return dataIndex * 2 - singleCellNumber;
    }

    public void ToRouteView(string routeID){
        view.SaveTargetRouteInStack(routeID);
        view.ToRouteView();
    }

    public void ToRegisterView(){
        view.ToRegisterView();
    }
    public void ToDisplayImageView(string routeID){
        view.SaveTargetRouteInStack(routeID);
        view.ToDisplayImageViewForImages();
    }

    public Vector2 GetCurentScrollPosition(){
        return myScroller.ScrollRect.content.anchoredPosition;
    }

}
}