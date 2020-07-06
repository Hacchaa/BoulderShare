using System.Collections;
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
    private bool isDefault;

    public bool IsDefault(){
        return isDefault;
    }
    public void LoadDefault(){
        //Debug.Log("first:"+firstSprite.ToString());
        Addressables.LoadAssetsAsync<Sprite>(firstSprite, OnLoad);
        text.text = "";
        img.color = firstColor;
        img.transform.localRotation = Quaternion.identity;
        isDefault = true;
    }
    public void ChangeShape(Sprite sprite){
        img.sprite = sprite;
        img.transform.localRotation = Quaternion.identity;
        isDefault = false;
    }

    public void ChangeColor(Color c){
        img.color = c;
        isDefault = false;
    }

    public void ChangeText(string txt){
        text.text = txt;
        isDefault = false;
    }

    public void AddRot(float angle){
        img.transform.localRotation *= Quaternion.Euler(0f, 0f, angle);
    }

    public void LoadTape(RTape t){
        if (t == null){
            return ;
        }
        if (t.isDefault){
            LoadDefault();
            return ;
        }
        if (!string.IsNullOrEmpty(t.spriteName)){
            //Debug.Log("t.spriteName:"+t.spriteName);
            Addressables.LoadAssetsAsync<Sprite>(t.spriteName, OnLoad);
        }
        text.text = t.tapeText;
        img.color = t.color;
        img.transform.localRotation = t.imageRot;
        isDefault = false;
    }

    public RTape GetTape(){
        RTape tape = new RTape();
        tape.spriteName = img.sprite.name;
        tape.tapeText = text.text;
        tape.color = img.color;
        tape.imageRot = img.transform.localRotation;
        tape.isDefault = isDefault;
        return tape;
    }

    private void OnLoad(Sprite sprite){
        img.sprite = sprite;
    }
}
}