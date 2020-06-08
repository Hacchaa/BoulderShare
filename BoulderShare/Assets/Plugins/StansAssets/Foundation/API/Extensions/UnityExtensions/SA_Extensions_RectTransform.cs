using UnityEngine;
using SA.Foundation.Animation;

public static class SA_Extensions_RectTransform
{
    public static void Reset(this RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    public static void MoveTo(this RectTransform transform, object callbackTarget, Vector2 position, float time, SA_EaseType easeType = SA_EaseType.linear, System.Action OnCompleteAction = null)
    {
        var tw = SA_ValuesTween.Create();

        tw.VectorTo(transform.anchoredPosition, position, time, easeType);
        tw.DestoryGameObjectOnComplete = true;
        tw.OnVectorValueChanged.AddSafeListener(transform, (Vector3 pos) =>
        {
            transform.anchoredPosition = pos;
        });

        tw.OnComplete.AddSafeListener(callbackTarget, OnCompleteAction);
    }

    public static Rect GetScreenRect(this RectTransform transform)
    {
        var rtCorners = new Vector3[4];
        transform.GetWorldCorners(rtCorners);
        var rtRect = new Rect(new Vector2(rtCorners[0].x, rtCorners[0].y), new Vector2(rtCorners[3].x - rtCorners[0].x, rtCorners[1].y - rtCorners[0].y));

        var canvas = transform.GetComponentInParent<Canvas>();
        var canvasCorners = new Vector3[4];
        canvas.GetComponent<RectTransform>().GetWorldCorners(canvasCorners);
        var cRect = new Rect(new Vector2(canvasCorners[0].x, canvasCorners[0].y), new Vector2(canvasCorners[3].x - canvasCorners[0].x, canvasCorners[1].y - canvasCorners[0].y));

        var screenWidth = Screen.width;
        var screenHeight = Screen.height;

        var size = new Vector2(screenWidth / cRect.size.x * rtRect.size.x, screenHeight / cRect.size.y * rtRect.size.y);
        var rect = new Rect(screenWidth * ((rtRect.x - cRect.x) / cRect.size.x), screenHeight * ((-cRect.y + rtRect.y) / cRect.size.y), size.x, size.y);
        return rect;
    }
}
