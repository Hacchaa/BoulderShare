using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//##課題の読み書きを行うクラス
//##各画面は課題の更新のためにこのクラスにアクセスする
public class BSRoute : SingletonMonoBehaviour<BSRoute>
{
    private MyUtility.BSWall wall;
    private MyUtility.AttemptRecords attemptRecords;
    private string timestamp;

    public void LoadRoute(string ts){
        wall = new MyUtility.BSWall();
        wall.timestamp = ts;
        attemptRecords = new MyUtility.AttemptRecords();
        attemptRecords.timestamp = ts;
        attemptRecords.records = new MyUtility.AttemptRecord[0];
    }

    public void SaveRoute(string ts){
        MyUtility.BSWall w = new MyUtility.BSWall();

        w.timestamp = ts;
        w.date = ts;
        w.gymName = "Noborock";
        w.title = "ピーナッツ";
        w.wallType = (int)MyUtility.WallType.Slab;
        w.grade = (int)MyUtility.Grade.Q5;
        w.usesKante = false;

        ES3.Save<MyUtility.BSWall>("firstSave", w);
    }

    public MyUtility.BSWall GetWall(){
        MyUtility.BSWall w = new MyUtility.BSWall();
        w = wall;

        return w;
    }

    public void SetWall(MyUtility.BSWall w){
        wall = w;
    }

    public MyUtility.AttemptRecords GetAttemptRecords(){
        MyUtility.AttemptRecords rec = new MyUtility.AttemptRecords();

        rec.timestamp = timestamp;
        rec.records = new MyUtility.AttemptRecord[attemptRecords.records.Length];
        Array.Copy(attemptRecords.records, rec.records, attemptRecords.records.Length);

        return rec;
    }
    public void SetAttemptRecords(MyUtility.AttemptRecords rec){
        attemptRecords = rec;
    }
}
