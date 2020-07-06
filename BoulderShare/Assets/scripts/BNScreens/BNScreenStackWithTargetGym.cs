using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;

namespace BoulderNotes {
public class BNScreenStackWithTargetGym : BNTStack
{
    [SerializeField] private BNGym targetGym;
    [SerializeField] private BNRoute targetRoute;
    [SerializeField] private BNRecord targetRecord;
    private BNWallImageNames targetImageNames;
    private InputItemsView.TargetItem targetItemToInput;
    private WallTypeMap.Type targetWallType;
    private BNGradeMap.Grade targetGrade;
    private RTape targetTape;
    private string targetString;
    private Sprite targetSprite;
    private SortToggle.SortType targetSortType;
    private bool inputWalltype = false;
    private bool inputGrade = false;
    private DisplayImageView.DisplayType targetDisplayType;

    public override void Init(){
        base.Init();
        targetGym = null;
        targetRecord = null;
        targetRecord = null;
        ClearInputs();
    }
    public void ClearInputs(){
        targetImageNames = null;
        targetTape = null;
        targetSprite = null;
        targetWallType = WallTypeMap.Type.Slab;
        inputWalltype = false;
        targetGrade = BNGradeMap.Grade.None;
        inputGrade = false;
        targetItemToInput = InputItemsView.TargetItem.None;
        targetSortType = SortToggle.SortType.None;  
        targetDisplayType = DisplayImageView.DisplayType.WallImages;
    }
    public BNGym GetTargetGym(){
        if (targetGym == null){
            return null;
        }
        return targetGym.Clone();
    }
    //シャローコピーであることに注意（recordはコピーしていない）
    public BNRoute GetTargetRoute(){
        if (targetRoute == null){
            return null;
        }
        return targetRoute.Clone();
    }

    public BNRecord GetTargetRecord(){
        if (targetRecord == null){
            return null;
        }
        return targetRecord.Clone();
    }

    public BNWallImageNames GetTargetImageNames(){
        return targetImageNames;
    }
    public InputItemsView.TargetItem GetTargetItemToInput(){
        return targetItemToInput;
    }
    public WallTypeMap.Type GetTargetWallType(){
        return targetWallType;
    }
    public DisplayImageView.DisplayType GetTargetDisplayType(){
        return targetDisplayType;
    }
    public bool InputWallType(){
        return inputWalltype;
    }
    public bool InputGrade(){
        return inputGrade;
    }
    public BNGradeMap.Grade GetTargetGrade(){
        return targetGrade;
    }
    public RTape GetTargetTape(){
        return targetTape;
    }
    public string GetTargetString(){
        return targetString;
    }
    public Sprite GetTargetSprite(){
        return targetSprite;
    }
    public SortToggle.SortType GetTargetSortType(){
        return targetSortType;
    }

    //gym,wall,routeは階層構造を維持する
    //gymの参照が変わると、wallとrouteの参照ができないようにしなくてはいけない
    public void StoreTargetGym(string gymID){
        targetGym = BNGymDataCenter.Instance.ReadGymFromCache(gymID);
    }

    public void StoreTargetRoute(string routeID){
        ClearRoute();
        if (targetGym == null){
            return ;
        }

        foreach(BNRoute r in targetGym.GetRoutes()){
            if (r.GetID().Equals(routeID)){
                targetRoute = r;
                break;
            }
        }
    }
    public void StoreTargetRecord(string recordID){
        ClearRecord();
        if (targetRoute == null){
            return ;
        }

        foreach(BNRecord r in targetRoute.GetRecords()){
            if (r.GetID().Equals(recordID)){
                targetRecord = r;
                break;
            }
        }
    }

