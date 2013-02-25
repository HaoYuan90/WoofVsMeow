using UnityEngine;
using System.Collections.Generic;

//this class handles logic related to gridmap 
//and is in charge of lighting the grids up 
//(turing masks on and off)

public class GridLogic : MonoBehaviour {
	
	private List<List<GameObject>> m_grids;
	
	private void InitGridData()
	{
		//Look for grid renderer game object in scene and get grid data
		m_grids = GameObject.Find("GridRenderer").GetComponent<GridGenerator>().GetGridData();
	}
	
	private void CheckGridData()
	{
		foreach(List<GameObject> a in m_grids)
			foreach(GameObject b in a)
				Debug.Log (b.GetComponent<HexGridModel>());
	}
	
	private void ClearAllMasks()
	{
		foreach(List<GameObject> a in m_grids)
			foreach(GameObject b in a)
				b.GetComponent<MaskManager>().OutlineMaskOn();
	}
	
	// Use this for initialization
	void Start () {
		InitGridData();
		//CheckGridData();
	}
	
	
	
	void Update () {
		if(Input.GetButtonDown("Fire1"))
		{
			RaycastHit grid;
			Ray selection = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(selection,out grid)){
				//the only object taking in ray cast should be grids
				grid.collider.gameObject.GetComponent<MaskManager>().RedMaskOn();
			}
		}
	}
}
