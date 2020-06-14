using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Linq;

namespace BoulderNotes{
public class RouteView : BNScreen
{
    [SerializeField] private RecordScrollerController scroller;
    [SerializeField] private BNRoute route;
    [SerializeField] private Image favoriteImage;
    [SerializeField] private Sprite favoriteOn;
    [SerializeField] private Sprite favoriteOff;

    private DateTime pushedTime;
    private bool isFavorite;
    [SerializeField] private List<Image> headBGImages;
    [SerializeField] private float headBGThreshold = 200f;

    [SerializeField] private Image wallImage;
    [SerializeField] private Sprite defaultSpr;
    [SerializeField] private float defaultWallImageHeight = 240f;
    
    [SerializeField] private RectTransform wallImageRect;
    [SerializeField] private RectTransform wallImageParent;
    [SerializeField] ScrollRect scrollRect;
    private RectTransform scrollRectRect;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private BNScreenStackWithTargetGym stack;
    private BNWallImageNames selectedWallImageName;
    private Texture2D selectedWallImage;

    [SerializeField] private GameObject editWallImageButton;

    public override void InitForFirstTransition(){
        scroller.Init();
        GradHeadBG(0f);

        if (belongingStack != null && belongingStack is BNScreenStackWithTargetGym){
            stack = belongingStack as BNScreenStackWithTargetGym;
        }
        wallImageRect = wallImage.GetComponent<RectTransform>();
        wallImageParent = wallImage.transform.parent.GetComponent<RectTransform>();
        scrollRectRect = scrollRect.GetComponent<RectTransform>();
    }

    public BNWallImageNames GetSelectedWallImageName(){
        return selectedWallImageName;
    }

    public Texture2D GetSelectedWallImage(){
        return selectedWallImage;
    }

    public override void UpdateScreen(){
        if (stack != null){
            //recordIDを削除
            stack.ClearRecord();
            route = stack.GetTargetRoute();
            if (route != null){

                if (route.IsFavorite()){
                    favoriteImage.sprite = favoriteOn;
                }else{
                    favoriteImage.sprite = favoriteOff;
                }
                isFavorite = route.IsFavorite();

                //Debug.Log("route.gettags().count "+route.GetTags().Count);
                //tag
                scroller.FetchData(route);
            }
    
            selectedWallImage = null;
            selectedWallImageName = null;

            List<BNWallImageNames> list = route.GetWallImageFileNames();
            if (list.Any()){
                wallImage.sprite = stack.LoadImageByES3(list[0].fileName);
                selectedWallImage = wallImage.sprite.texture;
                selectedWallImageName = list[0];
                editWallImageButton.SetActive(true);
            }else{
                wallImage.sprite = defaultSpr;
                editWallImageButton.SetActive(false);
            }

            FitWallImageToParent();
        }
    }

