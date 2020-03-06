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
        [SerializeField] private List<BNWall> walls;
        [SerializeField]private string gradeTableImagePath;
        [SerializeField]private string gymName;
        [SerializeField]private string gymBoardImagePath;

        public BNGym(){
            id = BNGymDataCenter.PREFIX_ID_GYM + DateTime.Now.ToString(BNGymDataCenter.FORMAT_ID);
            wallIDs = new List<string>();
            walls = new List<BNWall>();
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

        public void SetWalls(List<BNWall> list){
            walls = new List<BNWall>(list);
        }

        public void DeleteWall(BNWall wall){
            walls.Remove(wall);
        }

        public void AddWall(BNWall wall){
            walls.Add(wall);
        }
        public List<BNWall> GetWalls(){
            return new List<BNWall>(walls);
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
        [SerializeField] private string period;
        [SerializeField] private string start;
        [SerializeField] private string end;
        [SerializeField] private List<string> wallImagefileNames;
        [SerializeField] private List<string> routeIDs;
        [SerializeField] private List<BNRoute> routes;
        [SerializeField] private bool isFinished;

        public BNWall(){
            id = BNGymDataCenter.PREFIX_ID_WALL + DateTime.Now.ToString(BNGymDataCenter.FORMAT_ID);
            routeIDs = new List<string>();
            routes = new List<BNRoute>();
            wallImagefileNames = new List<string>();
            SetStart(DateTime.Now);
            isFinished = false;
        }
        public BNWall Clone(){
            return (BNWall)this.MemberwiseClone();
        }
        public string GetID(){
            return id;
        }

        public List<string> GetWallImageFileNames(){
            return new List<string>(wallImagefileNames);
        }
        public void AddWallImageFileName(string wallImage){
            wallImagefileNames.Add(wallImage);
        }
        public void SetWallImageFileNames(List<string> list){
            wallImagefileNames = new List<string>(list);
        }

        public bool IsFinished(){
            return isFinished;
        }

        public void SetID(string str){
            id = str;
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

        public void SetStart(DateTime t){
            start = t.ToString(BNGymDataCenter.FORMAT_DATE);
        }

        public void SetEnd(DateTime t){
            end = t.ToString(BNGymDataCenter.FORMAT_DATE);
        }
        public void ClearEnd(){
            end = "";
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

        public void SetRoutes(List<BNRoute> list){
            routes = new List<BNRoute>(list);
        }
        public void AddRoute(BNRoute route){
            routes.Add(route);
        }

        public void DeleteRoute(BNRoute route){
            routes.Remove(route);
        }

        public List<BNRoute> GetRoutes(){
            return new List<BNRoute>(routes);
        }
    }

    public class BNWallImage{
        public string fileName;
        public Texture2D texture;

        public BNWallImage(Texture2D tex){
            fileName = BNGymDataCenter.PREFIX_ID_WALLIMAGE + DateTime.Now.ToString(BNGymDataCenter.FORMAT_ID) + BNGymDataCenter.WALLIMAGE_EXTENSION;
            texture = tex;
        }

    }
    [Serializable]
    public class BNRoute{
        public enum ClearStatus {NoAchievement, RP, Flash, Onsight};
        [SerializeField] private string id;

        [SerializeField] private WallTypeMap.Type wallType;
        [SerializeField] private RTape tape;
        [SerializeField] private List<BNMark> marks;
        [SerializeField] private string routeImagePath;
        [SerializeField] private BNGradeMap.Grade grade;
        [SerializeField] private ClearStatus totalClearStatus;
        [SerializeField] private string start;
        [SerializeField] private string end;

        [SerializeField] private List<BNRecord> records;
        [SerializeField] private bool isFinished;
        [SerializeField] private bool usedKante;
        [SerializeField] private bool isFavorite;
        [SerializeField] private bool hasInsight;
        [SerializeField] private int totalClearRate;
        [SerializeField] private List<string> tags;

        public BNRoute(){
            id = BNGymDataCenter.PREFIX_ID_ROUTE + DateTime.Now.ToString(BNGymDataCenter.FORMAT_ID);
            wallType = WallTypeMap.Type.Slab;
            marks = new List<BNMark>();
            records = new List<BNRecord>();
            tape = null;
            grade = BNGradeMap.Grade.None;
            SetStart(DateTime.Now);
            isFinished = false;
            usedKante = false;
            isFavorite = false;
            hasInsight = true;
            totalClearStatus = ClearStatus.NoAchievement;
            totalClearRate = 0;
            tags = new List<string>();
        }
        public BNRoute Clone(){
            return (BNRoute)this.MemberwiseClone();
        }
        public string GetID(){
            return id;
        }

        public RTape GetTape(){
            if (tape == null){
                return null;
            }
            return tape.Clone();
        }
        public BNGradeMap.Grade GetGrade(){
            return grade;
        }
        public string GetGradeName(){
            return BNGradeMap.Entity.GetGradeName(grade);
        }
        public void SetGrade(BNGradeMap.Grade g){
            grade = g;
        }
        public void SetHasInsight(bool b){
            hasInsight = b;
        }

        public int GetTotalClearRate(){
            return totalClearRate;
        }

        public ClearStatus GetTotalClearStatus(){
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
            return records.Count + 1;
        }
        public void SetID(string str){
            id = str;
        }

        public void SetTape(RTape t){
            if (t == null){
                tape = null;
                return ;
            }
            tape = t.Clone();
        }

        public string GetPeriod(){
            return start + "～" + end;
        }

        public void SetStart(DateTime t){
            start = t.ToString(BNGymDataCenter.FORMAT_DATE);
        }

        public void SetEnd(DateTime t){
            end = t.ToString(BNGymDataCenter.FORMAT_DATE);
        }

        public void ClearEnd(){
            end = "";
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
            records = list.OrderBy(x=>x.GetID()).ToList();
            ReCalculateInfo();
        }

        //同じIDを持つBNRecordがrecordsに存在しないと仮定
        public void AddRecord(BNRecord rec){
            int i = 0;
            foreach(BNRecord r in records){
                if (r.GetID().CompareTo(rec.GetID()) > 0){
                    break;
                }
                i++;
            }
            records.Insert(i, rec);
            ReCalculateInfo();
        }

        public void DeleteRecord(string recID){
            BNRecord deleteTarget = null;
            foreach(BNRecord r in records){
                if(r.GetID().Equals(recID)){
                    deleteTarget = r;
                    break;
                }
            }
            records.Remove(deleteTarget);
            ReCalculateInfo();
        }

        //recordsはidでソートされていると仮定している
        private void ResetRecordTryNumbers(){
            int n = 1;
 
            foreach(BNRecord rec in records){
                rec.SetTryNumber(n);
                n++;
            }
        }
        public void ReCalculateInfo(){
            CalculateStatus();
            ResetRecordTryNumbers();
        }
        private void CalculateStatus(){
            bool isComplete = false;
            int maxClearRate = 0;
    
            foreach(BNRecord record in records){
                int rate = record.GetCompleteRate();
                if (record.GetTryNumber() == 1 && rate == 100){
                    if (hasInsight){
                        totalClearStatus = BNRoute.ClearStatus.Flash;
                    }else{
                        totalClearStatus = BNRoute.ClearStatus.Onsight;
                    }
                    totalClearRate = 100;
                    return ;
                }

                if (rate == 100){
                    isComplete = true;
                }

                if (maxClearRate < rate){
                    maxClearRate = rate;
                }
            }

            if (isComplete){
                totalClearStatus = BNRoute.ClearStatus.RP;
            }else{
                totalClearStatus = BNRoute.ClearStatus.NoAchievement;
            }

            totalClearRate = maxClearRate;
        }

        public string GetWallTypeName(){
            return WallTypeMap.Entity.GetWallTypeName(wallType);
        }
        public WallTypeMap.Type GetWallType(){
            return wallType;
        }        

        public void SetWallType(WallTypeMap.Type t){
            wallType = t;
        }

        public void AddTag(string str){
            if (!HasTag(str)){
                return ;
            }

            tags.Add(str);
        }
        public List<string> GetTags(){
            return new List<string>(tags);
        }
        public void SetTags(List<string> list){
            tags = new List<string>(list);
        }

        public bool HasTag(string str){
            return tags.Contains(str);
        }

    }
	[Serializable]
    public class BNMark{
        public Vector2 pos;
        public Vector2[] shape;
    }
    [Serializable]
    public class RTape{
        public Quaternion imageRot;
        public string spriteName;
        public string tapeText;
        public Color color;
        public RTape Clone(){
            return (RTape)this.MemberwiseClone();
        }
    }
    [Serializable]
    public class BNRecord{
        public enum Condition{Worst, Bad, Normal, Good, Best};
        [SerializeField] private string id;

        [SerializeField] private string date;
        [SerializeField] private Condition condition;
        [SerializeField] private string comment;
        [SerializeField] private int completeRate;
        [SerializeField] private int tryNumber;

        public BNRecord(){
            id = BNGymDataCenter.PREFIX_ID_RECORD + DateTime.Now.ToString(BNGymDataCenter.FORMAT_ID);
            SetDate(DateTime.Now);
        }

        public bool IsSame(BNRecord rec){
            return id.Equals(rec.GetID());
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
        public void SetDate(DateTime t){
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

    public class BNTriple{
        public BNRoute route;
        public BNWall wall;
        public BNGym gym;

        public BNTriple(BNGym g, BNWall w, BNRoute r){
            route = r;
            gym = g;
            wall = w;
        }
    }
}