    public void SetTargetImageNames(BNWallImageNames names){
        targetImageNames = names;
    }
    public void SetTargetItemToInput(InputItemsView.TargetItem item){
        targetItemToInput = item;
    }
    public void SetTargetWallType(WallTypeMap.Type type){
        targetWallType = type;
        inputWalltype = true;
    }
    public void SetTargetGrade(BNGradeMap.Grade grade){
        targetGrade = grade;
        inputGrade = true;
    }
    public void SetTargetTape(RTape tape){
        targetTape = tape;
    }
    public void SetTargetString(string str){
        targetString = str;
    }
    public void SetTargetSprite(Sprite sprite){
        targetSprite = sprite;
    }
    public void SetTargetSortType(SortToggle.SortType sortType){
        targetSortType = sortType;
    }
    public void SetTargetDisplayType(DisplayImageView.DisplayType t){
        targetDisplayType = t;
    }
/*
    public Sprite LoadWallImage(string fileName){
        if (targetGym == null){
            return null;
        }

        return LoadWallImage(targetGym, fileName);
    }

    public Sprite LoadWallImage(BNGym gym, string fileName){
        if (gym == null){
            return null;
        }
        Texture2D texture = BNGymDataCenter.Instance.LoadWallImage(gym,fileName);
        Sprite sprite = Sprite.Create(
            texture, 
            new Rect(0.0f, 0.0f, texture.width, texture.height), 
            new Vector2(0.5f, 0.5f),
            texture.height/4);

        return sprite;
    }*/
    public void LoadImageAsync(string fileName, LoadImageDelegate del){
        if (targetGym == null){
            return ;
        }
        LoadImageAsync(targetGym, fileName, del);
    }
    public void LoadImageAsync(BNGym gym, string fileName, LoadImageDelegate del){
        if (gym == null){
            return ;
        }
        BNGymDataCenter.Instance.LoadImageAsync(gym,fileName, del);
    }
    public Sprite LoadImageByES3(string fileName){
        if (targetGym == null){
            return null;
        }
        return BNGymDataCenter.Instance.LoadWallImageByES3(targetGym,fileName);
    }
    public Texture2D LoadTextureByES3(string fileName){
        if (targetGym == null){
            return null;
        }
        return BNGymDataCenter.Instance.LoadTextureByES3(targetGym,fileName);
    }
    public void WriteGym(BNGym gym, BNImage bnImage = null){
        BNGymDataCenter.Instance.WriteGym(gym, bnImage);
    }
    
    public void WriteRoute(BNRoute route, List<BNWallImage> wallImages = null){
        if (targetGym == null || route == null || string.IsNullOrEmpty(route.GetID())){
            return ;
        }
        //既にrouteIDが存在する場合、作らない
        string routeID = route.GetID();
        if (targetGym.GetRoutes().Any(x => x.GetID().Equals(routeID))){
            return ;
        }
        targetGym.AddRoute(route);
        BNGymDataCenter.Instance.ModifyGym(targetGym);

        if (wallImages == null){
            return ;
        }
        BNGymDataCenter.Instance.SaveWallImages(targetGym, wallImages);
    }
    public void WriteRecord(BNRecord record){
        if (targetGym == null || targetRoute == null || record == null || string.IsNullOrEmpty(record.GetID())){
            return ;
        }
        //既にrecordIDが存在する場合、作らない
        string recordID = record.GetID();
        if (targetRoute.GetRecords().Any(x => x.GetID().Equals(recordID))){
            return ;
        }
        targetRoute.AddRecord(record);
        BNGymDataCenter.Instance.ModifyGym(targetGym);
    }

    //targetgymを保存
    public void ModifyGym(BNImage bnImage = null){
        ModifyGym(targetGym, bnImage);
    }

    public void ModifyGym(BNGym gym, BNImage gradeTableImage = null){
        if (targetGym == null || gym == null){
            return ;
        }
        if (!targetGym.GetID().Equals(gym.GetID())){
            return ;
        }
        string file = targetGym.GetGradeTableImagePath();
        if (gradeTableImage != null && !string.IsNullOrEmpty(file)){
            BNGymDataCenter.Instance.DeleteWallImage(targetGym, file);
        }
        bool success = BNGymDataCenter.Instance.ModifyGym(gym, gradeTableImage);
        if (success){
            ClearRoute();
            targetGym = gym;                
        }       
    }
    
