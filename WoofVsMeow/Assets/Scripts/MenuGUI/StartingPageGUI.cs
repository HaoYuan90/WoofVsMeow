using UnityEngine;
using System.Collections;

public class StartingPageGUI : MonoBehaviour 
{
	public GUIStyle m_playerNameStyle;
	public GUIStyle m_textStyle;
	public GUIStyle m_buttonStyle;
	
	private string m_playerName;
	private string m_namePrefix;
	
	//We store the x,y,height and width of the GUI elements here for easy modification
	private Rect m_playerNameRect = new Rect(0,400,160,60);
	private Rect m_namePrefixRect = new Rect(0,400,200,60);
	private Rect m_saveButtonRect = new Rect(0,400,180,60);
	private Rect m_mainButtonRect = new Rect(0,0,400,140);
	
	//Modify this to control the gap between Save and the left of the screen
	private float m_mainBtnOffsetX = 150.0f;
	private float m_mainBtnOffsetY = 50.0f;
	
	readonly private float m_optimalWidth = 1600.0f;
	readonly private float m_optimalHeight = 900.0f;
	
	void Start()
	{
		m_playerName = PlayerPrefs.GetString("playername");
		m_namePrefix = "Your Name: ";
		
		m_playerNameRect = new Rect(m_optimalWidth/2 - m_playerNameRect.width/2, m_playerNameRect.y, 
			m_playerNameRect.width, m_playerNameRect.height);
		m_namePrefixRect = new Rect(m_playerNameRect.xMin - m_namePrefixRect.width - 10, m_namePrefixRect.y, 
			m_namePrefixRect.width, m_namePrefixRect.height);
		m_saveButtonRect = new Rect(m_playerNameRect.xMax + 40, m_saveButtonRect.y,
			m_saveButtonRect.width,m_saveButtonRect.height);

	}
	
	void OnGUI()
	{
		float ratio = Screen.width/m_optimalWidth;
		Rect buttonRect = m_mainButtonRect;
		
		buttonRect = new Rect(m_mainBtnOffsetX*ratio, Screen.height-(m_mainBtnOffsetY + m_mainButtonRect.height)*ratio, 
			buttonRect.width*ratio, buttonRect.height*ratio);
		if (GUI.Button(buttonRect,"Start",m_buttonStyle))
		{
			Application.LoadLevel("NetworkMenu");
		}
		buttonRect = new Rect(Screen.width-(m_mainBtnOffsetX+m_mainButtonRect.width)*ratio, buttonRect.y , buttonRect.width, buttonRect.height);
		if (GUI.Button(buttonRect,"Exit",m_buttonStyle))
		{
			Debug.Log ("NO! YOU CANNOT EXIT THIS GAME!");
		}
		
		GUI.Label(new Rect(m_namePrefixRect.x*ratio,m_namePrefixRect.y*ratio,m_namePrefixRect.width*ratio,
			m_namePrefixRect.height*ratio),m_namePrefix,m_textStyle);
        m_playerName = GUI.TextField(new Rect(m_playerNameRect.x*ratio,m_playerNameRect.y*ratio,m_playerNameRect.width*ratio,
			m_playerNameRect.height*ratio), m_playerName, 10 ,m_playerNameStyle);
		
		if(GUI.Button(new Rect(m_saveButtonRect.x*ratio,m_saveButtonRect.y*ratio,m_saveButtonRect.width*ratio,
			m_saveButtonRect.height*ratio),"Save Name")){
			PlayerPrefs.SetString ("playername",m_playerName);
		}
	}
}
