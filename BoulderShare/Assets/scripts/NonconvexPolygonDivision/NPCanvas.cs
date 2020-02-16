using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class NPCanvas : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler{
    private const int FINGER_NONE = -10;
    private int finger = FINGER_NONE;
    [SerializeField] private Camera cam;
    [SerializeField] private float lengthOfEdge = 0.1f;
    [SerializeField] private Material mat;
    [SerializeField] private GameObject npPrefab;
    [SerializeField] private Transform ncps;
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private Transform points;
    [SerializeField] float acceptedDecrementRatio = 0.2f;
    private List<Vector2> vertices;
    private Vector2 currentPoint;

    public void OnPointerDown(PointerEventData data){
        if (vertices == null){
            vertices = new List<Vector2>();
        }else{
            vertices.Clear();
        }
        if (finger == FINGER_NONE){
            finger = data.pointerId;
            currentPoint = GetWorldPoint(data.position);
            AddPoint(currentPoint);
        }
    }
    public void OnDrag(PointerEventData data){
        if (finger == data.pointerId){
            Vector3 vec = GetWorldPoint(data.position);
            Vector2 p = vec;
            if ((currentPoint - p).sqrMagnitude > lengthOfEdge){
                AddPoint(vec);
            }
        }
    }

    public void OnPointerUp(PointerEventData data){
        if (finger == data.pointerId){
            /*
            Debug.Log("vertices:"+vertices.Count);
            foreach(Vector2 v in vertices){
                Debug.Log(v);
            }*/
            Vector2[] arr = vertices.ToArray();
            List<int> inter = new List<int>();
            CalcIntersection(arr, inter);

            if (inter.Count > 2){
                finger = FINGER_NONE;
                return ;
            }
            if(inter.Count == 2){
                int num = inter[1] - inter[0]+1;
                //Debug.Log("ratio:"+(1.0 * num / arr.Length));
                if (1.0 * num / arr.Length < 1.0f - acceptedDecrementRatio){
                    finger = FINGER_NONE;
                    return ;
                }
                Vector2[] copy = new Vector2[num];
                int from1 = inter[0];
                int to1 = inter[0] + 1;
                int from2 = inter[1];
                int to2 = inter[1] + 1;
                if (to2 > arr.Length - 1){
                    to2 = 0;
                }
                copy[0] = CalcIntersectionPoint(arr[from1], arr[to1], arr[from2], arr[to2]);
                Array.Copy(arr, inter[0]+1, copy, 1, num-1);
                arr = copy;
            }
            if (arr.Length < 3){
                finger = FINGER_NONE;
                return ;
            }
            NPMeshing np = new NPMeshing(arr);
            GameObject go = Instantiate(npPrefab, ncps);
            go.SetActive(true);
            MeshFilter filter = go.GetComponent<MeshFilter>();
            filter.mesh = np.GenerateMesh();

            finger = FINGER_NONE;
        }
    }

    //辺どうしが交差している場所を計算する
    private bool CalcIntersection(Vector2[] vs, List<int> intersections = null){
        //signTable[i][j]がtrueなら、辺(i,i+1)が作る直線によって頂点jとj+1が分かれている
        bool[,] signTable = new bool[vs.Length, vs.Length];
        bool hasIntersection = false;

        for(int i = 0 ; i < vs.Length ; i++){
            int toIndex = (i+1) % vs.Length;
            Vector2 edge = vs[toIndex] - vs[i];
            float prevCross = 0.0f;

            for(int j = 0 ; j < vs.Length - 2 ; j++){
                int targetIndex = (toIndex + j + 1) % vs.Length;
                Vector2 sub = vs[targetIndex] - vs[i];
                float cross = edge.x * sub.y - edge.y * sub.x;

                //最初のループは必ずfalse
                if(prevCross * cross < 0.0f){
                    //交差の可能性あり
                    int anotherFrom = targetIndex - 1;
                    if (anotherFrom < 0){
                        anotherFrom = vs.Length - 1;
                    }
                    
                    if (signTable[anotherFrom,i]){
                        hasIntersection = true;
                        if (intersections != null){
                            if (i < anotherFrom){
                                intersections.Add(i);
                                intersections.Add(anotherFrom);
                            }else{
                                intersections.Add(anotherFrom);
                                intersections.Add(i);
                            }
                        }
                    }
                    signTable[i, anotherFrom] = true;
                }
                prevCross = cross;
            }
        }
        return hasIntersection;
    }
    private Vector2 CalcIntersectionPoint(Vector2 a, Vector2 b, Vector2 c, Vector2 d){
        float a1 = (b.y - a.y) / (b.x - a.x);
        float a2 = (d.y - c.y) / (d.x - c.x);

        float x = (a1 * a.x - a.y - a2 * c.x + c.y) / (a1 - a2);
        float y = (b.y - a.y) / (b.x - a.x) * (x - a.x) + a.y;
        return new Vector2(x, y);
    }

    private float Cross(Vector2 a, Vector2 b, Vector2 c){
        Vector2 sub1 = b - a;
        Vector2 sub2 = c - a;
        return sub1.x * sub2.y - sub1.y * sub2.x;
    }

    public Vector3 GetWorldPoint(Vector2 v){
        return cam.ScreenToWorldPoint(new Vector3(v.x, v.y, cam.transform.InverseTransformPoint(this.transform.position).z));
    }

    private void AddPoint(Vector3 vec){
        Vector2 p = vec;
        vertices.Add(p);
        currentPoint = p;
        GameObject point = Instantiate(pointPrefab, points);
        point.SetActive(true);
        point.transform.position = vec;
    }

}
