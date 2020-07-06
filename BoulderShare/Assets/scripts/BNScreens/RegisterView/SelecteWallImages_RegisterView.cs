using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoulderNotes{
public class SelecteWallImages_RegisterView : MonoBehaviour
{
    [SerializeField] private List<WallImageCell_RegisterView> list;
    private int cur;


    public void Init(){
        cur = 0;
        foreach(WallImageCell_RegisterView cell in list){
            cell.Init(cur);
            cur++;
        }
        cur = 0;
        list[0].EnableAction();
    }

    //戻り値はfileName, editFilenameの順番になっていなくてはいけない
    public BNWallImage[] GetImages(){
        if (cur == 0){
            return null;
        }
        BNWallImage[] arr = new BNWallImage[list.Count*2];
        int n = list.Count;
        for(int i = 0 ; i < n ; i++){
            BNWallImage[] tmp = list[i].GetWallImages();
            arr[i*2] = tmp[0];
            //editFileNameは未実装
            arr[i*2+1] = tmp[1];
        }
        return arr;
    }

    public void PickImage(int index){
        if (list[index].IsWallSelected()){
            Delete(index);
            return ;
        }
        PickImageManager.Instance.OpenMediaActiveSheet(OnLoadTex, "画像を選択");
    }
    public void Delete(int index){
        int n = list.Count;
        Sprite sprite;
        BNWallImage[] images;
        int i ;
        for(i = index ; i < n ; i++){
            if(i == n - 1 || !list[i+1].IsWallSelected()){
                list[i].Clear();
                if (i != n - 1){
                    list[i+1].DisableAction();
                }
                list[i].EnableAction();
                break;
            }
            sprite = list[i+1].GetDisplaySprite();
            images = list[i+1].GetWallImages();

            list[i].SetWallImages(images, sprite);
        }
        cur--;
    }

    private void OnLoadTex(Texture2D texture){
        list[cur].SetNormalTexture(texture);
        if(cur >= list.Count - 1){
            return ;
        }
        cur++;
        list[cur].EnableAction();
    }

    public void LoadImages(BNScreenStackWithTargetGym stack, List<BNWallImageNames> nameList){
        cur = 0;
        foreach(BNWallImageNames n in nameList){
            list[cur].Load(stack, n);
            cur++;
        }
        if(cur < list.Count){
            list[cur].EnableAction();
        }
    }
}
}