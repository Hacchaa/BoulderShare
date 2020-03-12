using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BoulderNotes{
public class DoubleVerticalScroller :  MonoBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IScrollHandler{
    [SerializeField] private ScrollRect scrollerParent;
    [SerializeField] private ScrollRect scrollerChild;

    private IDragHandler parentDragHandler;
    private IBeginDragHandler parentBeginDragHandler;
    private IEndDragHandler parentEndDragHandler;
    private IInitializePotentialDragHandler parentInitialHandler;
    private IScrollHandler parentScrollHandler;
    private IDragHandler childDragHandler;
    private IBeginDragHandler childBeginDragHandler;
    private IEndDragHandler childEndDragHandler;
    private IInitializePotentialDragHandler childInitialHandler;
    private IScrollHandler childScrollHandler;
    private bool moveParent ;

    void Awake(){
        Init();
    }
    public void Init(){
        parentDragHandler = scrollerParent.GetComponent<IDragHandler>();
        parentBeginDragHandler = scrollerParent.GetComponent<IBeginDragHandler>();
        parentEndDragHandler = scrollerParent.GetComponent<IEndDragHandler>();
        parentInitialHandler = scrollerParent.GetComponent<IInitializePotentialDragHandler>();
        parentScrollHandler = scrollerParent.GetComponent<IScrollHandler>();

        childDragHandler = scrollerChild.GetComponent<IDragHandler>();
        childBeginDragHandler = scrollerChild.GetComponent<IBeginDragHandler>();
        childEndDragHandler = scrollerChild.GetComponent<IEndDragHandler>();
        childInitialHandler = scrollerChild.GetComponent<IInitializePotentialDragHandler>();
        childScrollHandler = scrollerChild.GetComponent<IScrollHandler>();
    }

    public void ClampScrollBotWithParent(){
        float pos = scrollerParent.verticalNormalizedPosition;
        //Debug.Log("scrollParent:"+pos);
        if (pos <= 0f){
            scrollerParent.verticalNormalizedPosition = 0f;
        }
    }
    public void ClampScrollTopWithChild(){
        float pos = scrollerChild.verticalNormalizedPosition;
        //Debug.Log("scrollChild:"+pos);
        if (pos >= 1f){
            scrollerChild.verticalNormalizedPosition = 1f;
        }
    }
    public void OnInitializePotentialDrag(PointerEventData data){
        parentInitialHandler.OnInitializePotentialDrag(data);
        childInitialHandler.OnInitializePotentialDrag(data);
    }
    public void OnDrag(PointerEventData ped)
    {   
        float parentPos = scrollerParent.verticalNormalizedPosition;
        float childPos = scrollerChild.verticalNormalizedPosition;

        if (parentPos > 0f){
            if (!moveParent){
                //Debug.Log("to parent");
                parentBeginDragHandler.OnBeginDrag(ped);
            }
            moveParent = true;
            parentDragHandler.OnDrag(ped);
            return ;
        }

        if (childPos >= 1f && ped.delta.y < 0f){
            if (!moveParent){
                //Debug.Log("to parent");
                parentBeginDragHandler.OnBeginDrag(ped);
            }
            moveParent = true;
            parentDragHandler.OnDrag(ped);
            return ;
        }

        if (moveParent){
            //Debug.Log("to child");
            childBeginDragHandler.OnBeginDrag(ped);
        }
        moveParent = false;
        childDragHandler.OnDrag(ped);
    }
    public void OnScroll(PointerEventData data){
        parentScrollHandler.OnScroll(data);
        childScrollHandler.OnScroll(data);
    }
    public void OnBeginDrag(PointerEventData ped)
    {
        parentBeginDragHandler.OnBeginDrag(ped);
        childBeginDragHandler.OnBeginDrag(ped);
    }
    public void OnEndDrag(PointerEventData ped)
    {
        parentEndDragHandler.OnEndDrag(ped);
        childEndDragHandler.OnEndDrag(ped);
    }
}
}