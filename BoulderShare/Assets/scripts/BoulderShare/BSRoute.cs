using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//##課題の読み書きを行うクラス
//##各画面は課題の更新のためにこのクラスにアクセスする
public class BSRoute : SingletonMonoBehaviour<BSRoute>
{
    [SerializeField] private MyUtility.BSWall wall;
    private MyUtility.AttemptRecords attemptRecords;
    private string timestamp;
    [SerializeField] private Texture2D wallImage;
    [SerializeField] private Texture2D wallThumbnail;

/* 
    override protected void Awake(){
        base.Awake();
        Init();
    }*/
    public void Init(){
        wall = new MyUtility.BSWall();
        attemptRecords = new MyUtility.AttemptRecords();
        timestamp = "";
        wallImage = null;
        wallThumbnail = null;
    }

    private int CalcPNGWidth(byte[] png){
        int w = 0;
        int pos = 16;

        for(int i = 0 ; i < 4 ; i++){
            w = w * 256 + png[pos++];
        }

        return w;
    }

    private int CalcPNGHeight(byte[] png){
        int h = 0;
        int pos = 20;

        for(int i = 0 ; i < 4 ; i++){
            h = h * 256 + png[pos++];
        }

        return h;
    }

    public void LoadRoute(string ts){
        string file = MyUtility.E3ROUTEPATH+ts+"/"+MyUtility.E3WALLFILENAME;
        if (ES3.KeyExists(MyUtility.E3KEY_BSWALL, file)){
            wall = ES3.Load<MyUtility.BSWall>(MyUtility.E3KEY_BSWALL, file);
            
            file = MyUtility.E3ROUTEPATH+ts+"/"+MyUtility.E3PATH_WALLIMAGE;
            if (ES3.FileExists(file)){
                byte[] bin = ES3.LoadRawBytes(file);
                wallImage = new Texture2D(CalcPNGWidth(bin), CalcPNGHeight(bin));
                wallImage.LoadImage(bin);
            }else{
                Debug.Log("key of wallimage was not found");
            }

            file = MyUtility.E3ROUTEPATH+ts+"/"+MyUtility.E3PATH_WALLTHUMBNAIL;
            if (ES3.FileExists(file)){
                byte[] bin = ES3.LoadRawBytes(file);
                wallThumbnail = new Texture2D(CalcPNGWidth(bin), CalcPNGHeight(bin));
                wallThumbnail.LoadImage(bin);
            }else{
                Debug.Log("key of thumbnail was not found");
            }
        }else{
            Debug.Log("key of bswall was not found");
        }
    }

    public void SaveRoute(string ts){
        MyUtility.BSWall w = new MyUtility.BSWall();

        w.timestamp = ts;
        w.date = ts;
        w.gymName = "Noborock";
        w.title = "ピーナッツ";
        w.wallType = MyUtility.WallType.Slab;
        w.grade = MyUtility.Grade.Q5;
        w.usesKante = false;

        if (wallImage != null){
            ES3.SaveRaw(wallImage.EncodeToPNG(), MyUtility.E3ROUTEPATH+ts+"/"+MyUtility.E3PATH_WALLIMAGE);
        }
        if (wallThumbnail != null){
            ES3.SaveRaw(wallThumbnail.EncodeToPNG(), MyUtility.E3ROUTEPATH+ts+"/"+MyUtility.E3PATH_WALLTHUMBNAIL);
        }

        ES3.Save<MyUtility.BSWall>(MyUtility.E3KEY_BSWALL, w, MyUtility.E3ROUTEPATH+ts+"/"+MyUtility.E3WALLFILENAME);
    }

    public MyUtility.BSWall GetWall(){
        MyUtility.BSWall w = new MyUtility.BSWall();
        w = wall;

        return w;
    }

    public void SetWall(MyUtility.BSWall w){
        if (String.IsNullOrEmpty(w.timestamp)){
            return ;
        }
        wall = w;
        timestamp = w.timestamp;
    }

    public MyUtility.AttemptRecords GetAttemptRecords(){
        MyUtility.AttemptRecords rec = new MyUtility.AttemptRecords();

        rec.timestamp = timestamp;
        rec.records = new MyUtility.AttemptRecord[attemptRecords.records.Length];
        Array.Copy(attemptRecords.records, rec.records, attemptRecords.records.Length);

        return rec;
    }
    public void SetAttemptRecords(MyUtility.AttemptRecords rec){
        if (String.IsNullOrEmpty(attemptRecords.timestamp)){
            return;
        }
        attemptRecords = rec;
        timestamp = attemptRecords.timestamp;
    }

    public Texture2D GetWallImageTexture(){
        return wallImage;
    }

    public void SetWallImageTexture(Texture2D tex){
        wallImage = tex;
    }

    public Texture2D GetWallThmbnailTextrue(){
        return wallThumbnail;
    }

    public void SetWallThmbnailTexture(Texture2D tex){
        wallThumbnail = tex;
    }
}
