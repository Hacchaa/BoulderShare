using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace BoulderNotes{
    public delegate void OnButtonClickedDelegate();
    public delegate void OnButtonClickedDelegateWithString(string str);
    public delegate void OnButtonClickedDelegateWithBNGym(BNGym gym);
    public delegate void OnButtonClickedDelegateWithBNWall(BNWall wall);
    public delegate void OnButtonClickedDelegateWithBNRoute(BNRoute route);
    public delegate void OnButtonClickedDelegateWithBNRecord(BNRecord record);
    
    public class BNDataStructure : MonoBehaviour
    {

    }

    [Serializable]
    public class BNGym{
        [SerializeField]private string id;
        [SerializeField]private List<string> wallIDs;
        [SerializeField]private string gradeTableImagePath;
        [SerializeField]private string gymName;
        [SerializeField]private string gymBoardImagePath;

        public BNGym(){
            id = BNGymDataCenter.PREFIX_ID_GYM + DateTime.Now.ToString(BNGymDataCenter.FORMAT_ID);
            wallIDs = new List<string>();
        }
        public BNGym Clone(){
            return (BNGym)this.MemberwiseClone();
        }
        public string GetID(){
            return id;
        }

        public string GetGymName(){
            return gymName;
        }

        public void SetID(string str){
            id = str;
        }

        public void SetGymName(string name){
            gymName = name;
        }

        public void SetWallIDs(List<string> list){
            wallIDs = new List<string>(list);
        }

        public void AddWallID(string wallID){
            wallIDs.Add(wallID);
        }
        public List<string> GetWallIDs(){
            return new List<string>(wallIDs);
        }
    }
    [Serializable]
    public class BNGymIDs{
        public List<string> idList;

        public BNGymIDs(){
            idList = new List<string>();
        }
    }
    [Serializable]
    public class BNWall{
        [SerializeField] private string id;
        [SerializeField] private WallTypeMap.Type wallType;
        [SerializeField] private string period;
        [SerializeField] private string start;
        [SerializeField] private string end;
        [SerializeField] private string wallImagePath;
        [SerializeField] private List<string> routeIDs;
        [SerializeField] private bool isFinished;

        public BNWall(){
            id = BNGymDataCenter.PREFIX_ID_WALL + DateTime.Now.ToString(BNGymDataCenter.FORMAT_ID);
            routeIDs = new List<string>();
        }
        public BNWall Clone(){
            return (BNWall)this.MemberwiseClone();
        }
        public string GetID(){
            return id;
        }

        public WallTypeMap.Type GetWallType(){
            return wallType;
        }

        public bool IsFinished(){
            return isFinished;
        }

        public void SetID(string str){
            id = str;
        }
        public void SetWallType(WallTypeMap.Type t){
            wallType = t;
        }

        public string GetPeriod(){
            return start + "～" + end;
        }

        public string GetStart(){
            return start;
        }

        public string GetEnd(){
            return end;
        }

        public void SetStart(string str){
            start = str;
        }

        public void SetEnd(string str){
            end = str;
        }
        public void SetIsFinished(bool b){
            isFinished = b;
        }
        public void SetRouteIDs(List<string> list){
            routeIDs = new List<string>(list);
        }
        public void AddRouteID(string routeID){
            routeIDs.Add(routeID);
        }

        public List<string> GetRouteIDs(){
            return new List<string>(routeIDs);
        }
    }
    [Serializable]
    public class BNRoute{

        [SerializeField] private string id;
        [SerializeField] private List<BNMark> marks;
        [SerializeField] private string routeImagePath;
        [SerializeField] private BNGradeMap.Grade grade;
        [SerializeField] private int totalClearStatus;
        [SerializeField] private string start;
        [SerializeField] private string end;
        [SerializeField] private List<BNRecord> records;
        [SerializeField] private bool isFinished;
        [SerializeField] private bool usedKante;
        [SerializeField] private bool isFavorite;

        public BNRoute(){
            id = BNGymDataCenter.PREFIX_ID_ROUTE + DateTime.Now.ToString(BNGymDataCenter.FORMAT_ID);
            marks = new List<BNMark>();
            records = new List<BNRecord>();
        }
        public BNRoute Clone(){
            return (BNRoute)this.MemberwiseClone();
        }
        public string GetID(){
            return id;
        }
        public BNGradeMap.Grade GetGrade(){
            return grade;
        }
        public void SetGrade(BNGradeMap.Grade g){
            grade = g;
        }

        public int GetTotalClearStatus(){
            return totalClearStatus;
        }
        public bool IsFinished(){
            return isFinished;
        }

        public bool IsUsedKante(){
            return usedKante;
        }

        public bool IsFavorite(){
            return isFavorite;
        }

        public IReadOnlyList<BNRecord> GetRecords(){
            return records;
        }

        public BNRecord GetRecord(string id){
            BNRecord rec = null;

            foreach(BNRecord r in records){
                if(r.GetID().Equals(id)){
                    rec = r;
                    break;
                }
            }
            return rec;
        }

        public int GetNewTryNumber(){
            int n = 0;
            foreach(BNRecord rec in records){
                if (n < rec.GetTryNumber()){
                    n = rec.GetTryNumber();
                }
            }
            return n + 1;
        }
        public void SetID(string str){
            id = str;
        }
        public void SetTotalClearStatus(int s){
            totalClearStatus = s;
        }
        public string GetPeriod(){
            return start + "～" + end;
        }

        public void SetStart(string str){
            start = str;
        }

        public void SetEnd(string str){
            end = str;
        }
        public void SetIsFinished(bool b){
            isFinished = b;
        }

        public void SetIsUsedKante(bool b){
            usedKante = b;
        }

        public void SetIsFavorite(bool b){
            isFavorite = b;
        }

        public void SetRecords(IReadOnlyList<BNRecord> list){
            records = new List<BNRecord>(list);
        }

        //同じIDを持つBNRecordがrecordsに存在しないと仮定
        public void AddRecord(BNRecord rec){
            records.Add(rec);
        }

        public void DeleteRecord(BNRecord rec){
            records.Remove(rec);
            ResetRecordTryNumbers();
        }

        private void ResetRecordTryNumbers(){
            int n = 1;
            IEnumerable<BNRecord> sorted = records.OrderBy(x => x.GetDate());
            foreach(BNRecord rec in sorted){
                rec.SetTryNumber(n);
                n++;
            }
        }
    }
	[Serializable]
    public class BNMark{
        public Vector2 pos;
        public Vector2[] shape;
    }
    [Serializable]
    public class BNRecord{
        public enum Condition{Worst, Bad, Normal, Good, Best};
        [SerializeField] private string id;
        [SerializeField] private string time;
        [SerializeField] private string date;
        [SerializeField] private Condition condition;
        [SerializeField] private string comment;
        [SerializeField] private int completeRate;
        [SerializeField] private int tryNumber;
        public BNRecord(){
            id = BNGymDataCenter.PREFIX_ID_RECORD + DateTime.Now.ToString(BNGymDataCenter.FORMAT_ID);
        }
        public BNRecord Clone(){
            return (BNRecord)this.MemberwiseClone();
        }
        public string GetID(){
            return id;
        }

        public string GetDate(){
            return date;
        }
        public string GetTime(){
            return time;
        }

        public Condition GetCondition(){
            return condition;
        }

        public string GetComment(){
            return comment;
        }

        public int GetCompleteRate(){
            return completeRate;
        }

        public int GetTryNumber(){
            return tryNumber;
        }

        public void SetID(string str){
            id = str;
        }
        public void SetTime(string str){
            time = str;
            DateTime t = DateTime.ParseExact(time, BNGymDataCenter.FORMAT_TIME, null);
            date = t.ToString(BNGymDataCenter.FORMAT_DATE);

        }

        public void SetCondition(Condition cond){
            condition = cond;
        }

        public void SetCompleteRate(int rate){
            completeRate = rate;
        }

        public void SetTryNumber(int n){
            tryNumber = n;
        }
        public void SetComment(string str){
            comment = str;
        }
    }
}