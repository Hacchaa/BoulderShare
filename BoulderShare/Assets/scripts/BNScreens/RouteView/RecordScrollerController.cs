using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedUI.EnhancedScroller;
using System.Linq;
using UnityEngine.AddressableAssets;

namespace BoulderNotes{
public class RecordScrollerController : MonoBehaviour, IEnhancedScrollerDelegate
{
    [SerializeField] private RouteView view;
    [SerializeField] private AssetReference[] conditionImageRef;
    private List<RecordScrollerDataBase> _data;

    public EnhancedScroller myScroller;
    public RecordCellView recordCellViewPrefab;
    public RecordSubTitleCellView recordSubTitleCellViewPrefab;
    public RecordOverviewCellView recordOverviewCellViewPrefab;
    public RecordDateTitleCellView recordDateTitleCellViewPrefab;
    public RecordLineCellView recordLineCellViewPrefab;
    public RecordBrankCellView recordBrankCellViewPrefab;
    public RecordMainInfoCellView recordMainInfoCellViewPrefab;
    public RecordRouteWallImageCellView recordRouteWallImageCellViewPrefab;
    public RecordRouteWallImageTitleCellView recordRouteWallImageTitleCellViewPrefab;
    public void Init(){
        _data = new List<RecordScrollerDataBase>();
        myScroller.Delegate = this;        
    }

    public void FetchData(BNRoute route){
        _data.Clear();
        List<List<RecordScrollerData>> dataList = new List<List<RecordScrollerData>>();
        List<string> dateList = new List<string>();
        string curDate = "";
        List<RecordScrollerData> targetList = new List<RecordScrollerData>();

        //listを日付で並び変える
        IReadOnlyList<BNRecord> list = route.GetRecords();
        IEnumerable<BNRecord> sortedList ;
        if (list.Any()){
            sortedList = list.OrderByDescending(x => x.GetTime()).ThenByDescending(x => x.GetTryNumber());

            //日付毎にリストに格納
            foreach(BNRecord record in sortedList){
                RecordScrollerData data = new RecordScrollerData();
                string dat = record.GetDate2();
                data.date = record.GetDate3();
                data.record = record;
                data.completeRate = record.GetCompleteRate();
                data.comment = record.GetComment();
                data.tryNumber = record.GetTryNumber();
                data.conditionImageRef = conditionImageRef[(int)record.GetCondition()];

                if (string.IsNullOrEmpty(curDate) || !curDate.Equals(dat)){
                    dateList.Add(dat);
                    targetList = new List<RecordScrollerData>();
                    dataList.Add(targetList);
                    
                    curDate = dat;
                }
                targetList.Add(data);
            }
        }

        //壁画像が表示される場所を空けておく
        _data.Add(new RecordBrankScrollerData());
        RecordMainInfoScrollerData d = new RecordMainInfoScrollerData();
        if (view.GetBelongingStack() is BNScreenStackWithTargetGym){
            d.gymName = (view.GetBelongingStack() as BNScreenStackWithTargetGym).GetTargetGym().GetGymName();
        }
        d.tape = route.GetTape();
        d.grade = route.GetGradeName();
        d.date = route.GetPeriod();
        d.usedKante = route.IsUsedKante();
        d.completeStatusName = BNRoute.GetClearStatusName(route.GetTotalClearStatus());
        d.wallTypeName = route.GetWallTypeName();
        _data.Add(d);

        //_dataを作る
        int dayN = dateList.Count;
        int tryN = list.Count;
        _data.Add(new RecordOverviewScrollerData(){days = dayN, tryCount = tryN, completeRate = route.GetTotalClearRate()});
        _data.Add(new RecordSubTitleScrollerData());

        int n = dateList.Count;
        for(int i = 0 ; i < n ; i++){
            _data.Add(new RecordDateTitleScrollerData(){date = "挑戦"+(n-i)+"日目　"+dateList[i]});
            List<RecordScrollerData> l = dataList[i];
            int m = dataList[i].Count;
            for(int j = 0 ; j < m ; j++){
                _data.Add(l[j]);

                //最後以外の要素が追加された場合、線を引く
                if (j < m - 1){
                    _data.Add(new RecordLineScrollerData());
                }
            }
        }
        if (tryN > 0){
            _data.Add(new RecordLineScrollerData());
        }

        //フォトギャラリー
        _data.Add(new RecordRouteWallImageTitleScrollerData());
        foreach(BNWallImageNames nam in route.GetWallImageFileNames()){
            _data.Add(new RecordRouteWallImageScrollerData(){names = nam});
        }
        myScroller.ReloadData();
    }

