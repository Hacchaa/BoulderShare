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

    public override void Init(){
        base.Init();
        targetGym = null;
        targetRecord = null;
        targetRecord = null;
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
    public void WriteGym(BNGym gym, BNImage bnImage = null){
        BNGymDataCenter.Instance.WriteGym(gym, bnImage);
    }
    
    public void WriteRoute(BNRoute route, BNWallImage wallImage = null){
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

        if (wallImage == null){
            return ;
        }
        BNGymDataCenter.Instance.SaveWallImage(targetGym, wallImage);
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
    public void ModifyGym(){
        ModifyGym(targetGym);
    }

    public void ModifyGym(BNGym gym, BNImage bnImage = null){
        if (targetGym == null || gym == null){
            return ;
        }
        if (!targetGym.GetID().Equals(gym.GetID())){
            return ;
        }
        bool success = BNGymDataCenter.Instance.ModifyGym(gym, bnImage);
        if (success){
            ClearRoute();
            targetGym = gym;                
        }       
    }
    
    //routeは必ずcloneしたものにする
    public void ModifyRoute(BNRoute route, BNWallImage addWallImage = null, List<string> removeList = null){
        if (targetGym == null || targetRoute == null || route == null || string.IsNullOrEmpty(route.GetID())){
            return ;
        }

        if (targetRoute.GetID().Equals(route.GetID())){
            if (removeList != null){
                BNGymDataCenter.Instance.DeleteWallImages(targetGym, removeList);
            }
            if (addWallImage != null){
                BNGymDataCenter.Instance.SaveWallImage(targetGym, addWallImage);
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
}
}