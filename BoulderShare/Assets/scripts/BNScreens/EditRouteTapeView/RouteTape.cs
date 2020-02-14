﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AddressableAssets;

namespace BoulderNotes{
public class RouteTape : MonoBehaviour
{
    [SerializeField] private Image img;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private AssetReference firstSprite;
    [SerializeField] private Color firstColor;

    public void LoadDefault(){
        Addressables.LoadAssetsAsync<Sprite>(firstSprite, OnLoad);
        text.text = "";
        img.color = firstColor;
        img.transform.localRotation = Quaternion.identity;
    }
    public void ChangeShape(Sprite sprite){
        img.sprite = sprite;
        img.transform.localRotation = Quaternion.identity;
    }

    public void ChangeColor(Color c){
        img.color = c;
    }

    public void ChangeText(string txt){
        text.text = txt;
    }

    public void AddRot(float angle){
        img.transform.localRotation *= Quaternion.Euler(0f, 0f, angle);
    }

    public void LoadTape(RTape t){
        if (t == null){
            return ;
        }

        if (!string.IsNullOrEmpty(t.spriteName)){
            Addressables.LoadAssetsAsync<Sprite>(t.spriteName, OnLoad);
        }
        text.text = t.tapeText;
        img.color = t.color;
        img.transform.localRotation = t.imageRot;
    }

    public RTape GetTape(){
        RTape tape = new RTape();
        tape.spriteName = img.sprite.name;
        tape.tapeText = text.text;
        tape.color = img.color;
        tape.imageRot = img.transform.localRotation;
        return tape;
    }

    private void OnLoad(Sprite sprite){
        ChangeShape(sprite);
    }
}
}