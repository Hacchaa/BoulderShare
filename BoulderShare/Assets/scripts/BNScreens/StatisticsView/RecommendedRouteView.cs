using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.UI;
using System.Linq;

namespace BoulderNotes{
public class RecommendedRouteView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI clearRateText;
    [SerializeField] private TextMeshProUGUI gymNameText;
    [SerializeField] private TextMeshProUGUI wallTypeText;
    [SerializeField] private TextMeshProUGUI gradeText;
    [SerializeField] private TextMeshProUGUI kanteText;
    [SerializeField] private RouteTape tape;
    [SerializeField] private Image wallImage;
    [SerializeField] private Sprite defaultSprite;
    public void SetData(BNTriple triple){
        clearRateText.text = "達成度" + triple.route.GetTotalClearRate() + "%";
        gymNameText.text = triple.gym.GetGymName();
        wallTypeText.text = triple.route.GetWallTypeName();
        gradeText.text = triple.route.GetGradeName();

        if (triple.route.IsUsedKante()){
            kanteText.text = "あり";
        }else{
            kanteText.text = "なし";
        }

        tape.LoadTape(triple.route.GetTape());
        IReadOnlyList<string> list = triple.wall.GetWallImageFileNames();
        if (list.Any()){
            string path = BNGymDataCenter.Instance.GetWallImagePath(triple.gym);
            LoadImage(path+list[0]);
        }else{
            wallImage.sprite = defaultSprite;
        }
    }
    private void LoadImage(string path){
        Texture2D texture = BNGymDataCenter.Instance.LoadWallImage(path);
        Sprite spr = Sprite.Create(
            texture, 
            new Rect(0.0f, 0.0f, texture.width, texture.height), 
            new Vector2(0.5f, 0.5f),
            texture.height/4);

        wallImage.sprite = spr;
        FitImage(spr);
    }   
    private void FitImage(Sprite spr){
        RectTransform rect = wallImage.GetComponent<RectTransform>();
        RectTransform parent = wallImage.transform.parent.GetComponent<RectTransform>();
        float fitHeight = parent.rect.height;
        float fitWidth = parent.rect.width;

        float texWidth = spr.texture.width;
        float texHeight = spr.texture.height;

        float difW = Mathf.Abs(fitWidth - texWidth);
        float difH = Mathf.Abs(fitHeight - texHeight);
        
        float w, h;
        //texのwidthかheightがfitTargetのwidhtかheightより大きいか小さいか
        if (fitWidth > texWidth || fitHeight > texHeight){
            //小さい場合、textureのwidthかheightどちらかを拡大する
            //差の大きいほうをfitTargetに合わせて拡大
            if (difW < difH){
                h = fitHeight;
                w = texWidth * (fitHeight / texHeight); 
            }else{
                w = fitWidth;
                h = texHeight * (fitWidth / texWidth);
            }
        }else{
            //大きい場合、textureのwidthかheightどちらかを縮小する
            //差の小さいほうをfitTargetに合わせて縮小
            if (difW < difH){
                w = fitWidth;
                h = texHeight * (fitWidth / texWidth);
            }else{
                h = fitHeight;
                w = texWidth * (fitHeight / texHeight);                 
            }
        }   
        //Debug.Log("w="+w+" h="+h);
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(w, h);
    }
/*
    private async void LoadImageAsync(string path){
        Texture2D texture = await Task<Texture2D>.Run(()=>BNGymDataCenter.Instance.LoadWallImage(path));
        Sprite spr = Sprite.Create(
            texture, 
            new Rect(0.0f, 0.0f, texture.width, texture.height), 
            new Vector2(0.5f, 0.5f),
            texture.height/4);

        wallImage.sprite = spr;
    }*/
}
}