using UnityEngine;
using System.Collections.Generic;
using System;

//this class handles logic related to gridmap 
//and is in charge of lighting the grids up 
//(turing masks on and off)

public class GridLogic : MonoBehaviour {
	
	private List<List<GameObject>> m_grids;
	
	// Use this for initialization
	public void Initialise () {
		InitGridData();
		//CheckGridData();
	}
	
	private void InitGridData()
	{
		//Look for grid renderer game object in scene and get grid data
		GridGenerator gen = GameObject.Find("GridRenderer").GetComponent<GridGenerator>();
		m_grids = gen.GetGridData();
		//activate mask when game actually starts
		gen.m_maskActive = false;
		gen.ToggleMask();
	}
	
	public void ClearAllMasks()
	{
		foreach(List<GameObject> a in m_grids)
			foreach(GameObject b in a)
				b.GetComponent<MaskManager>().OutlineMaskOn();
	}
	
	//methods to find neighbour of a grid
	private List<GameObject> GetNeighbours(GameObject grid) 
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
	private GameObject TryGetGrid(int row, int col) 
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

		return TryGetGrid(row,col);
	}
		
	private GameObject GetTopRight(GameObject grid) 
	{
		int row = grid.GetComponent<HexGridModel>().m_row;
		int col = grid.GetComponent<HexGridModel>().m_col;
		col = (row%2 == 0)?col:col+1;
		row = row-1;

		return TryGetGrid(row,col);
	}
		
	private GameObject GetLeft(GameObject grid) 
	{
		int row = grid.GetComponent<HexGridModel>().m_row;
		int col = grid.GetComponent<HexGridModel>().m_col;
		col = col-1;

		return TryGetGrid(row,col);
	}
		
	private GameObject GetRight(GameObject grid) {
		int row = grid.GetComponent<HexGridModel>().m_row;
		int col = grid.GetComponent<HexGridModel>().m_col;
		col = col+1;

		return TryGetGrid(row,col);
	}
		
	private GameObject GetBottomLeft(GameObject grid) {
		int row = grid.GetComponent<HexGridModel>().m_row;
		int col = grid.GetComponent<HexGridModel>().m_col;
		col = (row%2 == 0)?col-1:col;
		row = row+1;

		return TryGetGrid(row,col);
	}
		
	private GameObject GetBottomRight(GameObject grid) {
		int row = grid.GetComponent<HexGridModel>().m_row;
		int col = grid.GetComponent<HexGridModel>().m_col;
		col = (row%2 == 0)?col:col+1;
		row = row+1;

		return TryGetGrid(row,col);
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
	//also formulate a path from src to all grids in range
	public void ProcessMovementRange(GameObject src, int movement)
	{
        //clear state variables
        ResetAllGraphStateVars();
        //list of nodes to check sorted from highest movementleft to lowest
        List<GameObject> openList = new List<GameObject>();
        //list of nodes already checked
        List<GameObject> closedList = new List<GameObject>();
		
		GameObject currentNode = src;
        currentNode.GetComponent<HexGridModel>().m_movementLeft = movement;
		//the player this unit belong to
		int thisControl = currentNode.GetComponent<TnGAttribute>().
			m_unit.GetComponent<UnitController>().m_control;
        openList.Add(currentNode);

        //run BFS
        while (openList.Count != 0)
        {
            //sort from largest movement left to smallest movementleft
            openList.Sort(
                delegate(GameObject a, GameObject b)
                {
                    int mla = a.GetComponent<HexGridModel>().m_movementLeft;
                    int mlb = b.GetComponent<HexGridModel>().m_movementLeft;
                    return mlb.CompareTo(mla); //order matters
                }
            );
            currentNode = openList[0];
            openList.RemoveAt(0);
            int movementLeft = currentNode.GetComponent<HexGridModel>().m_movementLeft;
			
            if (!closedList.Contains(currentNode) && movementLeft >= 0)
			{	
				closedList.Add(currentNode);
				 //turn on this hexgrid if there is no unit occupying it
				//if(currentNode.GetComponent<TnGAttribute>().m_unit == null){
					currentNode.GetComponent<MaskManager>().GreenMaskOn();
				//}
				if(movementLeft > 0)
				{
					int costToPass = currentNode.GetComponent<HexGridModel>().m_movementCost;
					List<GameObject> currentNeighbours = GetNeighbours(currentNode);
					foreach(GameObject n in currentNeighbours)
					{
						int srcHeight = currentNode.GetComponent<TnGAttribute>().m_height;
						int destHeight = n.GetComponent<TnGAttribute>().m_height;
						int costToJump = Math.Abs(destHeight-srcHeight)/3;
						int costToReach = costToPass + costToJump;
						//if closed list has n then
						//cannot move past n (enemy unit on or itself is an obstacle
						//or n has already been processed
						if(!closedList.Contains(n)){
							//n has already passed check, so do not have to check again
							if(openList.Contains (n)){
								n.GetComponent<HexGridModel>().UpdateMovementLeft(currentNode,costToReach);
							}
							else{
								if(n.GetComponent<HexGridModel>().CanPass(thisControl,false)){
									n.GetComponent<HexGridModel>().UpdateMovementLeft(currentNode,costToReach);
									openList.Add(n);
								}
								else{
									closedList.Add(n);
								}
							}
						}
					}
				}
			}
			/*
				//grid in the path must be empty or occupied by allies
            	if (currentNode.GetComponent<TnGAttribute>().m_unit == null
						|| (currentNode.GetComponent<TnGAttribute>().m_unit.GetComponent<UnitController>().m_control
						== currentNode.GetComponent<TnGAttribute>().m_unit.GetComponent<UnitController>().m_control)) {
	                closedList.Add(currentNode);
	                //turn on this hexgrid
					if (currentNode.GetComponent<TnGAttribute>().m_unit == null)
						if (currentNode.GetComponent<TnGAttribute>().m_unit == null)
	                		currentNode.GetComponent<MaskManager>().GreenMaskOn();
	                //if movement is 0, stop here
	                if (movementLeft > 0)
	                {
	                    List<GameObject> currentNeighbours = GetNeighbours(currentNode);
	                    foreach (GameObject n in currentNeighbours)
	                    {
	                        //update cost
	                        n.GetComponent<HexGridModel>().UpdateMovementLeft(currentNode);
	                        if (!openList.Contains(n))
	                            openList.Add(n);
	                    }
	                }*/
        }
	}
	
	public void ProcessAttackRange(GameObject src, int range)
	{
        //clear state variables
        ResetAllGraphStateVars();
        //list of nodes to check sorted from highest movementleft to lowest
        List<GameObject> openList = new List<GameObject>();
        //list of nodes already checked
        List<GameObject> closedList = new List<GameObject>();
		
		GameObject currentNode = src;
		int srcHeight = currentNode.GetComponentInChildren<TnGAttribute>().m_height;
		//the player this unit belong to
		int thisControl = currentNode.GetComponent<TnGAttribute>().
			m_unit.GetComponent<UnitController>().m_control;
		currentNode.GetComponent<HexGridModel>().m_movementLeft = range;
        openList.Add(currentNode);

        //run BFS
        while (openList.Count != 0)
        {
			openList.Sort(
                delegate(GameObject a, GameObject b)
                {
                    int mla = a.GetComponent<HexGridModel>().m_movementLeft;
                    int mlb = b.GetComponent<HexGridModel>().m_movementLeft;
                    return mlb.CompareTo(mla); //order matters
                }
            );
            currentNode = openList[0];
            openList.RemoveAt(0);
            int rangeLeft = currentNode.GetComponent<HexGridModel>().m_movementLeft;
            if (!closedList.Contains(currentNode) && rangeLeft >= 0)
            {
                closedList.Add(currentNode);
                //turn on this hexgrid
				if(currentNode.GetComponent<TnGAttribute>().m_unit == null){
					//lighter redmask to indicate valid range but no one to attack
					//TO CHANGE
					currentNode.GetComponent<MaskManager>().BlueMaskOn();
				}
				else if(thisControl != 
					currentNode.GetComponent<TnGAttribute>().m_unit.GetComponent<UnitController>().m_control)
				{
					//darker redmask to indicate can attack enemy
					currentNode.GetComponent<MaskManager>().RedMaskOn();
				}
				
                //if movement is 0, stop here
                if (rangeLeft > 0)
                {
                    List<GameObject> currentNeighbours = GetNeighbours(currentNode);
                    foreach (GameObject n in currentNeighbours)
                    {
						int destHeight = n.GetComponent<TnGAttribute>().m_height;
						
						if(destHeight - srcHeight >= 5) //dest a lot higher than src, hard to shoot up
                        	n.GetComponent<HexGridModel>().UpdateRangeLeft(currentNode,rangeLeft-2);
						else
							n.GetComponent<HexGridModel>().UpdateRangeLeft(currentNode,rangeLeft-1);
                        if (!openList.Contains(n))
                            openList.Add(n);
                    }
                }
			}
        }
	}
	
	//return a Grid list from source to destination
	public void HighlightMovementPath(GameObject src, GameObject dest) 
	{
		GameObject currentNode = dest;		
		while (currentNode != null) {
			currentNode.GetComponent<MaskManager>().BlueMaskOn();
			currentNode=currentNode.GetComponent<HexGridModel>().m_prevNode;
		}
		src.GetComponent<MaskManager>().RedMaskOn();
		dest.GetComponent<MaskManager>().RedMaskOn();
	}
	
	public void ProcessProductionRange(GameObject src) {
		List<GameObject> currentNeighbours = GetNeighbours(src);
		foreach (GameObject n in currentNeighbours) {
			if (n.GetComponent<TnGAttribute>().m_unit == null) {
				n.GetComponent<MaskManager>().RedMaskOn();
				n.GetComponent<HexGridModel>().m_prevNode = src;
			}
		}
	}
	
	
	public void InitUnitsAndBuildings(GameEngine engine)
	{
		foreach(List<GameObject> l in m_grids){
			foreach(GameObject e in l){
				if(e.GetComponent<TnGAttribute>().m_unit!= null){
					GameObject temp = e.GetComponent<TnGAttribute>().m_unit;
					temp.GetComponent<UnitController>().InitialiseUnit(engine,e);
				}
				if(e.GetComponent<TnGAttribute>().m_building!= null){
					GameObject temp = e.GetComponent<TnGAttribute>().m_building;
					temp.GetComponent<BuildingController>().InitialiseBuilding(engine,e);
				}
			}
		}
	}
	
	//debug method
	private void CheckGridData()
	{
		foreach(List<GameObject> a in m_grids)
			foreach(GameObject b in a)
				Debug.Log (b.GetComponent<HexGridModel>());
	}
	
	/* legacy A* path finding
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
	}*/
}
