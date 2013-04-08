using UnityEngine;
using System.Collections;

public class StartingPageGUI : MonoBehaviour 
{
	public GUIStyle m_playerNameStyle;
	public GUIStyle m_textStyle;
	
	private float fixedHeight = 598.0f;
	private float fixedWidth = 1366.0f;
	
	private float widthRatio;
	private float heightRatio;
	private float combinedRatio;
	
	private string m_playerName;
	private string m_namePrefix;
	
	private Rect m_playerNameRect = new Rect(0,50,120,28);
	private Rect m_namePrefixRect = new Rect(0,50,70,28);
	private Rect m_buttonRect = new Rect(0,50,150,28);
	
	void Start()
	{
		m_playerName = PlayerPrefs.GetString("playername");
		m_namePrefix = "Player: ";
		
		m_playerNameRect = new Rect(Screen.width/2 - m_playerNameRect.width/2, m_playerNameRect.yMax, m_playerNameRect.width, m_playerNameRect.height);
		m_namePrefixRect = new Rect(m_playerNameRect.xMin - m_namePrefixRect.width -5, m_namePrefixRect.yMax, m_namePrefixRect.width, m_namePrefixRect.height);
		m_buttonRect = new Rect(m_playerNameRect.xMax+5, m_buttonRect.yMax,m_buttonRect.width,m_buttonRect.height);
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
		
		GUI.Label(m_namePrefixRect,m_namePrefix,m_textStyle);
        m_playerName = GUI.TextField(m_playerNameRect, m_playerName, 10 ,m_playerNameStyle);
		if(GUI.Button(m_buttonRect,"save name")){
			PlayerPrefs.SetString ("playername",m_playerName);
		}
	}
}
