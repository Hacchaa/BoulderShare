using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Threading.Tasks;

//##ジムの読み書きを行うクラス
//##各画面はジムの更新のためにこのクラスにアクセスする

namespace BoulderNotes{
    public delegate void LoadImageDelegate(Sprite spr);
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
        public const string PREFIX_ID_WALLIMAGE = "WI";
        private const string ES3_ROOTPATH = "Gyms";
        private const string ES3_FILE_BNGYMIDS = "gymIDs";
        private const string ES3_KEY_BNGYMIDS = "BNGymIDs";
        private const string ES3_KEY_GYM = "BNGym";
        private const string ES3_KEY_WALL = "BNWall";
        private const string ES3_KEY_ROUTE = "BNRoute";
        private const string ES3_EXTENSION = ".es3";
        public const string WALLIMAGE_EXTENSION = ".png";
        private const string ES3_FILE_GYM = "gym";
        private const string ES3_FILE_WALL = "wall";
        private const string ES3_FILE_ROUTE = "route";
        private const string ES3_DIC_WALLIMAGE = "wallimages";
   
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

        private bool WriteGymIDs(List<string> ids){
            string path = ES3_ROOTPATH + "/" + ES3_FILE_BNGYMIDS + ES3_EXTENSION;
            ES3.Save<List<string>>(ES3_KEY_BNGYMIDS, ids, path); 
            return true;           
        }

        public bool WriteGym(BNGym gym){
            if (gym == null){
                return false;
            }
            string id = gym.GetID();
            if (String.IsNullOrEmpty(id)){
                return false;
            }

            //ES3読み込み
            List<string> ids = ReadGymIDs();

            //既にgymIDが存在する場合、作らない
            if (ids.Contains(id)){
                return false;
            }
            ids.Add(id);
            //ES3に書き込み
            //gymID
            WriteGymIDs(ids);
            //gym
            string path = ES3_ROOTPATH + "/" + id + "/" + ES3_FILE_GYM + ES3_EXTENSION;
            ES3.Save<BNGym>(ES3_KEY_GYM, gym, path);
            return true;
        }

        public bool WriteWall(BNWall wall, List<BNWallImage> wallImageList, BNGym gym){
            if (gym == null || wall == null || string.IsNullOrEmpty(wall.GetID())){
                return false;
            }

            //既にwallIDが存在する場合、作らない
            if (gym.GetWallIDs().Contains(wall.GetID())){
                return false;
            }
            gym.AddWallID(wall.GetID());
            ModifyGym(gym);

            string path = ES3_ROOTPATH + "/" + gym.GetID() + "/" + wall.GetID() + "/" + ES3_FILE_WALL + ES3_EXTENSION;
            ES3.Save<BNWall>(ES3_KEY_WALL, wall, path);  
            
            if (wallImageList == null){
                return true;
            }

            //画像を保存
            path = ES3_ROOTPATH + "/" + gym.GetID() + "/" + wall.GetID() + "/" + ES3_DIC_WALLIMAGE + "/";
            foreach(BNWallImage wallImage in wallImageList){
                if (string.IsNullOrEmpty(wallImage.fileName) || wallImage.texture == null){
                    continue;
                }
                ES3.SaveImage(wallImage.texture, path+wallImage.fileName);
            }
            return true;            
        }

        public bool WriteWall(BNWall wall, List<BNWallImage> wallImageList, string gymID){
            return WriteWall(wall, wallImageList, ReadGym(gymID));          
        }

        public bool WriteRoute(BNRoute route, BNWall wall, BNGym gym){
            if (gym == null || wall == null || route == null || string.IsNullOrEmpty(route.GetID())){
                return false;
            }

            //既にrouteIDが存在する場合、作らない
            if (wall.GetRouteIDs().Contains(route.GetID())){
                return false;
            }
            wall.AddRouteID(route.GetID());
            ModifyWall(wall, null, null, gym.GetID());

            string path = ES3_ROOTPATH + "/" + gym.GetID() + "/" + wall.GetID() + "/" + route.GetID() + "/" + ES3_FILE_ROUTE + ES3_EXTENSION;
            ES3.Save<BNRoute>(ES3_KEY_ROUTE, route, path);       

            return true;         
        }
        public bool WriteRoute(BNRoute route, string wallID, string gymID){
            return WriteRoute(route, ReadWall(wallID, gymID), ReadGym(gymID));            
        }

