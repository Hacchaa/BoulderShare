using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collisiotntest : MonoBehaviour
{
	[SerializeField] private Collider col;
    // Update is called once per frame
    void Update()
    {	
    	Quaternion localRot = transform.localRotation;
    	transform.localRotation = Quaternion.identity;
    	Vector3 boxSize = col.bounds.size;
    	transform.localRotation = localRot;
    	Bounds b = col.bounds;
    	Debug.Log("b.center:"+b.center + " boxSize:"+boxSize);
        Collider[] hitColliders = Physics.OverlapBox(b.center, boxSize/2.0f, transform.localRotation);

        for(int i = 0 ; i < hitColliders.Length ; i++){
        	if (!gameObject.name.Equals(hitColliders[i].gameObject.name)){
	        	Debug.Log("hit:"+hitColliders[i].gameObject.name+ ",pos:"+hitColliders[i].transform.position);
	        	EventTest src = hitColliders[i].gameObject.GetComponent<EventTest>();
	        	if (src != null){
	        		Transform hitColTra = hitColliders[i].gameObject.transform;
	        		//外→表面
	        		Vector3 colPoint = col.ClosestPoint(hitColTra.position);
	        		//内→表面
	        		Vector3 colPointLocal = transform.InverseTransformPoint(colPoint);
	        		Vector3 bCenterLocal = transform.InverseTransformPoint(b.center);
	        		Vector3 dif = colPointLocal - bCenterLocal;
	        		Debug.Log("colPointLocal:"+colPointLocal);
	        		Debug.Log("bCenterLocal:"+bCenterLocal);
	        		Debug.Log("dif:"+dif);
	        		float[] v = new float[3];
	        		v[0] = Mathf.Abs(Mathf.Abs(dif.x) - boxSize.x / transform.localScale.x / 2);
	        		v[1] = Mathf.Abs(Mathf.Abs(dif.y) - boxSize.y / transform.localScale.y / 2);
	        		v[2] = Mathf.Abs(Mathf.Abs(dif.z) - boxSize.z / transform.localScale.z / 2);
	        		Debug.Log("v:"+v[0]+" "+v[1]+" "+v[2]);
	        		float min = Mathf.Min(v);
	        		float inv = 1f;
	        		if (min == v[0]){
        				if (dif.x < 0){
        					inv *= -1;
        				}
        				colPoint = transform.TransformPoint(new Vector3(bCenterLocal.x + (boxSize.x / transform.localScale.x / 2 * inv), colPointLocal.y, colPointLocal.z));

        			}else if(min == v[1]){
        				if (dif.y < 0){
        					inv *= -1;
        				}
        				colPoint = transform.TransformPoint(new Vector3(colPointLocal.x, bCenterLocal.y + (boxSize.y / transform.localScale.y / 2 * inv), colPointLocal.z));

        			}else if(min == v[2]){
        				if (dif.z < 0){
        					inv *= -1;
        				}
        				colPoint = transform.TransformPoint(new Vector3(colPointLocal.x, colPointLocal.y, bCenterLocal.z + (boxSize.z / transform.localScale.z / 2 * inv)));
	        		}
	        		Vector3 dir = (colPoint - hitColTra.position).normalized;
	        		if ((b.center - hitColTra.position).sqrMagnitude > (b.center - colPoint).sqrMagnitude){
	        				dir *= -1;
	        		}
	        		hitColTra.position = colPoint + dir * src.GetR();
	        		//hitColTra.position = colPoint;
	        		Debug.Log("colPoint:"+colPoint + ", newPos:"+hitColTra.position + "R:"+src.GetR());
	        		//Debug.Break();
	        	}
        	}
        }
    }

    public float GetLength(){
    	return transform.localScale.x / 2;
    }
}
