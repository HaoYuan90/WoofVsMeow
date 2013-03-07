using UnityEditor;
using UnityEngine;

public class GridEditorControl : EditorWindow
{
	public int m_numRows;
	public int m_numCols;

	// Add menu item named "My Window" to the Window menu
	[MenuItem("Window/Grid Editor")]
	public static void ShowWindow()
	{
		//Show existing window instance. If one doesn't exist, make one.
		EditorWindow.GetWindow(typeof(GridEditorControl));
	}

	void OnGUI()
	{
		//fields to recreate gridmap
		GUILayout.Label("Recreate grid map, will erase all you had before");
		m_numRows = EditorGUILayout.IntField("number of rows",m_numRows);
		m_numCols = EditorGUILayout.IntField("number of cols",m_numCols);
		if(GUILayout.Button("Recreate grid map"))
		{
			if(GameObject.Find("HexGrids")!=null){
				GameObject.DestroyImmediate(GameObject.Find("HexGrids"));
			}
			GridGenerator gen = GameObject.Find("GridRenderer").GetComponent<GridGenerator>();
			gen.m_gridNumHor = m_numCols;
			gen.m_gridNumVer = m_numRows;
			gen.CreateGridMap();
		}
		
		GUILayout.Label("Update grid map, set y scaling according to terrain type");
		if(GUILayout.Button("Update grid map"))
		{
			GameObject.Find("GridRenderer").GetComponent<GridGenerator>().UpdateGridMap();
		}
		
		GUILayout.Label("Testing reference persistency and list serielisation");
		if(GUILayout.Button("Test"))
		{
			GameObject.Find("GridRenderer").GetComponent<GridGenerator>().RunTests();
		}
		GUILayout.Label("Mirror the map from top-down(x mirror) or from left-right(y mirror)");
		if(GUILayout.Button("X mirror"))
		{
			GameObject.Find("GridRenderer").GetComponent<GridGenerator>().Xmirror();
		}
		if(GUILayout.Button("Y mirror"))
		{
			GameObject.Find("GridRenderer").GetComponent<GridGenerator>().Ymirror();
		}
	}
}