        public bool ModifyGym(BNGym gym){
            string id = gym.GetID();
            if (String.IsNullOrEmpty(id)){
                return false;
            }

            //ES3読み込み
            List<string> ids = ReadGymIDs();
            BNGym oldGym = ReadGym(id);

            //oldGymが存在しない場合、作らない
            if (oldGym == null){
                return false;
            }

            //画像等の参照フィールドが変更された場合、参照先を削除する
  
            //ES3に上書き
            //gym
            string path = ES3_ROOTPATH + "/" + id + "/" + ES3_FILE_GYM + ES3_EXTENSION;
            ES3.Save<BNGym>(ES3_KEY_GYM, gym, path);
            return true;
        }
        public bool ModifyWall(BNWall wall, List<BNWallImage> addWallImageList, List<string> removeWallImageList, BNGym gym){
            if (gym == null || wall == null || string.IsNullOrEmpty(wall.GetID())){
                return false;
            }

            //ES3読み込み
            BNWall oldWall = ReadWall(wall.GetID(), gym.GetID());

            //oldWallが存在しない場合、作らない
            if (oldWall == null){
                return false;
            }

            //画像等の参照フィールドが変更された場合
            string path = ES3_ROOTPATH + "/" + gym.GetID() + "/" + wall.GetID() + "/" + ES3_DIC_WALLIMAGE + "/";
            List<string> list = wall.GetWallImageFileNames();
            if (addWallImageList != null){
                foreach(BNWallImage wallImage in addWallImageList){
                    if (!string.IsNullOrEmpty(wallImage.fileName)){
                        list.Add(wallImage.fileName);
                        ES3.SaveImage(wallImage.texture, path+wallImage.fileName);
                    }
                }
            }

            if (removeWallImageList != null){
                foreach(string str in removeWallImageList){
                    list.Remove(str);
                    ES3.DeleteFile(path+str);
                }
            }

            wall.SetWallImageFileNames(list);
  
            //ES3に上書き
            //wall
            path = ES3_ROOTPATH + "/" + gym.GetID() + "/" + wall.GetID() + "/" + ES3_FILE_WALL + ES3_EXTENSION;
            ES3.Save<BNWall>(ES3_KEY_WALL, wall, path);   
            return true;
        }
        public bool ModifyWall(BNWall wall, List<BNWallImage> addWallImageList, List<string> removeWallImageList, string gymID){
            return ModifyWall(wall, addWallImageList, removeWallImageList, ReadGym(gymID)); 
        }

        public bool ModifyRoute(BNRoute route, BNWall wall, BNGym gym){
            if (gym == null || wall == null || route == null || string.IsNullOrEmpty(route.GetID())){
                return false;
            }

            //ES3読み込み
            BNRoute oldRoute = ReadRoute(route.GetID(), wall.GetID(), gym.GetID());

            //oldRouteが存在しない場合、作らない
            if (oldRoute == null){
                return false;
            }

            //画像等の参照フィールドが変更された場合、参照先を削除する
  
            //ES3に上書き
            //route
            string path = ES3_ROOTPATH + "/" + gym.GetID() + "/" + wall.GetID() + "/" + route.GetID() + "/" + ES3_FILE_ROUTE + ES3_EXTENSION;
            ES3.Save<BNRoute>(ES3_KEY_ROUTE, route, path);    

            return true;
        }
        public bool ModifyRoute(BNRoute route, string wallID, string gymID){
            return ModifyRoute(route, ReadWall(wallID, gymID), ReadGym(gymID));    
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

        public Texture2D LoadWallImage(BNGym gym, BNWall wall, string fileName){
            string path = ES3_ROOTPATH + "/" + gym.GetID() + "/" + wall.GetID() + "/" + ES3_DIC_WALLIMAGE + "/";
            return ES3.LoadImage(path+fileName);
        }

        public string GetWallImagePath(BNGym gym, BNWall wall){
            return Application.persistentDataPath + "/" + ES3_ROOTPATH + "/" + gym.GetID() + "/" + wall.GetID() + "/" + ES3_DIC_WALLIMAGE;
        }

        public IEnumerator LoadImage(string path, LoadImageDelegate del)
        {
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(path))
            {
                yield return uwr.SendWebRequest();

                if (uwr.isNetworkError || uwr.isHttpError)
                {
                    Debug.Log(uwr.error);
                    Debug.Log("path" + path);
                }
                else
                {
                    // Get downloaded asset bundle
                    var texture = DownloadHandlerTexture.GetContent(uwr);
                    //Debug.Log("texture:"+texture.width);
                    Sprite sprite = Sprite.Create(
                        texture, 
                        new Rect(0.0f, 0.0f, texture.width, texture.height), 
                        new Vector2(0.5f, 0.5f),
                        texture.height/4);
                    if (del != null){
                        del(sprite);
                    }
                }
            }
        }
        public void TestForSettingGym(){
            BNGym gym = new BNGym();
            gym.SetGymName("Noborock");
            WriteGym(gym);
            
            BNWall wall = new BNWall();
            wall.SetWallType(WallTypeMap.Type.Slab);
            wall.SetStart(DateTime.Now);
            wall.SetIsFinished(true);
            WriteWall(wall, null, gym.GetID());

            wall = new BNWall();
            wall.SetWallType(WallTypeMap.Type.Vertical);
            wall.SetStart(DateTime.Now);
            wall.SetIsFinished(true);
            WriteWall(wall, null, gym.GetID());

            wall = new BNWall();
            wall.SetWallType(WallTypeMap.Type.HOverHang);
            wall.SetStart(DateTime.Now);
            wall.SetIsFinished(false);
            WriteWall(wall, null, gym.GetID());

            wall = new BNWall();
            wall.SetWallType(WallTypeMap.Type.Bulge);
            wall.SetStart(DateTime.Now);
            wall.SetIsFinished(false);
            WriteWall(wall, null, gym.GetID());             

            BNRoute route = new BNRoute();
            route.SetGrade(BNGradeMap.Grade.Q3);
            route.SetStart(DateTime.Now);
            WriteRoute(route, wall.GetID(), gym.GetID());
        }
    }
}
