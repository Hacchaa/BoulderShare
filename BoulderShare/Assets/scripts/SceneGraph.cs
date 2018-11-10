using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneGraph : MonoBehaviour {
	private SGNode root;
	[SerializeField]
	private GameObject sGNodePrefab;
	private GameObject nodeSet;

	void Start(){
		nodeSet = GameObject.FindGameObjectsWithTag("SGNodeSet")[0];
		root = Instantiate(sGNodePrefab, nodeSet.transform).GetComponent<SGNode>();

	}
}
