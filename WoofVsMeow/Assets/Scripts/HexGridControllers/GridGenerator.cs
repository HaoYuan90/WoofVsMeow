using UnityEngine;
using System.Collections.Generic;
using System;

[ExecuteInEditMode]
// this class generator a map of hex grids in the following pattern
/*
 *   ======== width ========= |
 *   * * * * * * * * * * * *  |
 *    * * * * * * * * * * *   length
 *   * * * * * * * * * * * *  |
 *    * * * * * * * * * * *   |
 * 
 */
// it also initiates a list containing all the hexgrids for use by other modules
public class GridGenerator: MonoBehaviour
{
	//this will enable testing of preserved data and reference
	[SerializeField]
	private int m_hexGridRefTestNum;
	
	public bool m_maskActive;
    public GameObject m_hexPrefab;
	public GameObject m_hexMaskPrefab;
	public Material m_hexBlueMaskMat;
	public Material m_hexRedMaskMat;
	public Material m_hexGreenMaskMat;
	public Material m_hexOutlineMaskMat;
	
    //instantiate using unity editor
    public int m_gridNumHor = 11; //number of grids in horizontal direction
    public int m_gridNumVer = 11; //number of grids in vertical direction

    //Hexagon tile width and length in game world
    private float m_hexWidth;
    private float m_hexLength;
	private Vector3 m_initPos;
	
	//list of all hexgrids
	[SerializeField]
	//unity cannot serialize nested list.....
	//public List<List<HexGridModel>> grids;
	private List<GameObject> m_grids;
	
	public List<List<GameObject>> GetGridData()
	{
		List<List<GameObject>> grids = new List<List<GameObject>>();
		for(int i=0;i<m_gridNumVer;i++)
			grids.Add(new List<GameObject>());
		int index = 0; //index of grid in m_grids
		for(int row=0;row<m_gridNumVer;row++){
			int numNodes = (row%2 == 0)?m_gridNumHor:m_gridNumHor-1;
			for(int col=0;col<numNodes;col++){
				grids[row].Add(m_grids[index]);
				index ++;
			}
		}
		if(index != m_grids.Count)
			throw new DataMisalignedException("Wrong number of grids in returned data");
		//if something goes wrong in this method, the index will simply not match up
		//the program will throw an exception
		return grids;
	}

    //Method to initialise Hexagon width and length
    private void GetGridSize()
    {
        //renderer component attached to the Hex prefab is used to get the current width and length
        m_hexWidth = m_hexPrefab.renderer.bounds.size.x;
        m_hexLength = m_hexPrefab.renderer.bounds.size.z;
    }
	
	//Method to initliase the list
	private void InitList()
	{
		m_grids = new List<GameObject>();
	}
	
	//Method to initliase testing states
	private void InitDebugStates()
	{
		m_hexGridRefTestNum = 0;
	}

    //Method to calculate the position of the first hexagon tile (negative x, positive z)
    //The center of the hex grid is (0,0,0)
    private void CalcInitPos()
    {
		m_initPos =  new Vector3(m_hexWidth*(-m_gridNumHor+1)/2f, 0, (m_gridNumVer/2)*m_hexLength*3f/4);
    }

    //Method used to convert coordinates in grids to game world coordinates
    private Vector3 CalcWorldCoord(int gridX, int gridY)
    {
        //Every second row is offset by half of the tile width
        float offset = 0;
        if (gridY % 2 != 0)
            offset = m_hexWidth / 2;
        float worldX =  (float)m_initPos.x + offset + gridX * m_hexWidth;
		
        //Every new line is offset in z direction by 3/4 of the hexagon length
        float worldZ = (float)m_initPos.z - gridY * m_hexLength * 0.75f;
        return new Vector3(worldX, 0, worldZ);
    }

    //Method to initialises and positions all the tiles based on initial position calculated
    private void CreateGrids()
    {
		//by default, masks are active
		m_maskActive = true;
		//random seeder for testing
		System.Random testIntGenerator = new System.Random();
        //Game object which is the parent of all the hex tiles
		if(GameObject.Find("HexGrids") == null)
		{
			InitDebugStates();
			InitList();
			//parent object to all the grids
        	GameObject hexGridGroup = new GameObject("HexGrids");
       		for (int y = 0; y < m_gridNumVer; y++)
       		{
				//row
				//alternating pattern
				int gridsToDraw = (y%2==0)?m_gridNumHor:m_gridNumHor-1;
				
            	for (int x = 0; x < gridsToDraw; x++)
            	{
					//col
                	GameObject hex = (GameObject)Instantiate(m_hexPrefab);
					hex.name = String.Format("Grid[{0}][{1}]",y,x);
					//get world coordinates of grid
                	hex.transform.position = CalcWorldCoord(x,y);
					//assign parent
                	hex.transform.parent = hexGridGroup.transform;
					Vector2 grid2DPosition = new Vector2(hex.transform.position.x, hex.transform.position.z);
					//initialise model
					hex.GetComponent<HexGridModel>().Initialise(grid2DPosition,m_hexWidth,m_hexLength,y,x); //x,y denoting col and row of grid
					hex.GetComponent<TnGAttribute>().m_terrainType = TerrainType.plain;
					hex.tag = "Grid";
					
					//add mask to the grid as children
					GameObject mask = (GameObject)Instantiate(m_hexMaskPrefab);
					mask.name = "mask";
					hex.GetComponent<MaskManager>().InitMasks(mask,m_hexRedMaskMat,m_hexGreenMaskMat,m_hexBlueMaskMat,m_hexOutlineMaskMat);
					
					//set the test bit in terrain component of hexmap to test reference
					int testInt = testIntGenerator.Next(0,m_gridNumHor);
					//test case set to 1
					if(testInt == 1)
						m_hexGridRefTestNum ++;
					hex.GetComponent<TestAttribute>().testNum = testInt;
					m_grids.Add(hex);
            	}
        	}
		}
    }
	
