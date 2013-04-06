using UnityEngine;
using System.Collections;

public class StartingPageGUI : MonoBehaviour 
{
	private float fixedHeight = 598.0f;
	private float fixedWidth = 1366.0f;
	
	private float widthRatio;
	private float heightRatio;

	void Start () 
	{
		
	}
	
	void Update () 
	{
		widthRatio = Screen.width/fixedWidth;
		heightRatio = Screen.height/fixedHeight;
	}
	
	void OnGUI()
	{
		if (GUI.Button(new Rect(433.0f*widthRatio, 250.0f*heightRatio, 500.0f*widthRatio, 100.0f*heightRatio),new GUIContent("Start")))
		{
			Application.LoadLevel("NetworkMenu");
		}
		
		if (GUI.Button(new Rect(533.0f*widthRatio, 400.0f*heightRatio, 300.0f*widthRatio, 75.0f*heightRatio),new GUIContent("Exit")))
		{
			Debug.Log ("NO! YOU CANNOT EXIT THIS GAME!");
		}
		
	}
}