    public void SaveTargerRecordInStack(BNRecord rec){
        if (belongingStack != null && belongingStack is BNScreenStackWithTargetGym){
            (belongingStack as BNScreenStackWithTargetGym).StoreTargetRecord(rec.GetID());
        }
    }
    public void SaveTargetWallImageNamesInStack(BNWallImageNames names){
        if (belongingStack != null && belongingStack is BNScreenStackWithTargetGym){
            (belongingStack as BNScreenStackWithTargetGym).SetTargetImageNames(names);
        }        
    }
    public void ReverseTransition(){
        BNScreens.Instance.ReverseTransition();
    }
    public void ToRecordView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.RecordView, BNScreens.TransitionType.Push);
    }

    public void ToModifyView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.ModifyView, BNScreens.TransitionType.Push);
    }

    public void ToRegisterRecordView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.RegisterRecordView, BNScreens.TransitionType.Push);
    }
    public void ToSelectRouteTagView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.SelectRouteTagView, BNScreens.TransitionType.Push);
    }
    public void ToEditWallImageView(){
        BNScreens.Instance.Transition(BNScreens.BNScreenType.EditWallImageView, BNScreens.TransitionType.Push);
    }
    public void OnFavoriteClicked(){
        if (pushedTime == null){
            pushedTime = DateTime.Now;
            return ;
        }

        TimeSpan ts = DateTime.Now - pushedTime;

        if (ts.TotalSeconds >= 1.5){
            SwitchFavoriteImage();
            pushedTime = DateTime.Now;
        }
    }

    private void SwitchFavoriteImage(){
        if (isFavorite){
            favoriteImage.sprite = favoriteOff;
        }else{
            favoriteImage.sprite = favoriteOn;
        }

        isFavorite = !isFavorite;

        //routeを保存
        if (route != null){
            route.SetIsFavorite(isFavorite);
            if (belongingStack != null && belongingStack is BNScreenStackWithTargetGym){
                (belongingStack as BNScreenStackWithTargetGym).ModifyRoute(route);
            }
        }
    }

    public void GradHeadBG(float r){
        foreach(Image img in headBGImages){
            img.color = new Color(img.color.r, img.color.g, img.color.b, r);
        }
    }

    public void GradHeadBGFromScroller(Vector2 v){
        //Debug.Log("gradHead");
        if (!processedInit){
            return ;
        }
        //現在スクロールpt量を計算
        float cur = scrollRect.content.anchoredPosition.y;
        float r = cur / headBGThreshold;
        r = Mathf.Clamp(r, 0f, 1f);
        GradHeadBG(r);

        if (r == 1f){
            ShowHeaderContent();
        }else{
            HideHeaderContent();
        }
    }
    private void ShowHeaderContent(){
        /*
        if(belongingStack is BNScreenStackWithTargetGym){
            title.text = (belongingStack as BNScreenStackWithTargetGym).GetTargetGym().GetGymName();
            title.gameObject.SetActive(true);
        }*/
        title.text = route.GetGradeName();
        title.gameObject.SetActive(true);
        
    }
    private void HideHeaderContent(){
        title.gameObject.SetActive(false);
    }
 
    public void DebugScroll(Vector2 v){
        if (wallImageParent == null){
            return ;
        }
        //Debug.Log("v="+v.y);
        //Debug.Log("diffHeight:"+(scrollRect.content.rect.height - scrollRectRect.rect.height)*(v.y-1f));
        float deltaH = Mathf.Abs(scrollRect.content.anchoredPosition.y);
        float r = (defaultWallImageHeight + (-scrollRect.content.anchoredPosition.y)) / defaultWallImageHeight;
        if (v.y >= 1f){
            wallImageParent.anchoredPosition = Vector2.zero;
            wallImageParent.sizeDelta = new Vector2(0f, defaultWallImageHeight + deltaH);
            wallImageRect.localScale = Vector3.one * (defaultWallImageHeight + deltaH ) / defaultWallImageHeight;
        }else{
            wallImageParent.anchoredPosition = scrollRect.content.anchoredPosition;
        }
    }

    private void FitWallImageToParent(){
        float fitHeight = wallImageParent.rect.height;
        float fitWidth = wallImageParent.rect.width;

        float texWidth = wallImage.mainTexture.width;
        float texHeight = wallImage.mainTexture.height;

        float difW = fitWidth - texWidth;
        float difH = fitHeight - texHeight;
      
        float w, h;
        if (fitHeight / fitWidth >= texHeight / texWidth){
            h = fitHeight;
            w = texWidth * (fitHeight / texHeight); 
        }else{
            w = fitWidth;
            h = texHeight * (fitWidth / texWidth);
        }
        /*
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
            // fit - tex であることに注意（符号がマイナス）
            if (difW > difH){
                w = fitWidth;
                h = texHeight * (fitWidth / texWidth);
            }else{
                h = fitHeight;
                w = texWidth * (fitHeight / texHeight);                 
            }
        }   */
        //Debug.Log("w="+w+" h="+h);
        wallImageRect.anchorMin = new Vector2(0.5f, 0.5f);
        wallImageRect.anchorMax = new Vector2(0.5f, 0.5f);
        wallImageRect.sizeDelta = new Vector2(w, h);
    }
}
}