using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BoulderNotes{
public class WallImageCell_RegisterView : MonoBehaviour
{
    [SerializeField] private GameObject plusIcon;
    [SerializeField] private Image image;
    [SerializeField] private Button activeButton;
    [SerializeField] private SelecteWallImages_RegisterView wallImages_RegisterView;
    private bool isWallSelected;

    private BNWallImage[] images;
    private int index;
    public void Init(int ind){
        plusIcon.SetActive(false);
        isWallSelected = false;
        activeButton.enabled = false;
        image.sprite = null;
        images = new BNWallImage[2];
        index = ind;
    }

    public void Load(BNScreenStackWithTargetGym stack, BNWallImageNames names){
        if (!string.IsNullOrEmpty(names.fileName)){
            Sprite sprite = stack.LoadImageByES3(names.fileName);
            images[0] = new BNWallImage(sprite.texture, names.fileName);
            isWallSelected = true;
            displayImage(sprite);
        }
        if (!string.IsNullOrEmpty(names.editedFileName)){
            Texture2D tex = stack.LoadTextureByES3(names.editedFileName);
            images[1] = new BNWallImage(tex, names.editedFileName);
        }
        activeButton.enabled = true;
        plusIcon.SetActive(false);
    }

    public void Clear(){
        plusIcon.SetActive(false);
        isWallSelected = false;
        activeButton.enabled = false;
        image.sprite = null;
        images[0] = null;
        images[1] = null;
    }
    public bool IsWallSelected(){
        return isWallSelected;
    }

    public void EnableAction(){
        activeButton.enabled = true;
        if (!isWallSelected){
            plusIcon.SetActive(true);
        }
    }
    public void DisableAction(){
        activeButton.enabled = false;
        plusIcon.SetActive(false);
    }

    public void displayImage(Sprite sprite){
        BNManager.Instance.FitImageToParent(image.GetComponent<RectTransform>(), null, sprite.texture);
        image.sprite = sprite;
    }
/*
    public Texture2D GetTexture(){
        if (image.sprite == null){
            return null;
        }
        return image.sprite.texture;
    }*/
    public BNWallImage[] GetWallImages(){
        return images;
    }
    public Sprite GetDisplaySprite(){
        return image.sprite;
    }

    public void SetWallImages(BNWallImage[] arr, Sprite sprite){
        images = arr;
        displayImage(sprite);
    }

    public void SetNormalTexture(Texture2D tex){
        Sprite sprite = BNManager.Instance.CreateSprite(tex);
        displayImage(sprite);
        plusIcon.SetActive(false);
        isWallSelected = true;
        images[0] = new BNWallImage(tex);
    }

    public void PushButton(){
        wallImages_RegisterView.PickImage(index);
    }
}
}