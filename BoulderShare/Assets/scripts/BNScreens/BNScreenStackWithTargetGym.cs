using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;

namespace BoulderNotes {
public class BNScreenStackWithTargetGym : BNTStack
{
    [SerializeField] private BNGym targetGym;
    [SerializeField] private BNWall targetWall;
    [SerializeField] private BNRoute targetRoute;
    [SerializeField] private BNRecord targetRecord;

    public override void Init(){
        base.Init();
        targetGym = null;
        targetWall = null;
        targetRecord = null;
        targetRecord = null;
    }
    public BNGym GetTargetGym(){
        if (targetGym == null){
            return null;
        }
        return targetGym.Clone();
    }
    public BNWall GetTargetWall(){
        if (targetWall == null){
            return null;
        }
        return targetWall.Clone();
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

    public void StoreTargetWall(string wallID){
        ClearWall();
        if (targetGym == null){
            return ;
        }

        foreach(BNWall w in targetGym.GetWalls()){
            if (w.GetID().Equals(wallID)){
                targetWall = w;
                break;
            }
        }
    }

    public void StoreTargetRoute(string routeID){
        ClearRoute();
        if (targetWall == null){
            return ;
        }

        foreach(BNRoute r in targetWall.GetRoutes()){
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
    }
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

    public void WriteGym(BNGym gym){
        BNGymDataCenter.Instance.WriteGym(gym);
    }
    
    public void WriteWall(BNWall wall, List<BNWallImage> wallImageList){
        if (targetGym == null || wall == null || string.IsNullOrEmpty(wall.GetID())){
            return ;
        }

        //既にwallIDが存在する場合、作らない
        string wallID = wall.GetID();
        if (targetGym.GetWalls().Any(x => x.GetID().Equals(wallID))){
            return ;
        }
        targetGym.AddWall(wall);
        BNGymDataCenter.Instance.ModifyGym(targetGym);

        if (wallImageList == null){
            return ;
        }
        BNGymDataCenter.Instance.SaveWallImages(targetGym, wallImageList);
    }
    public void WriteRoute(BNRoute route){
        if (targetGym == null || targetWall == null || route == null || string.IsNullOrEmpty(route.GetID())){
            return ;
        }
        //既にrouteIDが存在する場合、作らない
        string routeID = route.GetID();
        if (targetWall.GetRoutes().Any(x => x.GetID().Equals(routeID))){
            return ;
        }
        targetWall.AddRoute(route);
        BNGymDataCenter.Instance.ModifyGym(targetGym);
    }
    public void WriteRecord(BNRecord record){
        if (targetGym == null || targetWall == null || targetRoute == null || record == null || string.IsNullOrEmpty(record.GetID())){
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

    public void ModifyGym(BNGym gym){
        if (targetGym == null || gym == null){
            return ;
        }
        if (!targetGym.GetID().Equals(gym.GetID())){
            return ;
        }
        bool success = BNGymDataCenter.Instance.ModifyGym(gym);
        if (success){
            ClearWall();
            targetGym = gym;                
        }       
    }
    
    //wallは必ずcloneしたものにする
    public void ModifyWall(BNWall wall, List<BNWallImage> addWallImageList){
        if (targetGym == null || targetWall == null || wall == null || string.IsNullOrEmpty(wall.GetID())){
            return ;
        }
 
        if (!targetWall.GetID().Equals(wall.GetID())){
            return ;
        }

        List<string> newList = wall.GetWallImageFileNames();
        List<string> oldList = targetWall.GetWallImageFileNames();
        if (addWallImageList != null){
            BNGymDataCenter.Instance.SaveWallImages(targetGym, addWallImageList);
        }

        
        List<string> remList = new List<string>(oldList.Except(newList));
        BNGymDataCenter.Instance.DeleteWallImages(targetGym, remList);

        targetGym.DeleteWall(targetWall);
        targetGym.AddWall(wall);
        BNGymDataCenter.Instance.ModifyGym(targetGym);
        ClearRoute();
        targetWall = wall;
    }
    public void ModifyRoute(BNRoute route){
        if (targetGym == null || targetWall == null || targetRoute == null || route == null || string.IsNullOrEmpty(route.GetID())){
            return ;
        }

        if (targetRoute.GetID().Equals(route.GetID())){
            targetWall.DeleteRoute(targetRoute);
            targetWall.AddRoute(route);
            BNGymDataCenter.Instance.ModifyGym(targetGym);
            ClearRecord();
            targetRoute = route;
        }
    }
    public void ModifyRecord(BNRecord record){
        if (targetGym == null || targetWall == null || targetRoute == null || targetRecord == null || record == null || string.IsNullOrEmpty(record.GetID())){
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
    public void DeleteWall(){
        if (targetGym == null || targetWall == null){
            return ;
        }

        targetGym.DeleteWall(targetWall);
        BNGymDataCenter.Instance.DeleteWallImages(targetGym, targetWall.GetWallImageFileNames());
        BNGymDataCenter.Instance.ModifyGym(targetGym);
        ClearWall();
    }
    public void DeleteRoute(){
        if (targetGym == null || targetWall == null || targetRoute == null){
            return ;
        }
        BNWall tWall = targetWall;

        targetWall.DeleteRoute(targetRoute);
        BNGymDataCenter.Instance.ModifyGym(targetGym);
        ClearRoute();
    }
    public void DeleteRecord(){
        if (targetGym == null || targetWall == null || targetRoute == null || targetRecord == null){
            return ;
        }

        BNWall tWall = targetWall;
        BNRoute tRoute = targetRoute;

        targetRoute.DeleteRecord(targetRecord.GetID());
        BNGymDataCenter.Instance.ModifyGym(targetGym);
        ClearRecord();
    }

    public void ClearGym(){
        targetGym = null;
        targetWall = null;
        targetRoute = null;
        targetRecord = null;
    }

    public void ClearWall(){
        targetWall = null;
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