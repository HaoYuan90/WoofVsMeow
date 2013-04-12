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
	
	public GameObject GetBuildingAt(int row, int column)
	{
		//this method is made to facilitate RPC calls, it should never return null
		return m_grids[row][column].GetComponent<TnGAttribute>().m_building;
	}
	
	public GameObject GetUnitAt(int row, int column)
	{
		//this method is made to facilitate RPC calls, it should never return null
		return m_grids[row][column].GetComponent<TnGAttribute>().m_unit;
	}
	
	public GameObject GetGridAt(int row, int column)
	{
		//this method is made to facilitate RPC calls, it should never return null
		return m_grids[row][column];
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
	private void ResetAllGraphStateVars(bool considerTerrain)
	{
		foreach(List<GameObject> a in m_grids)
			foreach(GameObject b in a)
				b.GetComponent<HexGridModel>().ResetGraphStateVars(considerTerrain);
	}
	
	private void SetMovementCostAroundEnemies(GameObject src)
	{
		UnitController unit = src.GetComponent<TnGAttribute>().m_unit.GetComponent<UnitController>();
		int control = unit.m_control;
		//cost around enemy units
		foreach(Transform t in GameObject.Find("Units").transform)
		{
			UnitController temp = t.gameObject.GetComponent<UnitController>();
			int tempControl = temp.m_control;
			//is enemy, make grids near him harder to move about
			if(tempControl!=control){
				List<GameObject> neighbours = GetNeighbours(temp.currentGrid);
				foreach(GameObject e in neighbours){
					e.GetComponent<HexGridModel>().UpdateEnemyBlockageCost();
				}
			}
		}
		//cost around enemy buildings
		foreach(Transform t in GameObject.Find("Buildings").transform)
		{
			BuildingController temp = t.gameObject.GetComponent<BuildingController>();
			int tempControl = temp.m_control;
			//is enemy building, make grids near him harder to move about
			//neutral buildings have no effect
			if(tempControl!=control && tempControl!= -1){
				List<GameObject> neighbours = GetNeighbours(temp.currentGrid);
				foreach(GameObject e in neighbours){
					e.GetComponent<HexGridModel>().UpdateEnemyBlockageCost();
				}
			}
		}
	}
	
	//find the range of movement and highlight those grids
	//also formulate a path from src to all grids in range
	public void ProcessMovementRange(GameObject src, int movement, bool isRPC)
	{
        //clear state variables
        ResetAllGraphStateVars(true);
		SetMovementCostAroundEnemies(src);
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
				//if function is RPC, then no masks should be added
				if(!isRPC){
					if(currentNode.GetComponent<TnGAttribute>().m_unit == null
						&&currentNode.GetComponent<TnGAttribute>().m_building == null){
						currentNode.GetComponent<MaskManager>().GreenMaskOn();
					}
				}
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
								if(n.GetComponent<HexGridModel>().CanPass(thisControl,false) && 
									Math.Abs(srcHeight-destHeight)<=5){
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
        }
	}
	
	public void ProcessAttackRange(GameObject src, int range, bool isRPC)
	{
		//raycast based attack range processing
        //clear state variables
        ResetAllGraphStateVars(false);
		 //all nodes that are potentially in range of attack
        List<GameObject> nodeList = new List<GameObject>();
		//use this to get the node list
		List<GameObject> openList = new List<GameObject>();
        //list of nodes that can be reached via attack
        List<GameObject> validList = new List<GameObject>();
		
		GameObject currentNode = src;
		//the player this unit belong to
		int thisControl = currentNode.GetComponent<TnGAttribute>().
			m_unit.GetComponent<UnitController>().m_control;
		currentNode.GetComponent<HexGridModel>().m_movementLeft = range;
        openList.Add(currentNode);

        //run BFS
        while (openList.Count != 0)
        {
            currentNode = openList[0];
            openList.RemoveAt(0);
            int rangeLeft = currentNode.GetComponent<HexGridModel>().m_movementLeft;
			
            if (!nodeList.Contains(currentNode) && rangeLeft >= 0)
            {	
                nodeList.Add(currentNode);
                //if range left is 0, stop here
                if (rangeLeft > 0)
                {
                    List<GameObject> currentNeighbours = GetNeighbours(currentNode);
                    foreach (GameObject n in currentNeighbours)
                    {
						n.GetComponent<HexGridModel>().UpdateRangeLeft(currentNode,rangeLeft-1);
                        if (!openList.Contains(n))
                            openList.Add(n);
                    }
                }
			}
        }
		//remove itself
		nodeList.Remove (src);
		//run a ray cast from src to all nodes in node list, if not intersected, then node is valid
		//the following determines how high is considered to be an obstacle (tngattribute.m_height is related to this)
		float rayHeight = 4.5f+src.renderer.bounds.max.y; 
		Vector3 rayOrigin = new Vector3(src.transform.position.x, rayHeight,src.transform.position.z);
		foreach(GameObject node in nodeList)
		{
			Vector3 rayDest = new Vector3(node.transform.position.x, rayHeight,node.transform.position.z);
			float rayLen = (rayOrigin-rayDest).magnitude;
			Vector3 rayDir = rayDest-rayOrigin;
			Debug.DrawRay(rayOrigin,rayDir,Color.red);
			if(!Physics.Raycast(rayOrigin,rayDir,rayLen)){
				validList.Add(node);
			}
		}
        //clear state variables
        ResetAllGraphStateVars(false);
		foreach(GameObject node in validList)
		{
			if(!isRPC){
				if(node.GetComponent<TnGAttribute>().m_unit == null
					&&node.GetComponent<TnGAttribute>().m_building == null){
					node.GetComponent<MaskManager>().RedMaskOn();
				}
				else if(node.GetComponent<TnGAttribute>().m_unit != null)
				{
					GameObject tarUnit = node.GetComponent<TnGAttribute>().m_unit;
					if(thisControl != tarUnit.GetComponent<UnitController>().m_control){
						node.GetComponent<MaskManager>().DarkRedMaskOn();
						node.GetComponent<HexGridModel>().m_prevNode = src;
					}
				}
				else if(node.GetComponent<TnGAttribute>().m_building != null)
				{
					GameObject tarBuilding = node.GetComponent<TnGAttribute>().m_building;
					if(thisControl != tarBuilding.GetComponent<BuildingController>().m_control){
						node.GetComponent<MaskManager>().DarkRedMaskOn();
						node.GetComponent<HexGridModel>().m_prevNode = src;
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
	
	public void ProcessProductionRange(GameObject src) 
	{
		ResetAllGraphStateVars(false);
		List<GameObject> currentNeighbours = GetNeighbours(src);
		foreach (GameObject n in currentNeighbours) {
			if (n.GetComponent<TnGAttribute>().m_unit == null) {
				n.GetComponent<MaskManager>().BlueMaskOn();
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
