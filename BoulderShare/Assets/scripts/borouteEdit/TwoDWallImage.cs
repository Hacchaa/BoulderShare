using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class TwoDWallImage : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler {
	private int[] eTouches;
	private float prevLength;
	private bool isUpdate;
	private Vector2 bounds;
	private bool isOn = false;
	private Vector2 offTouchPos ;
	private int wallRotTarget;
	[SerializeField] private Camera cam;
	[SerializeField] private TwoDWall twoDWall;
	[SerializeField] private Transform wallTrans;
	[SerializeField] private CameraManager cameraManager;
	private const int FINGER_NONE = -10;
	[SerializeField] private WallManager wallManager;

	// Use this for initialization
	void Awake () {
		prevLength = -1;
		wallRotTarget = 0;
		eTouches = new int[] {FINGER_NONE,FINGER_NONE};
	}


	public void LateUpdate(){
		isUpdate = false;
	}

	public void IgnoreEvents(){
		gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
	}

	public void AcceptEvents(){
		gameObject.layer = LayerMask.NameToLayer("2D");
	}


	public void OnPointerDown(PointerEventData data){
		bounds = wallManager.GetMasterWallSize();
		
		if (eTouches[0] == FINGER_NONE){
			eTouches[0] = data.pointerId;
		}else if(eTouches[1] == FINGER_NONE){
			eTouches[1] = data.pointerId;
			prevLength = -1;
		}
	}

	public void OnDrag(PointerEventData data){
		Vector2 p1, p2, dP1, dP2;
		p1 = p2 = dP1 = dP2 = Vector2.zero;

		//扱っている２本の指かどうか
		if (isUpdate || eTouches[1] == FINGER_NONE || (data.pointerId != eTouches[0] && data.pointerId != eTouches[1])){
			return ;
		}

		//扱っている指の情報を取得する
		foreach(Touch touch in Input.touches){
			if (touch.fingerId == eTouches[0]){
				p1 = touch.position;
				dP1 = touch.deltaPosition;
			}else if (touch.fingerId == eTouches[1]){
				p2 = touch.position;
				dP2 = touch.deltaPosition;
			}
		}

		float depth = Mathf.Abs(cameraManager.Get2DDepth());
		float length = Vector2.Distance(p1, p2);
	
		isUpdate = true;
		Vector3 wP1 = cam.ScreenToWorldPoint(new Vector3((p1.x + p2.x) / 2.0f, (p1.y + p2.y) / 2.0f, depth));
    	Vector3 wP1Old = cam.ScreenToWorldPoint(new Vector3((p1.x - dP1.x + p2.x - dP2.x) / 2.0f, (p1.y - dP1.y + p2.y - dP2.y) / 2.0f, depth));

    	cameraManager.Translate2D(wP1Old - wP1);
    	cameraManager.Bounds2D(bounds);

		//prevLengthが設定されてしまうている場合
		//prevLengthとlengthの比で拡大、縮小する
		if (prevLength > 0 && length > 0){
			cameraManager.Zoom2D(-(length/prevLength - 1));
		}
		prevLength = length;
		isUpdate = true;
	}

	public void OnPointerUp(PointerEventData data){
		if (eTouches[0] == data.pointerId){
			eTouches[0] = eTouches[1];
			eTouches[1] = FINGER_NONE;
		}else if(eTouches[1] == data.pointerId){
			eTouches[1] = FINGER_NONE;
		}
	}

	public void OnPointerEnter(PointerEventData data){
		isOn = true;
	}

	public void OnPointerExit(PointerEventData data){
		isOn = false;
		offTouchPos = data.position;
	}

	public bool IsOnPointerEnter(){
		return isOn;
	}

	public Vector2 GetOffTouchPos(){
		return offTouchPos;
	}

	public void RotWall(bool isClockwise){
		if(isClockwise){
			wallRotTarget--;
			if (wallRotTarget < 0){
				wallRotTarget = 3;
			}
		}else{
			wallRotTarget++;
			if (wallRotTarget > 3){
				wallRotTarget = 0;
			}
		}
		float ang = 90.0f * wallRotTarget;
		wallTrans.DOLocalRotate(new Vector3(0.0f, 0.0f, ang), 0.25f);
	}

	public void RotateWallTexture(){
		wallTrans.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
		Texture2D texture = twoDWall.GetWallImage();

		if(wallRotTarget > 0 && wallRotTarget <= 3){
			RotTexture(texture, wallRotTarget);
			wallManager.CommitWallImage(texture);
		}
	}

	private void RotTexture(Texture2D tex, int rotType){
		int w = tex.width;
		int h = tex.height;
		Debug.Log("w, h " + w + "," + h);
		Debug.Log("rotType= "+rotType);

		Color[] cOri = tex.GetPixels();
		Color[] cNew = new Color[w * h];
		Debug.Log("tex.GetPixels()= "+cOri.Length);
		Debug.Log("cNew.Length = "+ (w * h));

		for(int i = 0 ; i < cOri.Length ; i++){
			int j = i / w ;
			int k = i % w ;
			Color c = new Color(cOri[i].r, cOri[i].g, cOri[i].b, cOri[i].a);
			//Debug.Log("i "+ i);
			//Debug.Log("j, k "+ j + "," + k);

			if(rotType == 1){
				int tmp = k;
				k = h - 1 - j;
				j = tmp;
				//Debug.Log("after j, k " + j + "," +k);
				cNew[h * j + k] = c; 
			}else if(rotType == 2){
				cNew[w * h - 1 - i] = c;
			}else if(rotType == 3){
				int tmp = k;
				k = j;
				j = w - 1 - tmp;
				//Debug.Log("after j, k " + j + "," +k);
				cNew[h * j + k] = c; 
			};
		}
		if (rotType == 1 || rotType == 3){
			tex.Resize(h, w);
		}
		tex.SetPixels(cNew);
		tex.Apply();
 
	}
}
