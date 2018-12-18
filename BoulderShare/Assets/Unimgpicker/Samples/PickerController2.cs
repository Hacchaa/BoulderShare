﻿using UnityEngine;
using System.Collections;
using System.IO;

namespace Kakera
{
    public class PickerController2 : MonoBehaviour
    {
        [SerializeField]
        private Unimgpicker imagePicker;
        [SerializeField]
        private TwoDWall twoDWall;
        [SerializeField]
        private EditorPopup popup;
        [SerializeField]
        private EditorManager eManager;
        [SerializeField]
        private ScreenTransitionManager trans;
        private bool isLoading = false;

        void Awake()
        {
            imagePicker.Completed += (string path) =>
            {
                isLoading = true;
                StartCoroutine(LoadImage(path));
            };

/*
            //画像が読み込まれたことを知らせる
            imagePicker.Completed += (string path) =>
            {
                phase1.AfterLoadingImage();
            };*/

        }
        void Start(){
        }

        public void OpenImagePicker(){
            popup.Open(OnPressShowPicker, eManager.ExitImmediately,"はじめに画像を読み込みましょう。", eManager.ExitImmediately);
        }

        public void OnPressShowPicker()
        {
            imagePicker.Show("Select Image", "unimgpicker", 1024);
            //StartCoroutine(WaitForExit());
        }

        private IEnumerator WaitForExit(){
            float startTime = Time.time;
            while(!isLoading){
                if (Time.time - startTime < 1.0f){
                    yield return null;
                }else{
                    eManager.ExitImmediately();
                    isLoading = true;
                }
            }
        }

        private IEnumerator LoadImage(string path)
        {
            var url = "file://" + path;
            var www = new WWW(url);
            yield return www;

            var texture = www.texture;
            if (texture == null)
            {
                Debug.LogError("Failed to load texture url:" + url);
            }

            twoDWall.SetWallSprite(Sprite.Create(
                texture, 
                new Rect(0.0f, 0.0f, texture.width, texture.height), 
                new Vector2(0.5f, 0.5f),
                texture.height/4)
            );

            byte[] pngData = texture.EncodeToPNG();
            string filePath = Application.persistentDataPath + "/Wall.png";
            Debug.Log("copy texture at "+ filePath);
            File.WriteAllBytes(filePath, pngData);
            trans.Transition("AttemptTreeView");

/*
            byte[] values;
            using(FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read)){
                BinaryReader bin = new BinaryReader(fileStream);
                values = bin.ReadBytes((int)bin.BaseStream.Length);
                bin.Close();
            }
            texture = new Texture2D(1, 1);
            texture.LoadImage(values);

            output.sprite = Sprite.Create(
                texture, 
                new Rect(0.0f, 0.0f, texture.width, texture.height), 
                new Vector2(0.5f, 0.5f),
                texture.height/4);
                */
        }
    }
}