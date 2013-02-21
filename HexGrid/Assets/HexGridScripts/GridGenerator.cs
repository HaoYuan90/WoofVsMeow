using UnityEngine;
using System.Collections.Generic;
using System;

[ExecuteInEditMode]
// this class generator a map of hex grids in the following pattern
/*
 *   ======== width ========= |
 *   * * * * * * * * * * * *  |
 *    * * * * * * * * * * *   height
 *   * * * * * * * * * * * *  |
 *    * * * * * * * * * * *   |
 * 
 */
// it also initiates a list containing all the hexgrids for use by other modules
public class GridGenerator: MonoBehaviour
{
	//when this is enabled, map will be destroyed and reinitialised everytime play is hit
	public bool m_mapGenDebugMode = false;
	//this will enable testing of preserved data and reference
	public bool m_editorDebugMode = true;
	[SerializeField]
	private int m_hexGridRefTestNum;
	
    public GameObject m_hexPrefab;
    //instantiate using unity editor
    public int m_gridNumHor = 11; //number of grids in horizontal direction
    public int m_gridNumVer = 11; //number of grids in vertical direction

    //Hexagon tile width and height in game world
    private float m_hexWidth;
    private float m_hexHeight;
	private Vector3 m_initPos;
	
	//list of all hexgrids
	[SerializeField]
	//unity cannot serialize nested list.....
	//public List<List<HexGridModel>> grids;
	private List<HexGridModel> grids;

    //Method to initialise Hexagon width and height
    private void GetGridSize()
    {
        //renderer component attached to the Hex prefab is used to get the current width and height
        m_hexWidth = m_hexPrefab.renderer.bounds.size.x;
        m_hexHeight = m_hexPrefab.renderer.bounds.size.z;
    }
	
	//Method to initliase the list
	private void CreateList()
	{
		grids = new List<HexGridModel>();
		/*
		grids = new List<List<HexGridModel>>();
		for(int i=0;i<m_gridNumVer;i++)
			grids.Add(new List<HexGridModel>());
			*/
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
		m_initPos =  new Vector3(m_hexWidth*(-m_gridNumHor+1)/2f, 0, (m_gridNumVer/2)*m_hexHeight*3f/4);
    }

    //Method used to convert coordinates in grids to game world coordinates
    private Vector3 CalcWorldCoord(int gridX, int gridY)
    {
        //Every second row is offset by half of the tile width
        float offset = 0;
        if (gridY % 2 != 0)
            offset = m_hexWidth / 2;
        float worldX =  (float)m_initPos.x + offset + gridX * m_hexWidth;
		
        //Every new line is offset in z direction by 3/4 of the hexagon height
        float worldZ = (float)m_initPos.z - gridY * m_hexHeight * 0.75f;
        return new Vector3(worldX, 0, worldZ);
    }

    //Method to initialises and positions all the tiles
    private void CreateGrid()
    {
		//random seeder for testing
		System.Random testIntGenerator = new System.Random();
		if(m_mapGenDebugMode)
			GameObject.DestroyImmediate(GameObject.Find("HexGrids"));
        //Game object which is the parent of all the hex tiles
		if(GameObject.Find("HexGrids") == null)
		{
			InitDebugStates();
			CreateList();
			//parent object to all the grids
        	GameObject hexGridGroup = new GameObject("HexGrids");
       		for (int y = 0; y < m_gridNumVer; y++)
       		{
				//alternating pattern
				int gridsToDraw = (y%2==0)?m_gridNumHor:m_gridNumHor-1;
				
            	for (int x = 0; x < gridsToDraw; x++)
            	{
                	GameObject hex = (GameObject)Instantiate(m_hexPrefab);
					//get world coordinates of grid
                	hex.transform.position = CalcWorldCoord(x,y);
					//assign parent
                	hex.transform.parent = hexGridGroup.transform;
					Vector2 grid2DPosition = new Vector2(hex.transform.position.x, hex.transform.position.z);
					//set the test bit in terrain component of hexmap to test reference
					if(m_editorDebugMode)
					{
						int testInt = testIntGenerator.Next(0,m_gridNumHor);
						//test case set to 1
						if(testInt == 1)
							m_hexGridRefTestNum ++;
						(hex.GetComponent<TerrainAttribute>() as TerrainAttribute).testNum = testInt;
					}
					HexGridModel temp = ScriptableObject.CreateInstance<HexGridModel>();
					temp.Initialise(hex,grid2DPosition,m_hexWidth,m_hexHeight);
					grids.Add(temp);
            	}
        	}
		}
    }
	
	private void TestListSerielisation()
	{
		bool passedTest = true;
		foreach(HexGridModel e in grids){
			if(e == null){
				passedTest = false;
			}
		}
		
		if(!passedTest)
			Debug.Log("Failed TestListSerielisation()");
		
		/*
		foreach(HexGridModel e in grids)
		{
			Debug.Log(e);
		}*/
	}
	
	private void TestReferenceSerielisation()
	{
		int numTestInt = 0;
		//test case set to 1
		foreach(HexGridModel e in grids)
		{
			if((e.grid.GetComponent<TerrainAttribute>() as TerrainAttribute).testNum == 1)
				numTestInt ++;
		}
		if(numTestInt!= m_hexGridRefTestNum)
			Debug.Log ("Failed TestReferenceSerielisation()");
	}

    //The grid should be generated on game start
    void Start()
    {
        GetGridSize();
		CalcInitPos();
        CreateGrid();
		
		if(m_editorDebugMode){
			TestListSerielisation();
			TestReferenceSerielisation();
		}
    }
}