	private void TestListSerielisation()
	{
		bool passedTest = true;
		foreach(GameObject e in m_grids){
			if(e == null){
				passedTest = false;
			}
		}
		
		if(!passedTest)
			Debug.Log("Failed TestListSerielisation()");
		else
			Debug.Log("Passed TestListSerielisation()");
		/*
		foreach(GameObject e in m_grids)
		{
			Debug.Log(e);
		}*/
	}
	
	private void TestReferenceSerielisation()
	{
		int numTestInt = 0;
		//test case set to 1
		foreach(GameObject e in m_grids)
		{
			if((e.GetComponent<TestAttribute>() as TestAttribute).testNum == 1)
				numTestInt ++;
		}
		if(numTestInt!= m_hexGridRefTestNum)
			Debug.Log ("Failed TestReferenceSerielisation()");
		else 
			Debug.Log ("Passed TestReferenceSerielisation()");
	}
	
	public void CreateGridMap()
	{
		GetGridSize();
		CalcInitPos();
        CreateGrids();
		UpdateGridMap();
	}
	
	//update y scaling as well as movement cost
	public void UpdateGridMap()
	{
		//TODO: all constants are hard coded, this is bad
		foreach(GameObject e in m_grids){
			TnGAttribute tempTnG = e.GetComponent<TnGAttribute>();
			HexGridModel tempModel = e.GetComponent<HexGridModel>();
			//update y scaling of different terrain types
			if(tempTnG.m_terrainType == TerrainType.hill){
				Vector3 scaling = new Vector3(1.0f,0.35f,1.0f);
				e.transform.localScale = scaling;
				tempModel.m_movementCost = 2;
			}
			else if(tempTnG.m_terrainType == TerrainType.obstacle){
				Vector3 scaling = new Vector3(1.0f,0.1f,1.0f);
				e.transform.localScale = scaling;
				tempModel.m_movementCost = 0;
			}
			else//plain
			{
				Vector3 scaling = new Vector3(1.0f,0.2f,1.0f);
				e.transform.localScale = scaling;
				tempModel.m_movementCost = 1;
			}
			//object has unit on top, place unit to proper place
			if(tempTnG.m_unit != null){
				tempTnG.m_unit.transform.position = 
					new Vector3(tempModel.m_center.x,e.renderer.bounds.max.y,tempModel.m_center.y);
				tempTnG.m_unit.tag = "Unit";
				//initialise parent object group
				if(GameObject.Find("Units") == null){
					new GameObject("Units");
				}
				tempTnG.m_unit.transform.parent = GameObject.Find("Units").transform;
			}
		}
	}
	
	public void RunTests()
	{
		TestListSerielisation();
		TestReferenceSerielisation();
	}
	
	public void Xmirror()
	{
		List<List<GameObject>> grids = GetGridData();
		if(grids.Count%2 != 1)
		{
			Debug.LogWarning("The map is not symmetric about x axis, number of rows must be odd"); 
			return;
		}
		int head = 0;
		int tail = grids.Count-1;
		while(head < tail){
			for(int i=0;i<grids[head].Count;i++){
				grids[tail][i].GetComponent<TnGAttribute>().MirrorTnGAttributesFrom(grids[head][i].GetComponent<TnGAttribute>());
			}
			head ++;
			tail --;
		}
		UpdateGridMap();
	}
	
	public void Ymirror()
	{
		List<List<GameObject>> grids = GetGridData();
		int head;
		int tail;
		for(int i=0;i<grids.Count;i++)
		{
			head = 0;
			tail = grids[i].Count-1;
			while(head<tail){
				grids[i][tail].GetComponent<TnGAttribute>().MirrorTnGAttributesFrom(grids[i][head].GetComponent<TnGAttribute>());
				head ++;
				tail --;
			}
		}
		UpdateGridMap();
	}
	
	public void ToggleMask()
	{
		m_maskActive = !m_maskActive;
		if(m_maskActive){
			foreach(GameObject e in m_grids)
				e.GetComponent<MaskManager>().ActivateMask();
		}
		else{
			foreach(GameObject e in m_grids)
				e.GetComponent<MaskManager>().DeactivateMask();
		}
	}
}
