using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NPNode : IComparable
{
    public int index;
    private float dist;
    private NPNode prev;
    private NPNode next;
    private bool isRegister;

    public NPNode(Vector2 v, int ind){
        index = ind;
        dist = v.sqrMagnitude;
        isRegister = false;
    }
    public NPNode GetPrev(){
        return prev;
    }
    public void SetPrev(NPNode node){
        prev = node;
    }

    public NPNode GetNext(){
        return next;
    }

    public void SetNext(NPNode node){
        next = node;
    }

    public float GetDist(){
        return dist;
    }

    public bool IsRegister(){
        return isRegister;
    }
    public void SetIsRegister(bool b){
        isRegister = b;
    }

    public void Remove(){
        prev.next = next;
        next.prev = prev;

        prev = null;
        next = null;
    }

    public int CompareTo(object obj){
        return (int)(this.GetDist() - ((NPNode)obj).GetDist());
    }
}
