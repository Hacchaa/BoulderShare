using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoulderNotes {
public class BNScreenStackWithTargetGym : BNTStack
{
    [SerializeField] private BNGym targetGym;
    [SerializeField] private BNWall targetWall;
    [SerializeField] private BNRoute targetRoute;
    [SerializeField] private BNRecord targetRecord;

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
    public void StoreTargetGym(BNGym gym){
        targetGym = gym;
    }
    public void StoreTargetGym(string gymID){
        ClearGym();
        if (string.IsNullOrEmpty(gymID)){
            return ;
        }
        targetGym = BNGymDataCenter.Instance.ReadGym(gymID);
    }

    public void StoreTargetWall(BNWall wall){
        targetWall = wall;
    }
  
    public void StoreTargetWall(string wallID){
        ClearWall();
        if (targetGym == null || string.IsNullOrEmpty(wallID)){
            return ;
        }
        targetWall = BNGymDataCenter.Instance.ReadWall(wallID, targetGym.GetID());
    }

    public void StoreTargetRoute(BNRoute route){
        targetRoute = route;
    }

    public void StoreTargetRoute(string routeID){
        ClearRoute();
        if (targetGym == null || targetWall == null || string.IsNullOrEmpty(routeID)){
            return ;
        }
        targetRoute = BNGymDataCenter.Instance.ReadRoute(routeID, targetWall.GetID(), targetGym.GetID());
    }
    public void StoreTargetRecord(BNRecord rec){
        targetRecord = rec;
    }
    public void StoreTargetRecord(string id){
        ClearRecord();
        if (targetRoute == null){
            return ;
        }
        targetRecord = targetRoute.GetRecord(id);
    }

    public void WriteGym(BNGym gym){
        targetGym = gym;
        BNGymDataCenter.Instance.WriteGym(gym);
    }
    public void WriteWall(BNWall wall){
        if (targetGym == null || wall == null){
            return ;
        }
        targetWall = wall;
        BNGymDataCenter.Instance.WriteWall(wall, targetGym);
    }
    public void WriteRoute(BNRoute route){
        if (targetGym == null || targetWall == null || route == null){
            return ;
        }
        targetRoute = route;
        BNGymDataCenter.Instance.WriteRoute(route, targetWall, targetGym);
    }
    public void WriteRecord(BNRecord record){
        if (targetGym == null || targetWall == null || targetRoute == null || record == null){
            return ;
        }
        targetRecord = record;

        targetRoute.AddRecord(record);

        BNGymDataCenter.Instance.ModifyRoute(targetRoute, targetWall, targetGym);
    }
    public void ModifyGym(BNGym gym){
        ClearWall();
        targetGym = gym;
        BNGymDataCenter.Instance.ModifyGym(gym);
    }
    public void ModifyWall(BNWall wall){
        if (targetGym == null || wall == null){
            return ;
        }
        ClearRoute();
        targetWall = wall;
        BNGymDataCenter.Instance.ModifyWall(wall, targetGym);
    }
    public void ModifyRoute(BNRoute route){
        if (targetGym == null || targetWall == null || route == null){
            return ;
        }
        ClearRecord();
        targetRoute = route;
        BNGymDataCenter.Instance.ModifyRoute(route, targetWall, targetGym);
    }
    public void ModifyRecord(BNRecord record){
        if (targetGym == null || targetWall == null || targetRoute == null || record == null){
            return ;
        }
        targetRecord = record;
        List<BNRecord> recList = new List<BNRecord>(targetRoute.GetRecords());

        int i = 0;
        bool hasRecord = false;
        foreach(BNRecord rec in recList){
            if (rec.GetID().Equals(record.GetID())){
                hasRecord = true;
                break;
            }
            i++;
        }
        if (!hasRecord){
            return ;
        }

        recList.RemoveAt(i);
        recList.Insert(i, record);
        targetRoute.SetRecords(recList);

        BNGymDataCenter.Instance.ModifyRoute(targetRoute, targetWall, targetGym);        
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
        BNGymDataCenter.Instance.DeleteWall(targetWall.GetID(), targetGym.GetID());
        ClearWall();
    }
    public void DeleteRoute(){
        if (targetGym == null || targetWall == null || targetRoute == null){
            return ;
        }
        BNGymDataCenter.Instance.DeleteRoute(targetRoute.GetID(), targetWall.GetID(), targetGym.GetID());
        ClearRoute();
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