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
	private int m_current_grid_row=0;
	private int m_current_grid_col=0;
	
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
	
	//check if row,col is valid in the current map
	private bool IsInBound(int row, int col) 
	{
		int maxCol=GameObject.Find("GridRenderer").GetComponent<GridGenerator>().m_gridNumHor;
		int maxRow=GameObject.Find("GridRenderer").GetComponent<GridGenerator>().m_gridNumVer;
		maxCol = (row%2==0) ? maxCol : maxCol-1;
      	return (row>=0 && row<maxRow && col>=0 && col<maxCol);
	}
	
	private bool CanPass(int row, int col)
	{
		return m_grids[row][col].GetComponent<HexGridModel>().CanPass("");
	}
	
	private GameObject GetTopLeft(GameObject grid) 
	{
		int row = grid.GetComponent<HexGridModel>().m_row;
		int col = grid.GetComponent<HexGridModel>().m_col;
		col = (row%2 == 0)?col-1:col;
		row = row-1;
		if(IsInBound(row,col) && CanPass(row,col))
			return m_grids[row][col];
		else
			return null;
	}
		
	private GameObject GetTopRight(GameObject grid) 
	{
		int row = grid.GetComponent<HexGridModel>().m_row;
		int col = grid.GetComponent<HexGridModel>().m_col;
		col = (row%2 == 0)?col:col+1;
		row = row-1;
		if(IsInBound(row,col) && CanPass(row,col))
			return m_grids[row][col];
		else
			return null;
	}
		
	private GameObject GetLeft(GameObject grid) 
	{
		int row = grid.GetComponent<HexGridModel>().m_row;
		int col = grid.GetComponent<HexGridModel>().m_col;
		col = col-1;
		if(IsInBound(row,col) && CanPass(row,col))
			return m_grids[row][col];
		else
			return null;
	}
		
	private GameObject GetRight(GameObject grid) {
		int row = grid.GetComponent<HexGridModel>().m_row;
		int col = grid.GetComponent<HexGridModel>().m_col;
		col = col+1;
		if(IsInBound(row,col) && CanPass(row,col))
			return m_grids[row][col];
		else
			return null;
	}
		
	private GameObject GetBottomLeft(GameObject grid) {
		int row = grid.GetComponent<HexGridModel>().m_row;
		int col = grid.GetComponent<HexGridModel>().m_col;
		col = (row%2 == 0)?col-1:col;
		row = row+1;
		if(IsInBound(row,col) && CanPass(row,col))
			return m_grids[row][col];
		else
			return null;
	}
		
	private GameObject GetBottomRight(GameObject grid) {
		int row = grid.GetComponent<HexGridModel>().m_row;
		int col = grid.GetComponent<HexGridModel>().m_col;
		col = (row%2 == 0)?col:col+1;
		row = row+1;
		if(IsInBound(row,col) && CanPass(row,col))
			return m_grids[row][col];
		else
			return null;
	}
	
	/*
	public class Node:IEquatable<Node>, IComparable<Node> {
	    public float Cost_So_Far;
	    public double Cost_Remaining;
		public List<List<int>> Past_Nodes;
	
    	public Node(int x,int y,float cost_so_far,double cost_remaining, List<List<int>> past_nodes) {
	        this.X=x;
	        this.Y=y;
			this.Cost_So_Far=cost_so_far;
			this.Cost_Remaining=cost_remaining;
			this.Past_Nodes=past_nodes;
			this.Past_Nodes.Add(new List<int>(){x,y});
	    }
		
	    public bool Equals(Node other) {
	        return (this.X==other.X && this.Y==other.Y);
	    }f
			
		public int CompareTo(Node other) {
	        return Cost_Remaining.CompareTo(other.Cost_Remaining);
	    }
	}
	*/
	
	//reset certain fields in hexgridmodel 
	//to run BFS and A* path finding
	private void ResetAllGraphStateVars()
	{
		foreach(List<GameObject> a in m_grids)
			foreach(GameObject b in a)
				b.GetComponent<HexGridModel>().ResetGraphStateVars();
	}
	
	//find the range of movement and highlight those grids
	public void HighlightMovementRange(int row, int col, int movement)
	{
		//clear state variables
		ResetAllGraphStateVars();
		//list of nodes to check
		List<GameObject> openList=new List<GameObject>();
		//list of nodes already checked
		List<GameObject> closedList=new List<GameObject>();
		
		GameObject currentNode=m_grids[row][col];
		//set movementleft
		currentNode.GetComponent<HexGridModel>().InitMovementLeft(movement);
		
		//check if first node is valid
		if (IsInBound(row,col))
			openList.Add(currentNode);
		
		//run BFS
		while (openList.Count!=0) {
			//sort from largest movement left to smallest movementleft
			openList.Sort(
				delegate (GameObject a, GameObject b) 
   				{
					int mla = a.GetComponent<HexGridModel>().movementLeft;
					int mlb = b.GetComponent<HexGridModel>().movementLeft;
					return mlb.CompareTo(mla); //order matters
				} 
			);
			currentNode=openList[0];
			//Debug.Log("Current node: "+currentNode.ToString());
			openList.RemoveAt(0);
			int movementLeft = currentNode.GetComponent<HexGridModel>().movementLeft;
			if (!closedList.Contains(currentNode) && movementLeft >= 0) {
				closedList.Add(currentNode);
				//turn on this hexgrid
				currentNode.GetComponent<MaskManager>().GreenMaskOn();	
				//if movement is 0, stop here
				if(movementLeft > 0){
					List<GameObject> currentNeighbours=GetNeighbours(currentNode);
					foreach (GameObject n in currentNeighbours) {
						//update cost
						n.GetComponent<HexGridModel>().UpdateMovementLeft(movementLeft);
						//if openlist already contains this node, the cost will be automatically updated
						//since openlist only contains a pointer to the gameobject
						if (!openList.Contains(n)) 
							openList.Add(n);
					}
				}
			}
		}
	}
	
	//implement : return a list of path instead of just highlighting them
	//find path from source to destination
	public void HighlightMovementPath(GameObject src, GameObject dest) 
	{
		ResetAllGraphStateVars();
		List<GameObject> openList=new List<GameObject>();
		List<GameObject> closedList=new List<GameObject>();
		
		int srcRow = src.GetComponent<HexGridModel>().m_row;
		int srcCol = src.GetComponent<HexGridModel>().m_col;
		
		GameObject currentNode = m_grids[srcRow][srcCol];
		currentNode.GetComponent<HexGridModel>().InitDistToDest(dest);
		currentNode.GetComponent<HexGridModel>().m_prevNode = null;
		
		//make sure src node is valid
		if (IsInBound(srcRow,srcCol))
			openList.Add(currentNode);
		
		//run A* path finding
		while (openList.Count!=0) 
		{
			//change later, dont have to use current node
			//to indicate if reachable
			if (openList.Contains(dest)){
				currentNode = dest;
				break;
			}
			openList.Sort(
				delegate (GameObject a, GameObject b) 
   				{
					float mla = a.GetComponent<HexGridModel>().distToDest;
					float mlb = b.GetComponent<HexGridModel>().distToDest;
					return mla.CompareTo(mlb); //order matters
				} 
			);
			currentNode=openList[0];
			openList.RemoveAt(0);
			//terminate if destination is reached
			//dijkstra like shortest path finding
			closedList.Add(currentNode);
			List<GameObject> currentNeighbours=GetNeighbours(currentNode);
			foreach (GameObject n in currentNeighbours) {
				n.GetComponent<HexGridModel>().InitDistToDest(dest);
				/*if (openList.Contains(n)) {
					*
					 * why can this happen? your cost remaining will always be the same isnt it?
					 * seems it is just the distance from this node to dest node
					if (n.Cost_Remaining>openList.Find(n2=>n2.Equals(n)).Cost_Remaining) {
						openList.Remove(n);
					 *contains already does a search, whats the purpose of doing binary search again?
					 *duplicate node?
						int index=openList.BinarySearch(n);
						if (index<0)
							openList.Insert(~index,n);
						else
							openList.Insert(index,n);
					}*
				}*/
				if(!openList.Contains(n)&&!closedList.Contains(n)){
					n.GetComponent<HexGridModel>().m_prevNode = currentNode;
					openList.Add(n);
				}
			}
			currentNode=null;
		}
		//turn on path

		if (currentNode!=null){
			while(currentNode.GetComponent<HexGridModel>().m_prevNode != null){
				currentNode.GetComponent<MaskManager>().BlueMaskOn();
				currentNode = currentNode.GetComponent<HexGridModel>().m_prevNode;
			}
		}
		//turn on dest node
		dest.GetComponent<MaskManager>().RedMaskOn();
	}
	
	void Update () {
		if(Input.GetButtonDown("Fire1")) {
			m_click_number=(m_click_number+1)%2;
			RaycastHit grid;
			Ray selection = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(selection,out grid)){				
				if (m_click_number==1) { //BFS
					Debug.Log("Clicking on: "+grid.collider.gameObject.GetComponent<HexGridModel>().m_row.ToString()
										+", "+grid.collider.gameObject.GetComponent<HexGridModel>().m_col.ToString());
					m_current_grid_row=grid.collider.gameObject.GetComponent<HexGridModel>().m_row;
					m_current_grid_col=grid.collider.gameObject.GetComponent<HexGridModel>().m_col;
					ClearAllMasks();
					HighlightMovementRange(m_current_grid_row, m_current_grid_col, 3);
					grid.collider.gameObject.GetComponent<MaskManager>().RedMaskOn();
				}
				else { //A*
					Debug.Log("Destination: "+grid.collider.gameObject.GetComponent<HexGridModel>().m_row.ToString()
										+", "+grid.collider.gameObject.GetComponent<HexGridModel>().m_col.ToString());
					HighlightMovementPath(m_grids[m_current_grid_row][m_current_grid_col],grid.collider.gameObject);
				}
			}
		}
	}
}
