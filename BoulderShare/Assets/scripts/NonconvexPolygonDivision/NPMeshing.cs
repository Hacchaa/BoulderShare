using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NPMeshing 
{
    private Vector3[] vertices;
    private int[] triangles;
    private int triIndex;
    private NPNode start;
    private List<NPNode> distList;
    //ある三角形において、Cross()で与えられる外積がglobalCrossと同じ符号なら、凸三角形
    private float globalCross;

    public NPMeshing(Vector2[] v){
        vertices = new Vector3[v.Length];
        for(int i = 0 ; i < v.Length ; i++){
            vertices[i] = new Vector3(v[i].x, v[i].y, 0.0f);
        }
        triangles = new int[(v.Length - 2) * 3];
    }

    public void Clear(){
        distList.Clear();
        NPNode node = start;

        while(node != null){
            NPNode next = node.GetNext();
            node.SetPrev(null);
            node.SetNext(null);
            node = next;
        }
        start = null;
        globalCross = 0.0f;
    }

    private void ConstructNodes(){
        distList = new List<NPNode>();
        triIndex = 0;

        start = new NPNode(vertices[0], 0);
        distList.Add(start);
        NPNode prev = start;
        for(int i = 1 ; i < vertices.Length ; i++){
            NPNode cur = new NPNode(vertices[i], i);
            cur.SetPrev(prev);
            prev.SetNext(cur);

            distList.Add(cur);
            prev = cur;
        }
        start.SetPrev(prev);
        prev.SetNext(start);

        distList = distList.OrderByDescending(x => x.GetDist()).ToList();
        NPNode node = distList[0];
        globalCross = Cross(node, node.GetPrev(), node.GetNext());
    }

    public Mesh GenerateMesh(){
        ConstructNodes();
        //remain = new List<Vector2>(vertices);
        int count = vertices.Length;

        while(count > 3){
            CreateTriangle();
            count--;
        }
        
        AddTriangle(distList[0]);

        Mesh mesh = new Mesh();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        Clear();
        return mesh;
    }

    private void CreateTriangle(){
        NPNode start = distList[0];
        NPNode center = start;
        NPNode prev = center.GetPrev();
        NPNode next = center.GetNext();

        if (!IsPointWithinTriangle(center)){
            AddTriangle(center);
            distList.RemoveAt(0);
            center.Remove();
            return ;
        }

        prev = center;
        center = next;
        next = center.GetNext();

        while(center != start){
            float cross = Cross(center, prev, next);
            if(globalCross * cross > 0.0f){
                if (!IsPointWithinTriangle(center)){
                    AddTriangle(center);
                    distList.Remove(center);
                    center.Remove();
                    return ;                   
                }
            }

            prev = center;
            center = next;
            next = center.GetNext();
        }
        
    }


    private void AddTriangle(NPNode node){
        if (globalCross > 0.0f){
            AddTriangle(node.index, node.GetNext().index, node.GetPrev().index);
        }else{
            AddTriangle(node.index, node.GetPrev().index, node.GetNext().index);
        }
    }

    private void AddTriangle(int a, int b, int c){
        triangles[triIndex] = a;
        triangles[triIndex+1] = b;
        triangles[triIndex+2] = c;

        triIndex += 3;
    }

    private bool IsPointWithinTriangle(NPNode start){
        NPNode center = start;
        NPNode prev = center.GetPrev();
        NPNode next = center.GetNext();
        NPNode p = next.GetNext();

        while(prev != p){
            if(IsPointWithinTriangle(p, center, next, prev)){
                return true;
            }
            p = p.GetNext();
        }
        return false;
    }

    private bool IsPointWithinTriangle(NPNode p, NPNode a, NPNode b, NPNode c){
        float sign = Cross(a, b, p);
        
        if (sign * Cross(b, c, p) < 0.0f){
            return false;
        }
        if (sign * Cross(c, a, p) < 0.0f){
            return false;
        }
        return true;
    }

    //center,aからcenter,bに向かう方向
    private float Cross(NPNode center, NPNode a, NPNode b){
        Vector3 sub1 = vertices[a.index] - vertices[center.index];
        Vector3 sub2 = vertices[b.index] - vertices[center.index];
        return sub1.x * sub2.y - sub1.y * sub2.x;
    }
}
