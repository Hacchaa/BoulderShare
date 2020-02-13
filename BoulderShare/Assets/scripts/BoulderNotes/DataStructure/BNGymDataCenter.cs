using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//##ジムの読み書きを行うクラス
//##各画面はジムの更新のためにこのクラスにアクセスする

namespace BoulderNotes{
    public class BNGymDataCenter : SingletonMonoBehaviour<BNGymDataCenter>
    {
        public const string FORMAT_ID = "yyyyMMddHHmmssffff";
        public const string FORMAT_TIME = "yyyyMMddHHmmssffff";
        public const string FORMAT_DATE = "yyyy年M月d日";
        public const string PREFIX_ID_GYM = "G";
        public const string PREFIX_ID_WALL = "W";
        public const string PREFIX_ID_ROUTE = "R";
        //try
        public const string PREFIX_ID_RECORD = "T";
        private const string ES3_ROOTPATH = "Gyms";
        private const string ES3_FILE_BNGYMIDS = "gymIDs";
        private const string ES3_KEY_BNGYMIDS = "BNGymIDs";
        private const string ES3_KEY_GYM = "BNGym";
        private const string ES3_KEY_WALL = "BNWall";
        private const string ES3_KEY_ROUTE = "BNRoute";
        private const string ES3_EXTENSION = ".es3";
        private const string ES3_FILE_GYM = "gym";
        private const string ES3_FILE_WALL = "wall";
        private const string ES3_FILE_ROUTE = "route";
   
/*
        public void Init(){
            gymMap = new Dictionary<string, int>();
            gyms = new List<BNGym>();
            gymIDs = new BNGymIDs();
            ReadGyms();
        }

        public void ReadGyms(){ 
            string path = ES3_ROOTPATH +"/"+ ES3_FILE_BNGYMIDS;
            if (ES3.KeyExists(ES3_KEY_BNGYMIDS, path)){
                gymIDs = ES3.Load<BNGymIDs>(ES3_KEY_BNGYMIDS, path);
                int n = gymIDs.idList.Count;
                for(int i = 0 ; i < n ; i++){
                    path = ES3_ROOTPATH + "/" + gymIDs.idList[i] + ES3_EXTENSION;
    
                    if (ES3.KeyExists(ES3_KEY_GYM, path)){
                        gyms.Add(ES3.Load<BNGym>(ES3_KEY_GYM, path));
                        gymMap.Add(gymIDs.idList[i], i);
                    }
                }
            }
        }*/

        public List<string> ReadGymIDs(){
            List<string> ids;
            string path = ES3_ROOTPATH + "/" + ES3_FILE_BNGYMIDS + ES3_EXTENSION;
            if (ES3.KeyExists(ES3_KEY_BNGYMIDS, path)){
                ids = ES3.Load<List<string>>(ES3_KEY_BNGYMIDS, path);
            }else{
                ids = new List<string>();
            }     

            return ids;       
        }
        public BNGym ReadGym(string id){
            string path = ES3_ROOTPATH + "/" + id + "/" + ES3_FILE_GYM + ES3_EXTENSION;
            if (ES3.KeyExists(ES3_KEY_GYM, path)){
                return ES3.Load<BNGym>(ES3_KEY_GYM, path);
            }

            return null;
        }

        public BNWall ReadWall(string wallID, string gymID){
            string path = ES3_ROOTPATH + "/" + gymID + "/" + wallID + "/" + ES3_FILE_WALL + ES3_EXTENSION;
            if (ES3.KeyExists(ES3_KEY_WALL, path)){
                return ES3.Load<BNWall>(ES3_KEY_WALL, path);
            }

            return null;            
        }
        public BNRoute ReadRoute(string routeID, string wallID, string gymID){
            string path = ES3_ROOTPATH + "/" + gymID + "/" + wallID + "/" + routeID + "/" + ES3_FILE_ROUTE + ES3_EXTENSION;
            if (ES3.KeyExists(ES3_KEY_ROUTE, path)){
                return ES3.Load<BNRoute>(ES3_KEY_ROUTE, path);
            }

            return null;            
        }
        public IReadOnlyList<BNGym> ReadGyms(){
            List<BNGym> gyms = new List<BNGym>();
            List<string> gymIDs;
            //ids読み込み
            gymIDs = ReadGymIDs();

            foreach(string id in gymIDs){
                BNGym gym = ReadGym(id);
                if (gym != null){
                    gyms.Add(gym);
                }
            }
            return gyms;
        }

