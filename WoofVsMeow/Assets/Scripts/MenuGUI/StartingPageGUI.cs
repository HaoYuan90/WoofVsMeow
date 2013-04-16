using UnityEngine;
using System.Collections;

public class StartingPageGUI : MonoBehaviour 
{
	public GUIStyle m_playerNameStyle;
	public GUIStyle m_textStyle;
	
	private string m_playerName;
	private string m_namePrefix;
	
	private Rect m_playerNameRect = new Rect(0,80,120,40);
	private Rect m_namePrefixRect = new Rect(0,80,120,40);
	private Rect m_saveButtonRect = new Rect(0,80,150,40);
	private Rect m_mainButtonRect = new Rect(0,0,500,80);
	
	readonly private float m_optimalWidth = 1600.0f;
	readonly private float m_optimalHeight = 900.0f;
	
	void Start()
	{
		m_playerName = PlayerPrefs.GetString("playername");
		m_namePrefix = "Your name: ";
		
		float ratio = Screen.width/m_optimalWidth;
		m_playerNameRect = new Rect(Screen.width/2 - m_playerNameRect.width/2, m_playerNameRect.yMax*ratio, 
			m_playerNameRect.width*ratio, m_playerNameRect.height*ratio);
		m_namePrefixRect = new Rect(m_playerNameRect.xMin - m_namePrefixRect.width -10, m_namePrefixRect.yMax*ratio, 
			m_namePrefixRect.width*ratio, m_namePrefixRect.height*ratio);
		m_saveButtonRect = new Rect(m_playerNameRect.xMax+10, m_saveButtonRect.yMax*ratio,
			m_saveButtonRect.width*ratio,m_saveButtonRect.height*ratio);

	}
	
	void OnGUI()
	{
		float ratio = Screen.width/m_optimalWidth;
		Rect buttonRect = m_mainButtonRect;
		buttonRect = new Rect(Screen.width/2 - buttonRect.width/2, Screen.height*3/5, 
			buttonRect.width*ratio, buttonRect.height*ratio);
		if (GUI.Button(buttonRect,"Start"))
		{
			PlayerPrefs.SetString ("playername",m_playerName);
			Application.LoadLevel("NetworkMenu");
		}
		buttonRect = new Rect(buttonRect.x, buttonRect.y+buttonRect.height+20 , buttonRect.width, buttonRect.height);
		if (GUI.Button(buttonRect,"Exit"))
		{
			Debug.Log ("NO! YOU CANNOT EXIT THIS GAME!");
		}
		
		GUI.Label(m_namePrefixRect,m_namePrefix,m_textStyle);
        m_playerName = GUI.TextField(m_playerNameRect, m_playerName, 10 ,m_playerNameStyle);
	}
}