    public int GetNumberOfCells(EnhancedScroller scroller){
        return _data.Count;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex){
        if (_data[dataIndex] is RecordScrollerData){
            return 60f;
        }
        if (_data[dataIndex] is RecordOverviewScrollerData){
            return 80f;
        }
        if (_data[dataIndex] is RecordSubTitleScrollerData){
            return 80f;
        }
        if (_data[dataIndex] is RecordDateTitleScrollerData){
            return 30f;
        }
        if (_data[dataIndex] is RecordBrankScrollerData){
            return 240f;
        }
        if (_data[dataIndex] is RecordMainInfoScrollerData){
            return 158f;
        }
        if (_data[dataIndex] is RecordRouteWallImageTitleScrollerData){
            return 80f;
        }
        if (_data[dataIndex] is RecordRouteWallImageScrollerData){
            return 200f;
        }

        //case in RecordLineScrollerData
        return 1f;
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex){
    
        if (_data[dataIndex] is RecordScrollerData){
            RecordCellView recordCellView = scroller.GetCellView(recordCellViewPrefab) as RecordCellView;
            RecordScrollerData data = _data[dataIndex] as RecordScrollerData;
            recordCellView.SetData(data);
            recordCellView.clickDel = ToRecordView;
            return recordCellView;
        }
        if (_data[dataIndex] is RecordOverviewScrollerData){
            RecordOverviewCellView recordOverviewCellView = scroller.GetCellView(recordOverviewCellViewPrefab) as RecordOverviewCellView;
            recordOverviewCellView.SetData((_data[dataIndex] as RecordOverviewScrollerData));
            return recordOverviewCellView;
        }
        if (_data[dataIndex] is RecordSubTitleScrollerData){
            RecordSubTitleCellView recordSubTitleCellView = scroller.GetCellView(recordSubTitleCellViewPrefab) as RecordSubTitleCellView;
            recordSubTitleCellView.SetData((_data[dataIndex] as RecordSubTitleScrollerData));
            return recordSubTitleCellView;
        }
        if (_data[dataIndex] is RecordDateTitleScrollerData){
            RecordDateTitleCellView　recordDateTitleCellView = scroller.GetCellView(recordDateTitleCellViewPrefab) as RecordDateTitleCellView;
            recordDateTitleCellView.SetData((_data[dataIndex] as RecordDateTitleScrollerData));
            return recordDateTitleCellView ;
        }
        if (_data[dataIndex] is RecordLineScrollerData){
            return scroller.GetCellView(recordLineCellViewPrefab) as RecordLineCellView;
        }
        if (_data[dataIndex] is RecordMainInfoScrollerData){
            RecordMainInfoCellView recordMainCellView = scroller.GetCellView(recordMainInfoCellViewPrefab) as RecordMainInfoCellView;
            recordMainCellView.Init();
            recordMainCellView.SetData((_data[dataIndex] as RecordMainInfoScrollerData));
            return recordMainCellView;
        }
        if (_data[dataIndex] is RecordRouteWallImageScrollerData){
            RecordRouteWallImageCellView recordRouteWallImageCellView = scroller.GetCellView(recordRouteWallImageCellViewPrefab) as RecordRouteWallImageCellView;
            recordRouteWallImageCellView.SetData((_data[dataIndex] as RecordRouteWallImageScrollerData), (view.GetBelongingStack() as BNScreenStackWithTargetGym), ToDisplayImageView);
            return recordRouteWallImageCellView;
        }
        if (_data[dataIndex] is RecordRouteWallImageTitleScrollerData){
            RecordRouteWallImageTitleCellView recordRouteWallImageTitleCellView = scroller.GetCellView(recordRouteWallImageTitleCellViewPrefab) as RecordRouteWallImageTitleCellView;
            recordRouteWallImageTitleCellView.SetData((_data[dataIndex] as RecordRouteWallImageTitleScrollerData), ToAddPhotoView);
            return recordRouteWallImageTitleCellView;
        }

        //case in RecordLineScrollerData
        return scroller.GetCellView(recordBrankCellViewPrefab) as RecordBrankCellView;

    }

    public void ToAddPhotoView(){
    }
    public void ToDisplayImageView(BNWallImageNames names){
        view.SaveTargetWallImageNamesInStack(names);
    }
 
    public void ToRecordView(BNRecord rec){
        view.SaveTargerRecordInStack(rec);
        view.ToRecordView();
    }

}
}