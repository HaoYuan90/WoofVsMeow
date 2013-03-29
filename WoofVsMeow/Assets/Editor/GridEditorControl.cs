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
		//setup the scene
		GUILayout.Label("Initialise all necessary components");
		if(GUILayout.Button("Setup Scene"))
		{
			//adjust camera
			GameObject camera = GameObject.Find("Main Camera");
			camera.transform.position = new Vector3(0f,25f,30f);
			camera.transform.rotation = Quaternion.identity;
			camera.transform.Rotate(new Vector3(50f,180f,0f));
			if(camera.GetComponent<CameraController>() == null)
				camera.AddComponent<CameraController>();
			//setup lighting
			if(GameObject.Find("DirectionalLightForEditingMap") == null)
			{
				GameObject temp = new GameObject("DirectionalLightForEditingMap");
				temp.transform.position = new Vector3(0f,50f,0f);
				temp.transform.rotation = Quaternion.identity;
				temp.transform.Rotate(new Vector3(90f,0f,0f));
				temp.AddComponent<Light>();
				Light light = temp.GetComponent<Light>();
				light.type = LightType.Directional;
				light.color = Color.white;
				light.intensity = 0.59f;
			}
			//setup renderer
			if(GameObject.Find("GridRenderer") == null)
			{
				GameObject temp = new GameObject("GridRenderer");
				temp.transform.position = new Vector3(100f,100f,100f);
				temp.transform.rotation = Quaternion.identity;
				temp.AddComponent<GridGenerator>();
			}
			//setup engine
			if(GameObject.Find("GameEngine") == null)
			{
				GameObject temp = new GameObject("GameEngine");
				temp.transform.position = new Vector3(100f,100f,100f);
				temp.transform.rotation = Quaternion.identity;
				temp.AddComponent<GridLogic>();
				temp.AddComponent<APSequenceController>();
				temp.AddComponent<GameEngine>();
				temp.AddComponent<NetworkView>();
			}
		}
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
		
		if(GUILayout.Button("Toggle grid masks"))
		{
			GameObject.Find("GridRenderer").GetComponent<GridGenerator>().ToggleMask();
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