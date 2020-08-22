using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BoulderNotes{
public delegate void LineAction(int x, int y);

[RequireComponent(typeof(Image))]
public class MobilePaintUGUI : MonoBehaviour
{
    public enum DrawMode
    {
        Default,
        FloodFill,
        Eraser
    }
    private DrawMode drawMode;
    [SerializeField] private Camera cam;
    [SerializeField] private string targetTextureName;
    [SerializeField] private string mainTextureName;
    private Image image;
    private RectTransform imageRect;
    private Material material;
    private Texture2D drawingTexture;
    private int texWidth;
    private int texHeight;

    [SerializeField] private int brushSize;
    [SerializeField] private bool hiQualityBrush;
    private int brushSizePow2;
    private int brushSizeX4;
    private int brushSizeDiv4;
    [SerializeField] private Color32 paintColor;

    private byte[] pixels;
    private Vector2[] pixelUVs;
    private Vector2[] pixelUVOlds;
    [SerializeField] private int touchableFingerNum = 1;
    private Dictionary<int, int> fingerToUVIndexMap;

    [SerializeField] private bool enableUndo;
    private List<byte[]> undoPixels;
    private int maxUndoBuffers = 10;

    [SerializeField] private Color32 clearColor;

    private bool textureNeedsUpdate;
    [SerializeField] private float textureUpdateInterval = 0.1f;
    private float nextTextureUpdate;

    private bool goneOut;
    [SerializeField] private bool connectBrushStokes;

    private bool isDoneFirstProc = false;
    private void FirstProc(){
        image = GetComponent<Image>();
        imageRect = image.GetComponent<RectTransform>();
        imageRect.anchorMin = new Vector2(0.5f, 0.5f);
        imageRect.anchorMax = new Vector2(0.5f, 0.5f);
        imageRect.pivot = new Vector2(0.5f, 0.5f);

        material = image.material;
        pixelUVs = new Vector2[touchableFingerNum];
        pixelUVOlds = new Vector2[touchableFingerNum];
        fingerToUVIndexMap = new Dictionary<int, int>();
        if (enableUndo){
            undoPixels = new List<byte[]>();
        }
        drawMode = DrawMode.Default;
        isDoneFirstProc = true;
    }

    public void Init(Texture mainTex){
        if (!isDoneFirstProc){
            FirstProc();
        }
        OnDestroy();

        SetBrushSizeByPixel(brushSize);

        if (mainTex == null){
            mainTex = material.GetTexture(mainTextureName);
        }else{
            material.SetTexture(mainTextureName, mainTex);
            image.enabled = false;
            image.enabled = true;
        }
        texWidth = mainTex.width;
        texHeight = mainTex.height;

        BNManager.Instance.FitImageToParent(imageRect, null, mainTex);

        pixels = new byte[texWidth * texHeight * 4];

        drawingTexture = new Texture2D(texWidth, texHeight, TextureFormat.RGBA32, false);
        material.SetTexture(targetTextureName, drawingTexture);

        drawingTexture.filterMode = FilterMode.Point;
        drawingTexture.wrapMode = TextureWrapMode.Clamp;

        ClearImage(false);
        textureNeedsUpdate = false;
    }
    void Update()
    {
        if (!isDoneFirstProc){
            return;
        }

        if (textureNeedsUpdate && Time.time > nextTextureUpdate)
        {
            nextTextureUpdate = Time.time + textureUpdateInterval;
            UpdateTexture();
            textureNeedsUpdate = false;
        }
    }
    public void OnBeginDrag(PointerEventData data){
        int n = fingerToUVIndexMap.Count;
        if (n > touchableFingerNum){
            return ;
        }
        fingerToUVIndexMap.Add(data.pointerId, n);

        int index = n;

        if (enableUndo)
        {
            GrabUndoBufferNow();
        }

        // get hit texture coordinate
        CalcTextureCoordinateFromScreenPoint(data.position, out pixelUVs[index]);
        goneOut = false;
    }

    public void OnDrag(PointerEventData data){
        if (!fingerToUVIndexMap.ContainsKey(data.pointerId)){
            return ;
        }
        int index = fingerToUVIndexMap[data.pointerId];
        Vector2 v;
        bool isIn = CalcTextureCoordinateFromScreenPoint(data.position, out v);
        if (!isIn){
            goneOut = true;
            return ;
        }

        pixelUVOlds[index] = pixelUVs[index];
        pixelUVs[index] = v;

        switch (drawMode){
            case DrawMode.Default:
                DrawCircle((int)pixelUVs[index].x, (int)pixelUVs[index].y);
                break;

            case DrawMode.FloodFill:
                FloodFill((int)pixelUVs[index].x, (int)pixelUVs[index].y);
                break;

            case DrawMode.Eraser:
                EraseWithBackgroundColor((int)pixelUVs[index].x, (int)pixelUVs[index].y);
                break;

            default:
                break;
        }

        if (connectBrushStokes){
            if (goneOut){
                goneOut = false;
                pixelUVOlds[index] = pixelUVs[index];
            }

            switch (drawMode){
                case DrawMode.Default:
                    DrawLine(pixelUVOlds[index], pixelUVs[index]);
                    break;

                case DrawMode.Eraser:
                    EraseWithBackgroundColorLine(pixelUVOlds[index], pixelUVs[index]);
                    break;

                default:
                    // unknown mode
                    break;
            }
        }
        textureNeedsUpdate = true;
    }

    public void OnEndDrag(PointerEventData data){
        if (!fingerToUVIndexMap.ContainsKey(data.pointerId)){
            return ;
        }
        fingerToUVIndexMap.Remove(data.pointerId); 
    }
    public void DrawCircle(int x, int y){
        int pixel = 0;

        for(int py = -brushSize ; py < brushSize ; py++){
            for(int px = -brushSize ; px < brushSize ; px++){
                if (px * px + py * py > brushSizePow2) continue;
                if (x + px < 0 || y + py < 0 || x + px >= texWidth || y + py >= texHeight) continue; // temporary fix for corner painting

                pixel = (texWidth * (y + py) + x + px) * 4;

                pixels[pixel] = paintColor.r;
                pixels[pixel + 1] = paintColor.g;
                pixels[pixel + 2] = paintColor.b;
                pixels[pixel + 3] = paintColor.a;
            }
        }
    } 
    public void EraseWithBackgroundColor(int x, int y){
        var origColor = paintColor;
        paintColor = clearColor;

        DrawCircle(x, y);
        paintColor = origColor;
    }
    private void FloodFill(int x, int y){
        byte hitColorR = pixels[((texWidth * (y) + x) * 4) + 0];
        byte hitColorG = pixels[((texWidth * (y) + x) * 4) + 1];
        byte hitColorB = pixels[((texWidth * (y) + x) * 4) + 2];
        byte hitColorA = pixels[((texWidth * (y) + x) * 4) + 3];

        if (paintColor.r == hitColorR && paintColor.g == hitColorG && paintColor.b == hitColorB && paintColor.a == hitColorA){
            return;
        }

        Queue<int> fillPointX = new Queue<int>();
        Queue<int> fillPointY = new Queue<int>();
        fillPointX.Enqueue(x);
        fillPointY.Enqueue(y);

        int ptsx, ptsy;
        int pixel = 0;

        while (fillPointX.Count > 0){
            ptsx = fillPointX.Dequeue();
            ptsy = fillPointY.Dequeue();

            if (ptsy - 1 > -1){
                pixel = (texWidth * (ptsy - 1) + ptsx) * 4; // down
                if (pixels[pixel + 0] == hitColorR
                    && pixels[pixel + 1] == hitColorG
                    && pixels[pixel + 2] == hitColorB
                    && pixels[pixel + 3] == hitColorA)
                {
                    fillPointX.Enqueue(ptsx);
                    fillPointY.Enqueue(ptsy - 1);
                    DrawPoint(pixel);
                }
            }

            if (ptsx + 1 < texWidth){
                pixel = (texWidth * ptsy + ptsx + 1) * 4; // right
                if (pixels[pixel + 0] == hitColorR
                    && pixels[pixel + 1] == hitColorG
                    && pixels[pixel + 2] == hitColorB
                    && pixels[pixel + 3] == hitColorA)
                {
                    fillPointX.Enqueue(ptsx + 1);
                    fillPointY.Enqueue(ptsy);
                    DrawPoint(pixel);
                }
            }

            if (ptsx - 1 > -1){
                pixel = (texWidth * ptsy + ptsx - 1) * 4; // left
                if (pixels[pixel + 0] == hitColorR
                    && pixels[pixel + 1] == hitColorG
                    && pixels[pixel + 2] == hitColorB
                    && pixels[pixel + 3] == hitColorA)
                {
                    fillPointX.Enqueue(ptsx - 1);
                    fillPointY.Enqueue(ptsy);
                    DrawPoint(pixel);
                }
            }

            if (ptsy + 1 < texHeight){
                pixel = (texWidth * (ptsy + 1) + ptsx) * 4; // up
                if (pixels[pixel + 0] == hitColorR
                    && pixels[pixel + 1] == hitColorG
                    && pixels[pixel + 2] == hitColorB
                    && pixels[pixel + 3] == hitColorA)
                {
                    fillPointX.Enqueue(ptsx);
                    fillPointY.Enqueue(ptsy + 1);
                    DrawPoint(pixel);
                }
            }
        }
    }
    public void DrawPoint(int pixel){
        pixels[pixel] = paintColor.r;
        pixels[pixel + 1] = paintColor.g;
        pixels[pixel + 2] = paintColor.b;
        pixels[pixel + 3] = paintColor.a;
    }
    public void DrawLine(int startX, int startY, int endX, int endY, LineAction action){
        if (action == null){
            return ;
        }
        int x1 = endX;
        int y1 = endY;
        int tempVal = x1 - startX;
        int dx = (tempVal + (tempVal >> 31)) ^ (tempVal >> 31); // http://stackoverflow.com/questions/6114099/fast-integer-abs-function
        tempVal = y1 - startY;
        int dy = (tempVal + (tempVal >> 31)) ^ (tempVal >> 31);


        int sx = startX < x1 ? 1 : -1;
        int sy = startY < y1 ? 1 : -1;
        int err = dx - dy;
        int pixelCount = 0;
        int e2;
        for (;;) // endless loop
        {
            if (hiQualityBrush)
            {
                action(startX, startY);
            }
            else {
                pixelCount++;
                if (pixelCount > brushSizeDiv4) // might have small gaps if this is used, but its alot(tm) faster to skip few pixels
                {
                    pixelCount = 0;
                    action(startX, startY);
                }
            }

            if (startX == x1 && startY == y1) break;
            e2 = 2 * err;
            if (e2 > -dy)
            {
                err = err - dy;
                startX = startX + sx;
            }
            else if (e2 < dx)
            {
                err = err + dx;
                startY = startY + sy;
            }
        }
    }

    public void DrawLine(Vector2 start, Vector2 end){
        DrawLine((int)start.x, (int)start.y, (int)end.x, (int)end.y, DrawCircle);
    }
    public void EraseWithBackgroundColorLine(Vector2 start, Vector2 end){
        DrawLine((int)start.x, (int)start.y, (int)end.x, (int)end.y, EraseWithBackgroundColor);
    }

    public void ClearImage(bool updateUndoBuffer = true){
        if(enableUndo && updateUndoBuffer){
            GrabUndoBufferNow();
        }

        for (int ind = 0 ; ind < pixels.Length; ind +=4)
        {
            pixels[ind] = clearColor.r;
            pixels[ind + 1] = clearColor.g;
            pixels[ind + 2] = clearColor.b;
            pixels[ind + 3] = clearColor.a;
        }

        UpdateTexture();
    }

    private void UpdateTexture(){
        drawingTexture.LoadRawTextureData(pixels);
        drawingTexture.Apply(false);
    }
    public void DoUndo()
    {
        if (enableUndo)
        {
            int n = undoPixels.Count;
            if (n > 0)
            {
                byte[] tmp = undoPixels[n-1];
                System.Array.Copy(tmp, pixels, tmp.Length);
                drawingTexture.LoadRawTextureData(tmp);
                drawingTexture.Apply(false);

                undoPixels.RemoveAt(n - 1);
            } // else, no undo available
        }
    }

    public bool CanDoUndo(){
        return enableUndo && undoPixels.Count > 0;
    }

    public void GrabUndoBufferNow(){
        int n = undoPixels.Count;
        // TODO: remove oldest item, if too many buffers
        if (n >= maxUndoBuffers)
        {
            undoPixels.RemoveAt(0);
            n--;
        }

        undoPixels.Add(new byte[pixels.Length]); // TODO: need to reset size, if image size changes
        n++;
        System.Array.Copy(pixels, undoPixels[n - 1], pixels.Length);        
    }

    public void SetDrawMode(DrawMode mode){
        drawMode = mode;
    }

    public void SetPaintColor(Color32 newColor){
        paintColor = newColor;
    }
    public void SetBrushSizeByPixel(int newSize)
    {
        brushSize = (int)Mathf.Clamp(newSize, 1, 999);

        brushSizePow2 = brushSize * brushSize;
        brushSizeX4 = brushSize * 4;
        brushSizeDiv4 = brushSize / 4;
    }
    public void SetBrushSizeByPt(float sizePt){
        SetBrushSizeByPixel(Mathf.RoundToInt(sizePt * GetRatioOfImageTextureToImageSize()));
    }

    public int GetBrushSize(){
        return brushSize;
    }

    public Image GetImage(){
        return image;
    }

    public float GetRatioOfImageTextureToImageSize(){
        //Debug.Log("ratio:"+(texWidth / imageRect.sizeDelta.x));
        return texWidth / imageRect.sizeDelta.x;
    }

    //return bool:Is screenPoint in the image.
    private bool CalcTextureCoordinateFromScreenPoint(Vector2 screenPoint, out Vector2 coord){
        Vector2 local;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(imageRect, screenPoint, cam, out local);
        //uv座標の原点を基準にする
        Vector2 size = imageRect.sizeDelta;
        float r = texWidth / size.x;
        coord.x = (local.x + size.x * 0.5f) * r;
        coord.y = (local.y + size.y * 0.5f) * r;
        //Debug.Log("screenPoint:"+screenPoint.x+","+screenPoint.y+" localPoint:"+local.x+","+local.y);
        //Debug.Log("coord:"+coord.x+","+coord.y);
        if (coord.x < 0 || coord.x > texWidth || coord.y < 0 || coord.y > texHeight){
            return false;
        }
        return true;
    }
    void OnDestroy()
    {
        if (drawingTexture != null){
            Destroy(drawingTexture);
        }
        pixels = null;
        if (enableUndo && undoPixels != null){
            undoPixels.Clear();
        }

        // System.GC.Collect();
        if (fingerToUVIndexMap != null){
            fingerToUVIndexMap.Clear();
        }
    }
}
}