        public IReadOnlyList<BNWall> ReadWalls(BNGym gym){
            if (gym == null){
                return new List<BNWall>();
            }
            List<BNWall> walls = new List<BNWall>();
            List<string> wallIDs = gym.GetWallIDs();

            foreach(string id in wallIDs){
                BNWall wall = ReadWall(id, gym.GetID());
                if (wall != null){
                    walls.Add(wall);
                }
            }
            return walls;
        }

        public IReadOnlyList<BNRoute> ReadRoutes(BNWall wall, string gymID){
            if (wall == null || string.IsNullOrEmpty(gymID)){
                return new List<BNRoute>();
            }
            List<BNRoute> routes = new List<BNRoute>();
            List<string> routeIDs = wall.GetRouteIDs();

            foreach(string id in routeIDs){
                BNRoute route = ReadRoute(id, wall.GetID(), gymID);
                if (route != null){
                    routes.Add(route);
                }
            }
            return routes;
        }

        private void WriteGymIDs(List<string> ids){
            string path = ES3_ROOTPATH + "/" + ES3_FILE_BNGYMIDS + ES3_EXTENSION;
            ES3.Save<List<string>>(ES3_KEY_BNGYMIDS, ids, path);            
        }

        public void WriteGym(BNGym gym){
            if (gym == null){
                return ;
            }
            string id = gym.GetID();
            if (String.IsNullOrEmpty(id)){
                return ;
            }

            //ES3読み込み
            List<string> ids = ReadGymIDs();

            //既にgymIDが存在する場合、作らない
            if (ids.Contains(id)){
                return ;
            }
            ids.Add(id);
            //ES3に書き込み
            //gymID
            WriteGymIDs(ids);
            //gym
            string path = ES3_ROOTPATH + "/" + id + "/" + ES3_FILE_GYM + ES3_EXTENSION;
            ES3.Save<BNGym>(ES3_KEY_GYM, gym, path);
        }

        public void WriteWall(BNWall wall, BNGym gym){
            if (gym == null || wall == null || string.IsNullOrEmpty(wall.GetID())){
                return ;
            }

            //既にwallIDが存在する場合、作らない
            if (gym.GetWallIDs().Contains(wall.GetID())){
                return ;
            }
            gym.AddWallID(wall.GetID());
            ModifyGym(gym);

            string path = ES3_ROOTPATH + "/" + gym.GetID() + "/" + wall.GetID() + "/" + ES3_FILE_WALL + ES3_EXTENSION;
            ES3.Save<BNWall>(ES3_KEY_WALL, wall, path);              
        }

        public void WriteWall(BNWall wall, string gymID){
            WriteWall(wall, ReadGym(gymID));          
        }

        public void WriteRoute(BNRoute route, BNWall wall, BNGym gym){
            if (gym == null || wall == null || route == null || string.IsNullOrEmpty(route.GetID())){
                return ;
            }

            //既にrouteIDが存在する場合、作らない
            if (wall.GetRouteIDs().Contains(route.GetID())){
                return ;
            }
            wall.AddRouteID(route.GetID());
            ModifyWall(wall, gym.GetID());

            string path = ES3_ROOTPATH + "/" + gym.GetID() + "/" + wall.GetID() + "/" + route.GetID() + "/" + ES3_FILE_ROUTE + ES3_EXTENSION;
            ES3.Save<BNRoute>(ES3_KEY_ROUTE, route, path);                
        }
        public void WriteRoute(BNRoute route, string wallID, string gymID){
            WriteRoute(route, ReadWall(wallID, gymID), ReadGym(gymID));            
        }

