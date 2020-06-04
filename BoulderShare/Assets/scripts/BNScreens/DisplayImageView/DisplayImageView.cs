using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

namespace BoulderNotes{
public class DisplayImageView: BNScreen
{
    [SerializeField] private MoveImageController controller;
    private BNWallImageNames imageNames;
    private bool showedEditedImage;
    private BNScreenStackWithTargetGym stack;
    [SerializeField] private Sprite[] wallChangeSprites;
    [SerializeField] private Image wallChangeIcon;
    public override void InitForFirstTransition(){
        stack = belongingStack as BNScreenStackWithTargetGym;
        if (stack == null){
            return ;
        }
        
        imageNames = stack.GetTargetImageNames();
        if (imageNames != null){
            if (!string.IsNullOrEmpty(imageNames.editedFileName)){
                stack.LoadImageAsync(imageNames.editedFileName, OnLoad);
                showedEditedImage = true;
                wallChangeIcon.sprite = wallChangeSprites[1];
            }else if (!string.IsNullOrEmpty(imageNames.fileName)){
                stack.LoadImageAsync(imageNames.fileName, OnLoad);
                showedEditedImage = false;
                wallChangeIcon.sprite = wallChangeSprites[0];
            }
        }
/*
        if (TemporaryRepository_BNScreens.Instance.bNWallImageNames != null){
            imageNames = TemporaryRepository_BNScreens.Instance.bNWallImageNames;

            if (!string.IsNullOrEmpty(imageNames.editedFileName)){
                showedEditedImage = true;

            }
            controller.Init();
        }*/
    }

    public void ChangeWallImage(){
        if (stack == null || imageNames == null){
            return ;
        }
        if (!showedEditedImage && !string.IsNullOrEmpty(imageNames.editedFileName)){
            stack.LoadImageAsync(imageNames.editedFileName, OnLoad);
            showedEditedImage = true;
            wallChangeIcon.sprite = wallChangeSprites[1];
        }else if(showedEditedImage && !string.IsNullOrEmpty(imageNames.fileName)){
            stack.LoadImageAsync(imageNames.fileName, OnLoad);
            showedEditedImage = false;
            wallChangeIcon.sprite = wallChangeSprites[0];
        }
    }

    private void OnLoad(Sprite sprite){
        controller.Init(sprite);
    }

    public override void UpdateScreen(){

    }    
    public void ReverseTransition(){
        BNScreens.Instance.ReverseTransition();
    }
}
}