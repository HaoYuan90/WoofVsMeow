using UnityEngine;
using System.Collections;

public class StartingPageGUI : MonoBehaviour 
{
	public GUIStyle m_playerNameStyle;
	public GUIStyle m_textStyle;
	
	private float fixedHeight = 598.0f; //height of screen in free aspect ratio
	private float fixedWidth = 1366.0f; //width of screen in free aspect ratio
	
	private float widthRatio;
	private float heightRatio;
	private float combinedRatio;
	
	private string m_playerName;
	private string m_namePrefix;
	
	private Rect m_fixedPlayerNameRect = new Rect(0,50,160,60); //Fixed position of the player name for 16:9 aspect ratio
	private Rect m_fixedNamePrefixRect = new Rect(0,50,70,60); //Fixed position of the player name label for 16:9 aspect ratio
	private Rect m_fixedButtonRect = new Rect(0,50,150,60); //Fixed position of the player name button for 16:9 aspect ratio
	
	private Rect m_playerNameRect = new Rect(0,50,120,40);
	private Rect m_namePrefixRect = new Rect(0,50,70,40);
	private Rect m_buttonRect = new Rect(0,50,150,40);
	
	void Start()
	{
		m_playerName = PlayerPrefs.GetString("playername");
		m_namePrefix = "Player: ";
	}
	
	void Update () 
	{
		widthRatio = Screen.width/fixedWidth;
		heightRatio = Screen.height/fixedHeight;
		if (widthRatio<heightRatio){combinedRatio = widthRatio;}
		else{combinedRatio = heightRatio;}
		
		m_playerNameRect = new Rect(Screen.width/2 - (m_fixedPlayerNameRect.width/2)*combinedRatio, m_fixedPlayerNameRect.yMax*combinedRatio, m_fixedPlayerNameRect.width*combinedRatio, m_fixedPlayerNameRect.height*combinedRatio);
		m_namePrefixRect = new Rect((m_fixedPlayerNameRect.xMin - m_fixedNamePrefixRect.width -5)*combinedRatio, m_fixedNamePrefixRect.yMax*combinedRatio, m_fixedNamePrefixRect.width*combinedRatio, m_fixedNamePrefixRect.height*combinedRatio);
		m_buttonRect = new Rect(m_playerNameRect.xMax+5, (m_fixedButtonRect.yMax)*combinedRatio, (m_fixedButtonRect.width)*combinedRatio, m_fixedButtonRect.height*combinedRatio);
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
