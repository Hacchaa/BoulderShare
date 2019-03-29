using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class DAG : MonoBehaviour
{
    [SerializeField] private NodeSet nodeSet;
    [SerializeField] private EdgeSet edgeSet;
    [SerializeField] private Transform drawField;
    public int num;
    private List<IBNode> rootList;
    private IBNode currentTarget;

    private IBNode[][] arrangeMap;
    private List<List<IBNode>> attemptTreeList;

    private const int ITERATION_COG = 10;
    private const int ITERATION_PRIORITY = 10;
    private const string DUMMYNODETAG = "DummyNode";
    [SerializeField] private int showIndex;

    void Awake(){
    	rootList = new List<IBNode>();
    	currentTarget = null;
    	nodeSet.transform.position = drawField.position;
    	edgeSet.transform.position = drawField.position;
    	attemptTreeList = new List<List<IBNode>>();
    	showIndex = -1;
    }

    private void Init(){
    	rootList.Clear();
    	currentTarget = null;
    	nodeSet.Init();
    	edgeSet.Init();
    	attemptTreeList.Clear();
    	showIndex = -1;
    }

    public int GetShowIndex(){
    	return showIndex;
    }

    public void ShowNext(){
    	int n = attemptTreeList.Count;
    	if (n == 0){
    		return ;
    	}
    	showIndex++;
    	if (showIndex > n - 1){
    		showIndex = 0;
    	}
    	ShowAT();
    }

    public void ShowPrev(){
    	int n = attemptTreeList.Count;
    	if (n == 0){
    		return ;
    	}
    	showIndex--;
    	if (showIndex < 0){
    		showIndex = n - 1;
    	}
    	ShowAT();    	
    }

    public void ShowAt(int ind){
    	int n = attemptTreeList.Count;
    	if (n == 0 || ind < 0 || ind > n - 1){
    		return ;
    	}

    	if (showIndex < 0){
    		showIndex = n - 1;
    	}    	
    	ShowAT();
    }

    public Vector3 GetViewPos(){
    	return drawField.position + new Vector3(1.0f, 1.0f);
    }

    public List<IBNodeData> MakeNodeData(MyUtility.AttemptTree at, Dictionary<int, MyUtility.Scene> sceneMap){
    	List<IBNodeData> list = new List<IBNodeData>();
    	foreach(int id in at.idList){
    		list.Add(new BNodeData(id));
    	}
    	return list;
    }

    public List<IBNode> MakeFirstTree(List<IBNodeData> list){
    	List<IBNode> l = new List<IBNode>();
    	IBNode curNode = nodeSet.MakeNode(list[0], null, null);
    	rootList.Add(curNode);
    	l.Add(curNode);
		int n = list.Count;
		for(int i = 1 ; i < n ; i++){
			curNode = nodeSet.MakeNode(list[i], curNode, null);
			l.Add(curNode);
		}

		return l;
    }

    public void Construction(List<MyUtility.AttemptTree> atList, Dictionary<int, MyUtility.Scene> sceneMap){
		int atNum = atList.Count;
		int oldCreationNumber = -1;
		//Debug.Log("atNum="+atNum);
		
		Init();

		for(int i = 0 ; i < atNum ; i++){
			//Debug.Log("i="+i);
			List<IBNodeData> list = MakeNodeData(atList[i], sceneMap);
			List<IBNode> nodeList = new List<IBNode>();
			int curCreationNumber = atList[i].numOfCreatingHScene;

			if (i == 0){
				//Debug.Log("makeFirstTree()");
				//baseの木を構成
				nodeList = MakeFirstTree(list);
			}else{
				//初めにbaseのどの根からスタートするか求める
				IBNode curNode = null;
				IBNodeData curData = list[0];
				//Debug.Log("oldCreationNumber <= curData.GetID() == "+oldCreationNumber + " <= " + curData.GetID());
				//curNodeの値を持つシーンがbaseに存在するかどうか
				if (oldCreationNumber <= curData.GetID()){
					//存在しない場合
					//curNodeをbaseの根に追加する
					curNode = nodeSet.MakeNode(curData, null, null);
					rootList.Add(curNode);
				}else{
					//存在する場合
					//curNodeと値が等しいbaseのノードを探してそこから始める
					curNode = DAGNode.Search(rootList, curData);
				}
				nodeList.Add(curNode);
				int m = list.Count;
				//Debug.Log("m="+m);
				for(int j = 1 ; j < m ; j++){
					//Debug.Log("j="+j);
					curData = list[j];
					//Debug.Log("oldCreationNumber <= curData.GetID() == "+oldCreationNumber + " <= " + curData.GetID());
					//curDataを持つシーンがbaseに存在するかどうか
					if(oldCreationNumber <= curData.GetID()){
						//存在しない場合、
						//curNodeから分岐する
						curNode = nodeSet.MakeNode(curData, curNode, null);
					}else{
						//存在する場合
						//rootlistから子供を探す
						IBNode tmp = DAGNode.Search(rootList, curData);

						//tmpがcurNodeの子供でない場合
						if (!curNode.IsParent(tmp)){
							//新しく子供に設定する
							curNode.AddChild(tmp);
							//tmpがrootListに属している場合、rootListから削除する
							rootList.Remove(tmp);
						}
						//次に進む
						curNode = tmp;
					}
					nodeList.Add(curNode);
				}
			}
			oldCreationNumber = curCreationNumber;
			attemptTreeList.Add(nodeList);
		}
	}

	public void ConvertLayerGraph(){
		int layer = 0;
		List<IBNode>[] layers;
		int i, j;
		//layerとposition, priorityをリセット
		foreach(IBNode root in rootList){
			root.ClearArrangeRecursively();
		}
		//ノードにレイヤーを設定、戻り値で最大レイヤーの取得
		foreach(IBNode root in rootList){
			layer = root.SetLayerRecursively(0);
		}

		if (layer == 0){
			return ;
		}
		//ダミーノードの追加
		InsertDummyNodes();

		layers = new List<IBNode>[layer+1];
		for(i = 0 ; i < layers.Length ; i++){
			layers[i] = new List<IBNode>();
		}
		//レイヤー毎にノードを取得する
		DAGNode.AssignNodeRecursively(rootList, layers);

		//重心法でレイヤー内の頂点順番決定
		CenterOfGravityMethod(layers);

		//レイヤー毎の頂点数を記憶
		int maxWidth = 0;
		int[] layerNum = new int[layer+1];
		for(i = 0 ; i < layerNum.Length ; i++){
			layerNum[i] = layers[i].Count;
			if (maxWidth < layerNum[i]){
				maxWidth = layerNum[i];
			}
		}
		//Debug.Log("layers:"+(layer+1)+" width:"+maxWidth);
		//Debug.Log("layers.Length:"+layers.Length);
		//layersを二次元配列に変換
		arrangeMap = new IBNode[layer+1][];
		for(i = 0 ; i < arrangeMap.Length ; i++){
			arrangeMap[i] = new IBNode[maxWidth*3];
		}
		//Debug.Log("arrangeMap "+arrangeMap.Length+", "+arrangeMap[0].Length);
		for(i = 0 ; i < layers.Length ; i++){
			j = 0;
			foreach(IBNode node in layers[i]){
				//maxWidthだけoffsetをかける
				arrangeMap[i][j+maxWidth] = layers[i][j];
				layers[i][j].SetPosition(j+maxWidth);
				j++;
			}
		}

		//優先度を記録
		for(i = 0 ; i < arrangeMap.Length ; i++){
			for(j = 0 ; j < arrangeMap[i].Length;  j++){
				IBNode curNode = arrangeMap[i][j];
				if (curNode != null){
					//dummy判定
					if(((DAGNode)curNode).gameObject.tag.Equals(DUMMYNODETAG)){
						//dummyの場合、最大優先度を与える
						curNode.SetUpPriority(maxWidth+1);
						curNode.SetDownPriority(maxWidth+1);
					}else{
						//dummyでない場合
						curNode.SetUpPriority(curNode.GetParentEdgeList().Count);
						curNode.SetDownPriority(curNode.GetChildEdgeList().Count);
					}
				}
			}
		}

		//優先法で頂点配置の決定
		PriorityMethod(arrangeMap, layers);

		DrawLayerGraph(arrangeMap);
	}

	private void DontFocusAll(){
		nodeSet.DontFocusAll();
		edgeSet.DontFocusAll();
	}

	private void ShowAT(){
		//Debug.Log("showAT :"+showIndex + ", num="+attemptTreeList[showIndex].Count);
		List<IBNode> at = attemptTreeList[showIndex];
		IBNode past = null;

		DontFocusAll();

		foreach(IBNode node in at){
			//Debug.Log("node:"+node.GetData().GetID());
			((DAGNode)node).Focus();
			IBNodeData data = node.GetData();
			//Debug.Log("node:Data:"+data.GetID()+" node.GetLayer() ==> " + node.GetLayer());
			if (past != null && (node.GetLayer() - past.GetLayer() > 1)){
				DAGEdge targetEdge = null;
				foreach(IBEdge e in past.GetChildEdgeList()){
					//Debug.Log("e.GetDescendantNode().GetData().GetID() == -data.GetID() ==> " + e.GetDescendantNode().GetData().GetID() + " == " + -data.GetID());
					if (e.GetDescendantNode().GetData().GetID() == -data.GetID()){
						//Debug.Log("taretEdge found");
						targetEdge = e as DAGEdge;
						break;
					}
				}
				targetEdge.Focus();
				DAGNode dNode = targetEdge.GetDescendantNode() as DAGNode;

				while(dNode.gameObject.tag.Equals(DUMMYNODETAG)){
					targetEdge = dNode.GetChildEdgeList()[0] as DAGEdge;
					targetEdge.Focus();
					dNode = targetEdge.GetDescendantNode() as DAGNode;
				}
			}else if (past != null){
				foreach(IBEdge e in past.GetChildEdgeList()){
					if (e.GetDescendantNode().GetData().GetID() == data.GetID()){
						((DAGEdge)e).Focus();
						break;
					}
				}
			}
			past = node;
		}
	}


	private void DrawLayerGraph(IBNode[][] arrangeMap){
		for(int i = 0 ; i < arrangeMap.Length ; i++){
			for(int j = 0 ; j < arrangeMap[i].Length ; j++){
				if (arrangeMap[i][j] != null){
					DAGNode node = (DAGNode)arrangeMap[i][j];
					node.Draw();
					node.transform.localPosition = new Vector3(node.GetPosition(), node.GetLayer(), 0.0f);
				}
			}
		}
		edgeSet.DrawEdges();
	}

	private void InsertDummyNodes(){
		foreach(IBNode node in rootList){
			InsertDymmyNode(node);
		}
	}
	private void InsertDymmyNode(IBNode node){
		if(((DAGNode)node).gameObject.tag.Equals(DUMMYNODETAG)){
			return;
		}
		foreach(IBEdge e in node.GetChildEdgeList()){
			InsertDymmyNode(e.GetDescendantNode());
		}

		List<IBEdge> edgeList = new List<IBEdge>(node.GetChildEdgeList());
		List<IBEdge> remList = new List<IBEdge>();
		foreach(IBEdge e in edgeList){
			IBNode childNode = e.GetDescendantNode();
			if (((DAGNode)childNode).gameObject.tag.Equals(DUMMYNODETAG)){
				continue;
			}
			int diff = childNode.GetLayer() - node.GetLayer();

			if (diff > 1){
				IBNode curParent = node;
				for(int i = 0 ; i < diff - 1 ; i++){
					if (i == diff - 2){
						curParent = nodeSet.MakeDummyNode(new BNodeData(-childNode.GetData().GetID()), curParent, childNode);
					}else{
						curParent = nodeSet.MakeDummyNode(new BNodeData(-childNode.GetData().GetID()), curParent, null);
					}
					curParent.SetLayer(node.GetLayer() + i + 1);
				}
				remList.Add(e);
			}
		}
		node.RemoveEdge(remList);
	}

	private void CenterOfGravityMethod(List<IBNode>[] layers){
		int c = 0;
		//初期値の設定
		for(int i = 0 ; i < layers.Length ; i++){
			int j = 0;
			foreach(IBNode node in layers[i]){
				node.SetPosition(j);
				j++;
			}
		}
		while(c < ITERATION_COG){
			COGDown(layers);
			COGUp(layers);
			c++;
		}
	}

	private void COGDown(List<IBNode>[] layers){
		for(int i = 1 ; i < layers.Length ; i++){
			layers[i] = layers[i]
				.OrderBy(node => node.CalcUpperCOG())
				.Where((node, index) => node.SetPosition(index))
				.ToList();
		}
	}

	private void COGUp(List<IBNode>[] layers){
		for(int i = layers.Length - 2 ; i >= 0 ; i--){
			layers[i] = layers[i]
				.OrderBy(node => node.CalcLowerCOG())
				.Where((node, index) => node.SetPosition(index))
				.ToList();
		}
	}

	private void PriorityMethod(IBNode[][] arrangeMap, List<IBNode>[] layers){
		IBNode[][] downPriorityMap = new IBNode[layers.Length][];
		IBNode[][] upPriorityMap = new IBNode[layers.Length][];

		for(int i = 0 ; i < layers.Length ; i++){
			downPriorityMap[i] = layers[i].OrderByDescending(node => node.GetDownPriority()).ToArray();
			upPriorityMap[i] = layers[i].OrderByDescending(node => node.GetUpPriority()).ToArray();
		}

		int c = 0;
		while(c < ITERATION_PRIORITY){
			PriorityDown(downPriorityMap, arrangeMap);
			PriorityUp(upPriorityMap, arrangeMap);
			c++;
		}
	}

	private void PriorityDown(IBNode[][] downPriorityMap, IBNode[][] arrangeMap){
		for(int i = 1 ; i < downPriorityMap.Length ; i++){
			for(int j = 0 ; j < downPriorityMap[i].Length ; j++){
				IBNode node = downPriorityMap[i][j];
				float cog = node.CalcUpperCOG();
				int pos = (int)Math.Round(cog, 0, MidpointRounding.AwayFromZero);

				int shift = pos - node.GetPosition();
				if (shift == 0){
					continue;
				}else{
					BestEffortShift(arrangeMap[i], node, shift, true);
				}
			}
		}
	}

	private void PriorityUp(IBNode[][] upPriorityMap, IBNode[][] arrangeMap){
		for(int i = upPriorityMap.Length - 2; i >= 0 ; i--){
			for(int j = 0 ; j < upPriorityMap[i].Length && upPriorityMap[i][j] != null; j++){
				IBNode node = upPriorityMap[i][j];
				float cog = node.CalcLowerCOG();
				int pos = (int)Math.Round(cog, 0, MidpointRounding.AwayFromZero);

				int shift = pos - node.GetPosition();
				if (shift == 0){
					continue;
				}else{
					BestEffortShift(arrangeMap[i], node, shift, false);
				}
			}
		}
	}

	private void showArr(IBNode[] arr, bool isDown){
		for(int i = 0 ; i < arr.Length ; i++){
			if(arr[i] == null){
				Debug.Log(i+":"+"n");
			}else{
				if (isDown){
					Debug.Log(i+":"+arr[i].GetDownPriority());
				}else{
					Debug.Log(i+":"+arr[i].GetUpPriority());
				}
			}
		}
	}

	private bool BestEffortShift(IBNode[] arr, IBNode node, int shift, bool isDown){
		int pos = node.GetPosition();
		int pri ;
		if (isDown){
			pri = node.GetDownPriority();
		}else{
			pri = node.GetUpPriority();
		}
		int iteDir;
		int emptyCount = 0;
		int index = 0;
		int lastIndex = -1;

		if(shift < 0){
			iteDir = -1;
		}else{
			iteDir = 1;
		}
		//showArr(arr, isDown);
		//Debug.Log("node.pos:"+pos+", shift:"+shift+", isDown:"+isDown+", iteDir:"+iteDir);

		index = pos + iteDir;
		while(emptyCount < Math.Abs(shift) && index >= 0 && index < arr.Length){
			if(arr[index] == null){
				emptyCount++;
				lastIndex = index;
			}else{
				if (isDown){
					if(arr[index].GetDownPriority() > pri){
						return false;
					}
				}else{
					if(arr[index].GetUpPriority() > pri){
						return false;
					}
				}
			}

			index += iteDir;
		}

		//Debug.Log("emptyCount:"+emptyCount+", lastIndex:"+lastIndex);

		//シフトできない場合
		if (emptyCount < Math.Abs(shift)){
			return false;
		}

		//シフトする
		index = lastIndex;
		int emptyIndex = lastIndex;
		while(arr[pos] != null){
			//データのある場所まで移動
			while(arr[index] == null){
				index -= iteDir;
			}
			//データのない場所まで移動
			while(arr[emptyIndex] != null){
				emptyIndex -= iteDir;
			}

			//データのある場所のデータからデータのない場所へ移動
			arr[emptyIndex] = arr[index];
			arr[index] = null;
			arr[emptyIndex].SetPosition(emptyIndex);

			//次に進める
			emptyIndex -= iteDir;
			index -= iteDir;
		}
		return true;
	}

    public void Remove(){
    	/*
    	if ()
		foreach(IBEdge eParent in parentEdgeList){
			IBNode parent = eParent.GetAnsectorNode();
			foreach(IBEdge eChild in childEdgeList){
				IBNode child = eChild.GetDescendantNode();
				parent.AddChild(child);
			}
		}
		foreach(IBEdge e in parentEdgeList){
			e.GetAnsectorNode().GetChildEdgeList().Remove(e);
		}
		parentEdgeList.Clear();

		foreach(IBEdge e in childEdgeList){
			e.GetDescendantNode().GetParentEdgeList().Remove(e);
		}
		childEdgeList.Clear();

		Destory(this.gameObject);*/
	}
/*
	public void InsertParent(IBNodeData data){
		if (currentTarget == null){
    		return ;
    	}

    	GameObject obj = Instantiate(nodePrefab, nodeSet.transform);
    	DAGNode parent = obj.GetComponent<DAGNode>();
    	if (parent != null){
    		parent.SetData(data);
    		foreach(IBEdge e in currentTarget.GetParentEdgeList()){
				e.SetDescendantNode(parent);
			}
			parent.GetParentEdgeList().AddRange(currentTarget.GetParentEdgeList());
			currentTarget.GetParentEdgeList().Clear();
			IBEdge edge = new BEdge(parent, currentTarget);
			parent.GetChildEdgeList().Add(edge);
			currentTarget.GetParentEdgeList().Add(edge);
    	}
	}*/
}
