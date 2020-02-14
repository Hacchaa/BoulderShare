﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedUI.EnhancedScroller;
using System.Linq;

namespace BoulderNotes{
public class RecordScrollerController : MonoBehaviour, IEnhancedScrollerDelegate
{
    [SerializeField] private RouteView view;
    private List<RecordScrollerDataBase> _data;

    public EnhancedScroller myScroller;
    public RecordCellView recordCellViewPrefab;
    public RecordSubTitleCellView recordSubTitleCellViewPrefab;
    public RecordOverviewCellView recordOverviewCellViewPrefab;
    public RecordDateTitleCellView recordDateTitleCellViewPrefab;
    public RecordLineCellView recordLineCellViewPrefab;

    public Sprite[] conditionImages;

    public void Init(){
        _data = new List<RecordScrollerDataBase>();
        myScroller.Delegate = this;        
    }

    public void FetchData(IReadOnlyList<BNRecord> list){
        _data.Clear();
        List<List<RecordScrollerData>> dataList = new List<List<RecordScrollerData>>();
        List<string> dateList = new List<string>();

        string curDate = "";
        List<RecordScrollerData> targetList = new List<RecordScrollerData>();
        int maxCompleteRate = 0;

        //listを日付で並び変える
        IEnumerable<BNRecord> sortedList ;
        if (list.Any()){
            sortedList = list.OrderByDescending(x => x.GetDate()).ThenByDescending(x => x.GetTryNumber());

            //日付毎にリストに格納
            foreach(BNRecord record in sortedList){
                RecordScrollerData data = new RecordScrollerData();
                data.date = record.GetDate();
                data.record = record;
                data.completeRate = record.GetCompleteRate();
                data.comment = record.GetComment();
                data.tryNumber = record.GetTryNumber();
                data.bodyCondition = record.GetCondition();

                if (maxCompleteRate < data.completeRate){
                    maxCompleteRate = data.completeRate;
                }

                if (string.IsNullOrEmpty(curDate) || !curDate.Equals(data.date)){
                    dateList.Add(data.date);
                    targetList = new List<RecordScrollerData>();
                    dataList.Add(targetList);
                    
                    curDate = data.date;
                }
                targetList.Add(data);
            }
        }

        //_dataを作る
        int dayN = dateList.Count;
        int tryN = list.Count;
        _data.Add(new RecordOverviewScrollerData(){days = dayN, tryCount = tryN, completeRate = maxCompleteRate});
        _data.Add(new RecordSubTitleScrollerData());

        int n = dateList.Count;
        for(int i = 0 ; i < n ; i++){
            _data.Add(new RecordDateTitleScrollerData(){date = dateList[i]});
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

        //case in RecordLineScrollerData
        return 1f;
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex){
    
        if (_data[dataIndex] is RecordScrollerData){
            RecordCellView recordCellView = scroller.GetCellView(recordCellViewPrefab) as RecordCellView;
            RecordScrollerData data = _data[dataIndex] as RecordScrollerData;
            recordCellView.SetData(data, conditionImages[(int)data.bodyCondition]);
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

        //case in RecordLineScrollerData
        return scroller.GetCellView(recordLineCellViewPrefab) as RecordLineCellView;

    }
 
    public void ToRecordView(BNRecord rec){
        view.SaveTargerRecordInStack(rec);
        view.ToRecordView();
    }

}
}