using UnityEngine;
using System.Collections.Generic;
using System;

//this class handles logic related to gridmap 
//and is in charge of lighting the grids up 
//(turing masks on and off)

public class GridLogic : MonoBehaviour {
	
	private List<List<GameObject>> m_grids;
	
	//temp used, debatable
	private int m_click_number=0;
	private GameObject currentSelectedGrid=null;
	
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
	
	//methods to find neighbour of a grid
	
	List<GameObject> GetNeighbours(GameObject grid) 
	{
		//implement : if grid cannot be passed
		List<GameObject> neighbours=new List<GameObject>();
		GameObject n;
		n=GetTopLeft(grid);     if (n!=null) neighbours.Add(n);
		n=GetTopRight(grid);    if (n!=null) neighbours.Add(n);
		n=GetLeft(grid);        if (n!=null) neighbours.Add(n);
		n=GetRight(grid);       if (n!=null) neighbours.Add(n);
		n=GetBottomLeft(grid);  if (n!=null) neighbours.Add(n);
		n=GetBottomRight(grid); if (n!=null) neighbours.Add(n);
		return neighbours;
	}
	
	//return the GameObject grid if row,col is in bound
	private GameObject GetGrid(int row, int col) 
	{
		int maxCol=GameObject.Find("GridRenderer").GetComponent<GridGenerator>().m_gridNumHor;
		int maxRow=GameObject.Find("GridRenderer").GetComponent<GridGenerator>().m_gridNumVer;
		maxCol = (row%2==0) ? maxCol : maxCol-1;
      	if (row>=0 && row<maxRow && col>=0 && col<maxCol)
			return m_grids[row][col];
		else
			return null;
	}
	
	private GameObject GetTopLeft(GameObject grid) 
	{
		int row = grid.GetComponent<HexGridModel>().m_row;
		int col = grid.GetComponent<HexGridModel>().m_col;
		col = (row%2 == 0)?col-1:col;
		row = row-1;
		return GetGrid(row,col);
	}
		
	private GameObject GetTopRight(GameObject grid) 
	{
		int row = grid.GetComponent<HexGridModel>().m_row;
		int col = grid.GetComponent<HexGridModel>().m_col;
		col = (row%2 == 0)?col:col+1;
		row = row-1;
		return GetGrid(row,col);
	}
		
	private GameObject GetLeft(GameObject grid) 
	{
		int row = grid.GetComponent<HexGridModel>().m_row;
		int col = grid.GetComponent<HexGridModel>().m_col;
		col = col-1;
		return GetGrid(row,col);
	}
		
	private GameObject GetRight(GameObject grid) {
		int row = grid.GetComponent<HexGridModel>().m_row;
		int col = grid.GetComponent<HexGridModel>().m_col;
		col = col+1;
		return GetGrid(row,col);
	}
		
	private GameObject GetBottomLeft(GameObject grid) {
		int row = grid.GetComponent<HexGridModel>().m_row;
		int col = grid.GetComponent<HexGridModel>().m_col;
		col = (row%2 == 0)?col-1:col;
		row = row+1;
		return GetGrid(row,col);
	}
		
	private GameObject GetBottomRight(GameObject grid) {
		int row = grid.GetComponent<HexGridModel>().m_row;
		int col = grid.GetComponent<HexGridModel>().m_col;
		col = (row%2 == 0)?col:col+1;
		row = row+1;
		return GetGrid(row,col);
	}

	//reset certain fields in hexgridmodel 
	//to run BFS and A* path finding
	private void ResetAllGraphStateVars()
	{
		foreach(List<GameObject> a in m_grids)
			foreach(GameObject b in a)
				b.GetComponent<HexGridModel>().ResetGraphStateVars();
	}
	
	//find the range of movement and highlight those grids
	public void HighlightMovementRange(GameObject src, float movement)
	{
		ResetAllGraphStateVars();
		List<GameObject> openList=new List<GameObject>(); //list of nodes to be visited, from largest movementLeft to smallest
		List<GameObject> closedList=new List<GameObject>(); //visited nodes
		
		GameObject currentNode=src;
		currentNode.GetComponent<HexGridModel>().SetMovementLeft(movement);
		openList.Add(currentNode);
		
		while (openList.Count!=0) {
			currentNode=openList[0];
			openList.RemoveAt(0);
			closedList.Add(currentNode);
			currentNode.GetComponent<MaskManager>().GreenMaskOn(); //highlight the current grid which is reachable
			
			List<GameObject> currentNeighbours=GetNeighbours(currentNode);
			foreach (GameObject n in currentNeighbours) {
				if (!closedList.Contains(n)) {//Only process unvisited nodes
					n.GetComponent<HexGridModel>().UpdateMovementLeft(currentNode); //update movementLeft, thus need to update the order of the list later
					if (n.GetComponent<HexGridModel>().m_movementLeft>=0) { //only reachable grids will be added to openList
						openList.Remove(n);
						openList.Add(n);
					}
				}
			}
			
			openList.Sort(
				delegate (GameObject a, GameObject b) 
   				{
					float mla = a.GetComponent<HexGridModel>().m_movementLeft;
					float mlb = b.GetComponent<HexGridModel>().m_movementLeft;
					return mlb.CompareTo(mla); //order matters
				} 
			);
		}
	}
	
	//return a Grid list from source to destination
	public List<GameObject> HighlightMovementPath(GameObject src, GameObject dest) 
	{
		List<GameObject> pathList = new List<GameObject>();
		GameObject currentNode = dest;		
		while (currentNode != null) {
			pathList.Insert(0,currentNode);
			currentNode.GetComponent<MaskManager>().BlueMaskOn();
			currentNode=currentNode.GetComponent<HexGridModel>().m_prevNode;
		}
		src.GetComponent<MaskManager>().RedMaskOn();
		dest.GetComponent<MaskManager>().RedMaskOn();
		ResetAllGraphStateVars();
		return pathList;
	}
	
	void Update () {
		if(Input.GetButtonDown("Fire1")) {
			m_click_number=(m_click_number+1)%2;
			RaycastHit grid;
			Ray selection = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(selection,out grid)){				
				if (m_click_number==1) { //First click will show the movement range
					//Debug.Log("Clicking on: "+grid.collider.gameObject.GetComponent<HexGridModel>().m_row.ToString()
					//					+", "+grid.collider.gameObject.GetComponent<HexGridModel>().m_col.ToString());
					currentSelectedGrid=grid.collider.gameObject;
					ClearAllMasks();
					HighlightMovementRange(currentSelectedGrid, (float)3.0);
					grid.collider.gameObject.GetComponent<MaskManager>().RedMaskOn();
				}
				else { //Second Click will show the movement path
					//Debug.Log("Destination: "+grid.collider.gameObject.GetComponent<HexGridModel>().m_row.ToString()
					//					+", "+grid.collider.gameObject.GetComponent<HexGridModel>().m_col.ToString());
					HighlightMovementPath(currentSelectedGrid,grid.collider.gameObject);
				}
			}
		}
	}
}
