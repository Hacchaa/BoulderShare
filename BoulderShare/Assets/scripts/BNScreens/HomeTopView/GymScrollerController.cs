using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedUI.EnhancedScroller;
using System.Linq;

namespace BoulderNotes{
public class GymScrollerController : MonoBehaviour, IEnhancedScrollerDelegate
{
    [SerializeField] private HomeTopView view;
    private List<GymScrollerDataBase> _data;

    public EnhancedScroller myScroller;
    public GymCellView gymCellViewPrefab;
    public GymCellFootView gymCellFootViewPrefab;

    public void Init(){
        _data = new List<GymScrollerDataBase>();
        myScroller.Delegate = this;        
    }

    public void FetchData(IReadOnlyList<BNGym> list, SortToggle.SortType sortType){
        _data.Clear();
        IEnumerable<BNGym> sorted = list;
        if (sortType == SortToggle.SortType.Latest){
            //sorted = list;
        }else if (sortType == SortToggle.SortType.Name){
            sorted = list.OrderBy(x => x.GetGymName());
        }else if(sortType == SortToggle.SortType.More){
            //sorted = list;
        }

        foreach(BNGym gym in sorted){
            GymScrollerData data = new GymScrollerData();
            data.gymName = gym.GetGymName();
            data.gymID = gym.GetID();
            data.boardImagePath = gym.GetBoardImagePath();
            _data.Add(data);
        }
        _data.Add(new GymScrollerDataBase());
        myScroller.ReloadData();
    }

    public int GetNumberOfCells(EnhancedScroller scroller){
        return _data.Count;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex){
        if (_data[dataIndex] is GymScrollerData){
            return 80f;
        }

        return 60f;
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex){
        if (_data[dataIndex] is GymScrollerData){
            GymCellView cellView = scroller.GetCellView(gymCellViewPrefab) as GymCellView;
            cellView.SetData((_data[dataIndex] as GymScrollerData));
            cellView.clickDel = ToGymView;
            return cellView;
        }else{
            GymCellFootView  cell = scroller.GetCellView(gymCellFootViewPrefab) as GymCellFootView;
            cell.clickDel = ToRegisterView;
            return cell;
        }
    }

    public void ToGymView(string gymID){
        view.SaveTargetGymInStack(gymID);
        view.ToGymView();
    }

    public void ToRegisterView(){
        view.ToRegisterView();
    }

}
}