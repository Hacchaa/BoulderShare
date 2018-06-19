using UnityEngine;
using System.Collections;
using System.IO;

namespace Kakera
{
    public class PickerController : MonoBehaviour
    {
        [SerializeField]
        private Unimgpicker imagePicker;

        [SerializeField]
        private SpriteRenderer imageRenderer;

        private Observer observer;
        void Awake()
        {
            imagePicker.Completed += (string path) =>
            {
                StartCoroutine(LoadImage(path, imageRenderer));
            };

            observer = GameObject.Find("Observer").GetComponent<Observer>();
        }

        public void OnPressShowPicker()
        {
            imagePicker.Show("Select Image", "unimgpicker", 1024);
        }

        private IEnumerator LoadImage(string path, SpriteRenderer output)
        {
            var url = "file://" + path;
            var www = new WWW(url);
            yield return www;

            var texture = www.texture;
            if (texture == null)
            {
                Debug.LogError("Failed to load texture url:" + url);
            }

            output.sprite = Sprite.Create(
                texture, 
                new Rect(0.0f, 0.0f, texture.width, texture.height), 
                new Vector2(0.5f, 0.5f),
                texture.height/4);

            byte[] pngData = texture.EncodeToPNG();
            string filePath = Application.persistentDataPath + "/Wall.png";
            //Debug.Log("copy texture at "+ filePath);
            File.WriteAllBytes(filePath, pngData);

            observer.InitHoldsAndScenes();
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