        public void ModifyGym(BNGym gym){
            string id = gym.GetID();
            if (String.IsNullOrEmpty(id)){
                return ;
            }

            //ES3読み込み
            List<string> ids = ReadGymIDs();
            BNGym oldGym = ReadGym(id);

            //oldGymが存在しない場合、作らない
            if (oldGym == null){
                return ;
            }

            //画像等の参照フィールドが変更された場合、参照先を削除する
  
            //ES3に上書き
            //gym
            string path = ES3_ROOTPATH + "/" + id + "/" + ES3_FILE_GYM + ES3_EXTENSION;
            ES3.Save<BNGym>(ES3_KEY_GYM, gym, path);
        }
        public void ModifyWall(BNWall wall, BNGym gym){
            if (gym == null || wall == null || string.IsNullOrEmpty(wall.GetID())){
                return ;
            }

            //ES3読み込み
            BNWall oldWall = ReadWall(wall.GetID(), gym.GetID());

            //oldWallが存在しない場合、作らない
            if (oldWall == null){
                return ;
            }

            //画像等の参照フィールドが変更された場合、参照先を削除する
  
            //ES3に上書き
            //wall
            string path = ES3_ROOTPATH + "/" + gym.GetID() + "/" + wall.GetID() + "/" + ES3_FILE_WALL + ES3_EXTENSION;
            ES3.Save<BNWall>(ES3_KEY_WALL, wall, path);   
        }
        public void ModifyWall(BNWall wall, string gymID){
            ModifyWall(wall, ReadGym(gymID)); 
        }

        public void ModifyRoute(BNRoute route, BNWall wall, BNGym gym){
            if (gym == null || wall == null || route == null || string.IsNullOrEmpty(route.GetID())){
                return ;
            }

            //ES3読み込み
            BNRoute oldRoute = ReadRoute(route.GetID(), wall.GetID(), gym.GetID());

            //oldRouteが存在しない場合、作らない
            if (oldRoute == null){
                return ;
            }

            //画像等の参照フィールドが変更された場合、参照先を削除する
  
            //ES3に上書き
            //route
            string path = ES3_ROOTPATH + "/" + gym.GetID() + "/" + wall.GetID() + "/" + route.GetID() + "/" + ES3_FILE_ROUTE + ES3_EXTENSION;
            ES3.Save<BNRoute>(ES3_KEY_ROUTE, route, path);    
        }
        public void ModifyRoute(BNRoute route, string wallID, string gymID){
            ModifyRoute(route, ReadWall(wallID, gymID), ReadGym(gymID));    
        }

        public void DeleteGym(string gymID){
            string path = ES3_ROOTPATH + "/" + gymID;
            ES3.DeleteDirectory(path);
        }

        public void DeleteWall(string wallID, string gymID){
            string path = ES3_ROOTPATH + "/" + gymID + "/" + wallID;
            ES3.DeleteDirectory(path);
        }

        public void DeleteRoute(string routeID, string wallID, string gymID){
            string path = ES3_ROOTPATH + "/" + gymID + "/" + wallID + "/" + routeID;
            ES3.DeleteDirectory(path);
        }


        public void TestForSettingGym(){
            BNGym gym = new BNGym();
            gym.SetGymName("Noborock");
            WriteGym(gym);
            
            BNWall wall = new BNWall();
            wall.SetWallType(WallTypeMap.Type.Slab);
            wall.SetStart(DateTime.Now.ToString(FORMAT_DATE));
            wall.SetIsFinished(true);
            WriteWall(wall, gym.GetID());

            wall = new BNWall();
            wall.SetWallType(WallTypeMap.Type.Vertical);
            wall.SetStart(DateTime.Now.ToString(FORMAT_DATE));
            wall.SetIsFinished(true);
            WriteWall(wall, gym.GetID());

            wall = new BNWall();
            wall.SetWallType(WallTypeMap.Type.HOverHang);
            wall.SetStart(DateTime.Now.ToString(FORMAT_DATE));
            wall.SetIsFinished(false);
            WriteWall(wall, gym.GetID());

            wall = new BNWall();
            wall.SetWallType(WallTypeMap.Type.Bulge);
            wall.SetStart(DateTime.Now.ToString(FORMAT_DATE));
            wall.SetIsFinished(false);
            WriteWall(wall, gym.GetID());             

            BNRoute route = new BNRoute();
            route.SetGrade(BNGradeMap.Grade.Q3);
            route.SetStart(DateTime.Now.ToString("yyyy/MM/dd"));
            WriteRoute(route, wall.GetID(), gym.GetID());
        }
    }
}
