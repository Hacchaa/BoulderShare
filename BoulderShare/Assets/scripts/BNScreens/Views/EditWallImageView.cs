﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

namespace BoulderNotes{
public class EditWallImageView: BNScreen
{
    [SerializeField] private GameObject mobliePaintRoot;
    [SerializeField] private unitycoder_MobilePaint.MobilePaint mobilePaint;
    [SerializeField] private RectTransform drawArea;
    [SerializeField] private float offsetTop = 88f;
    [SerializeField] private float offsetWidth = 60f;

    [SerializeField] private Slider brushSlider;
    [SerializeField] private RectTransform brushPreview;
    [SerializeField] private GameObject mobilePaintUI;
    [SerializeField] private Camera mobilePaintCamera;
    [SerializeField] private float maskThreshould = 0.2f;
    [SerializeField] private Image testImage;
    [SerializeField] private EditWallImage_PenSizeController penSizeController;
    [SerializeField] private EditWallImage_PenTypeController penTypeController;
    [SerializeField] private EditWallImage_FillTypeController fillTypeController;
    [SerializeField] private EditWallImage_Undo undoController;
    private float ptPerTexsize;
    private float defaultFOV;
    private Renderer rend;
    private int texWidth;
    private int texHeight;
    private BNScreenStackWithTargetGym stack;
    private RouteView routeView;
    private BNRoute route;
    private BNWallImageNames wallImageNames;
    private Texture2D wallImage;
    
    public override void InitForFirstTransition(){
        stack = null;
        route = null;
        routeView = null;

        penSizeController.Init();
        penTypeController.Init();
        fillTypeController.Init();
        undoController.Init();
    
        if (belongingStack != null && belongingStack is BNScreenStackWithTargetGym){
            stack = belongingStack as BNScreenStackWithTargetGym;
            route = stack.GetTargetRoute();
            BNScreen s = stack.GetPreviousScreen(1);
            if (s is RouteView){
                routeView = s as RouteView;
                wallImage = routeView.GetSelectedWallImage();
                wallImageNames = routeView.GetSelectedWallImageName();
            }
        }
        if (wallImage != null){
            CalcDrawArea();
            mobliePaintRoot.SetActive(true);
            rend = mobilePaint.GetComponent<Renderer>();
            rend.material.SetTexture("_MainTex", wallImage);
            mobilePaint.overrideWidth = wallImage.width;
            mobilePaint.overrideHeight = wallImage.height;
            mobilePaint.userInterface = mobilePaintUI;
            mobilePaint.InitializeEverything();
            brushSlider.value = mobilePaint.brushSize;
            brushPreview.gameObject.SetActive(false);

            defaultFOV = mobilePaintCamera.fieldOfView;

            texWidth = wallImage.width;
            texHeight = wallImage.height;

            testImage.material = rend.material;
            testImage.enabled = false;
            testImage.enabled = true;
        }
    }

    public override void UpdateScreen(){

    }

    private void CalcDrawArea(){
        drawArea.anchorMin = Vector2.zero;
        drawArea.anchorMax = Vector2.one;
        drawArea.offsetMin = new Vector2(0f, 0f);
        drawArea.offsetMax = new Vector2(0f, 0f);

        float parentW = drawArea.rect.width - offsetWidth;
        float parentH = drawArea.rect.height - offsetTop;
        int texW = wallImage.width;
        int texH = wallImage.height;
        float drawW;
        float drawH;
        float r;
        if (parentH / (float)parentW <= texH / (float)texW){
            //heightを合わせる
            r = parentH / texH;
        }else{
            //widthを合わせる
            r = parentW / texW;
        }
        drawH = texH * r;
        drawW = texW * r;

        ptPerTexsize = drawW / texW;
        Debug.Log("parentH:"+parentH + " parentW:"+ parentW);
        Debug.Log("texH:"+texH + " texW:"+ texW);
        Debug.Log("drawH:"+drawH + " drawW:"+ drawW);
        drawArea.anchorMin = Vector2.zero;
        drawArea.anchorMax = Vector2.one;

        drawArea.offsetMin = new Vector2((parentW - drawW + offsetWidth)/2f, (parentH - drawH + offsetTop)/2f);
        drawArea.offsetMax = new Vector2(-(parentW - drawW + offsetWidth)/2f, -(parentH - drawH + offsetTop)/2f);
    }