    //routeは必ずcloneしたものにする
    public void ModifyRoute(BNRoute route, List<BNWallImage> addWallImages = null, List<string> removeList = null){
        if (targetGym == null || targetRoute == null || route == null || string.IsNullOrEmpty(route.GetID())){
            return ;
        }

        if (targetRoute.GetID().Equals(route.GetID())){
            if (removeList != null){
                BNGymDataCenter.Instance.DeleteWallImages(targetGym, removeList);
            }
            if (addWallImages != null){
                BNGymDataCenter.Instance.SaveWallImages(targetGym, addWallImages);
            }

            targetGym.DeleteRoute(targetRoute);
            targetGym.AddRoute(route);
            BNGymDataCenter.Instance.ModifyGym(targetGym);
            ClearRecord();
            targetRoute = route;
        }
    }
    public void ModifyRecord(BNRecord record){
        if (targetGym == null || targetRoute == null || targetRecord == null || record == null || string.IsNullOrEmpty(record.GetID())){
            return ;
        }

        if (targetRecord.GetID().Equals(record.GetID())){
            targetRoute.DeleteRecord(targetRecord.GetID());
            targetRoute.AddRecord(record);
            BNGymDataCenter.Instance.ModifyGym(targetGym);
            targetRecord = record;
        }   
    }
    public void DeleteGym(){
        if (targetGym == null){
            return ;
        }

        BNGymDataCenter.Instance.DeleteGym(targetGym.GetID());
        ClearGym();
    }

    public void DeleteGradeTable(){
        //Debug.Log("deleteGradeTable");
        if (targetGym == null){
            return ;
        }

        string file = targetGym.GetGradeTableImagePath();
        if (!string.IsNullOrEmpty(file)){
            //Debug.Log("go DeleteWallImage");
            BNGymDataCenter.Instance.DeleteWallImage(targetGym, file);
            targetGym.SetGradeTableImagePath("");
            BNGymDataCenter.Instance.ModifyGym(targetGym, null);
        }
    }
    public void DeleteRoute(){
        if (targetGym == null || targetRoute == null){
            return ;
        }

        targetGym.DeleteRoute(targetRoute);
        BNGymDataCenter.Instance.DeleteWallImages(targetGym, targetRoute.GetAllWallImageFileNames());
        BNGymDataCenter.Instance.ModifyGym(targetGym);
        ClearRoute();
    }
    public void DeleteRecord(){
        if (targetGym == null || targetRoute == null || targetRecord == null){
            return ;
        }

        targetRoute.DeleteRecord(targetRecord.GetID());
        BNGymDataCenter.Instance.ModifyGym(targetGym);
        ClearRecord();
    }

    public void ClearGym(){
        targetGym = null;
        targetRoute = null;
        targetRecord = null;
    }

    public void ClearRoute(){
        targetRoute = null;
        targetRecord = null;
    }

    public void ClearRecord(){
        targetRecord = null;
    }
    public string FindOldestWallImageName(){
        if (targetGym == null){
            return "";
        }
        foreach(BNRoute r in targetGym.GetRoutes()){
            BNWallImageNames n = r.GetFirstWallImageFileNames();
            if (n != null && !string.IsNullOrEmpty(n.fileName)){
                return n.fileName;
            }
        }

        return "";
    }
    public bool HasWallImage(string fileName){
        if (targetGym == null || string.IsNullOrEmpty(fileName)){
            return false;
        }

        foreach(BNRoute r in targetGym.GetRoutes()){
            foreach(BNWallImageNames names in r.GetWallImageFileNames()){
                if (names.fileName.Equals(fileName)){
                    return true;
                }
            }
        }
        return false;
    }
}
}