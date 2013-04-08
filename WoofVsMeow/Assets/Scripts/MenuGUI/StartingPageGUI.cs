using UnityEngine;
using System.Collections;

public class StartingPageGUI : MonoBehaviour 
{
	private float fixedHeight = 598.0f;
	private float fixedWidth = 1366.0f;
	
	private float widthRatio;
	private float heightRatio;
	private float combinedRatio;

	void Start () 
	{
		
	}
	
	void Update () 
	{
		widthRatio = Screen.width/fixedWidth;
		heightRatio = Screen.height/fixedHeight;
		if (widthRatio<heightRatio){combinedRatio = widthRatio;}
		else{combinedRatio = heightRatio;}
	}
	
	void OnGUI()
	{
		if (GUI.Button(new Rect(433.0f*combinedRatio, 250.0f*combinedRatio, 500.0f*combinedRatio, 100.0f*combinedRatio),new GUIContent("Start")))
		{
			Application.LoadLevel("NetworkMenu");
		}
		
		if (GUI.Button(new Rect(533.0f*combinedRatio, 400.0f*combinedRatio, 300.0f*combinedRatio, 75.0f*combinedRatio),new GUIContent("Exit")))
		{
			Debug.Log ("NO! YOU CANNOT EXIT THIS GAME!");
		}
		
	}
}