    public void ReverseTransition(){
        BNScreens.Instance.ReverseTransition();
    }

    public void DoUndo(){
        if(mobilePaint != null){
            mobilePaint.DoUndo();
        }
    }

    public void OnBrushSliderValueChanged(float v){
        if (mobilePaint != null){
            mobilePaint.SetBrushSize((int)v);
            SetBrushSize(v);
        }
    }

    public void SetBrushSize(float v = -1f){
        if (v < 0){
            v = mobilePaint.brushSize;
        }
        //brushsizeは半径
        brushPreview.sizeDelta = Vector2.one * v * ptPerTexsize * 2f / ( mobilePaintCamera.fieldOfView / defaultFOV);
    }
    public void ShowBrush(){
        brushPreview.gameObject.SetActive(true);
    }


    public void HideBrush(){
        brushPreview.gameObject.SetActive(false);
    }

    public void SetBrushMode(){
        mobilePaint.SetDrawModeBrush();
    }
    public void SetFillMode(){
        mobilePaint.SetDrawModeFill();
    }

    public void SaveWallImage(){
        Color32[] maskColors = ((Texture2D)rend.material.GetTexture("_MaskTex")).GetPixels32();
        Color32[] mainColors = ((Texture2D)rend.material.GetTexture("_MainTex")).GetPixels32();
        byte[] bin = new byte[texWidth * texHeight * 4];
        Texture2D image = new Texture2D(texWidth, texHeight, TextureFormat.RGBA32, false);

        for(int i = 0 ; i < maskColors.Length ; i++){
            if (maskColors[i].a / 255f < maskThreshould){
                bin[i*4] = mainColors[i].r;
                bin[i*4+1] = mainColors[i].g;
                bin[i*4+2] = mainColors[i].b;
                bin[i*4+3] = mainColors[i].a;
            }else{
                byte a = maskColors[i].a;
                bin[i*4] = (byte)(Mathf.Round(Mathf.Lerp(mainColors[i].r, maskColors[i].r, a / 255f)));
                bin[i*4+1] = (byte)(Mathf.Round(Mathf.Lerp(mainColors[i].g, maskColors[i].g, a / 255f)));
                bin[i*4+2] = (byte)(Mathf.Round(Mathf.Lerp(mainColors[i].b, maskColors[i].b, a / 255f)));
                bin[i*4+3] = 255;
            }
        }

        image.LoadRawTextureData(bin);
        image.Apply(false);

        if (stack != null){
            string[] str = wallImageNames.fileName.Split('.');
            string name = str[0] + BNGymDataCenter.POSTFIX_ID_WALLIMAGEEDITED + "." + str[1];
            List<BNWallImage> wallImages = new List<BNWallImage>();
            wallImages.Add(new BNWallImage(image, name));
            wallImageNames.editedFileName = name;
            route.ModifyWallImageFileName(wallImageNames);

            stack.ModifyRoute(route, wallImages);
        }
        /*
        testImage.sprite = Sprite.Create(
            image, 
            new Rect(0.0f, 0.0f, image.width, image.height), 
            new Vector2(0.5f, 0.5f),
            image.height/4);

        bin = image.EncodeToPNG(); // this works also, but only returns drawing layer

        string filename = "screenshot.png";
        string folder = Application.dataPath + "/Screenshots/";
        string path = folder + filename;

        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        File.WriteAllBytes(path, bin);

        Debug.Log("Screenshot saved at: "+path);*/
    }


    private bool undoSwitch = false;
    public void Undo(){
        if (undoSwitch){
            undoController.ShowDisableIcon();
        }else{
            undoController.ShowEnableIcon();
        }
        undoSwitch = !undoSwitch;
    }

    private bool fillSwitch = false;

    public void ChangeFillType(){
        if (fillSwitch){
            fillTypeController.SwitchOff();
        }else{
            fillTypeController.SwitchOn();
        }
        fillSwitch = !fillSwitch;
    }
}
}