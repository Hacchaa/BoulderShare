using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BoulderNotes{
public class DisplayImage : MonoBehaviour
{
    private RectTransform root;
    [SerializeField] private Image image;
    private RectTransform imageRect;
    private Vector2 size;
    private Sprite[] sprites;
    private int cur;

    public void Init(float w, float h, float x, float y, Sprite sprite, Sprite editedSprite){
        if  (sprites == null){
            sprites = new Sprite[2];
        }
        sprites[0] = sprite;
        sprites[1] = editedSprite;

        if (root == null){
            root = GetComponent<RectTransform>();
            Vector2 setting = new Vector2(0f, 0.5f);
            root.pivot = setting;
            root.anchorMax = setting;
            root.anchorMin = setting;
            root.sizeDelta = new Vector2(w, h);
        }
        root.anchoredPosition = new Vector2(x, y);

        if(imageRect == null){
            imageRect = image.GetComponent<RectTransform>();
        }

        if(editedSprite != null){
            cur = 1;
            BNManager.Instance.FitImageToParent(image, imageRect, root, editedSprite);
        }else{
            cur = 0;
            BNManager.Instance.FitImageToParent(image, imageRect, root, sprite);
        }
        size = imageRect.sizeDelta;
    }
    public void Clear(){
        imageRect.sizeDelta = size;
        imageRect.anchoredPosition = Vector2.zero;
        if (sprites[1] != null && cur == 0){
            image.sprite = sprites[1];
            cur = 1;
        }
    }

    public Vector2 GetSize(){
        return size;
    }

    public bool IsAlreadyMoved(){
        return imageRect.sizeDelta != size;
    }

    public RectTransform GetViewRect(){
        return root;
    }

    public RectTransform GetImageRect(){
        return imageRect;
    }

    public bool IsInTheRightSide(){
        if (root.rect.width/2f >= imageRect.anchoredPosition.x + imageRect.rect.width/2f){
			return true;
		}
        return false;
    }
    public bool IsInTheLeftSide(){
        if (- root.rect.width/2f <= imageRect.anchoredPosition.x - imageRect.rect.width/2f){
			return true;
		}
        return false;
    }

    public void ClampOnRight(){
        float x = root.rect.width/2f - imageRect.rect.width/2f;
        imageRect.anchoredPosition = new Vector2(x, imageRect.anchoredPosition.y);
    }
    public void ClampOnLeft(){
        float x = root.rect.width/2f - imageRect.rect.width/2f;
        imageRect.anchoredPosition = new Vector2(-x, imageRect.anchoredPosition.y);
    }

    public bool HasEditedSprite(){
        return sprites[1] != null;
    }

    public void ChangeWallImage(){
        if (sprites[1] == null){
            return ;
        }
        if (cur == 0){
            cur = 1;
        }else{
            cur = 0;
        }
        image.sprite = sprites[cur];
    }

    public bool ShowedOrigin(){
        return cur == 0;
    }
}
}