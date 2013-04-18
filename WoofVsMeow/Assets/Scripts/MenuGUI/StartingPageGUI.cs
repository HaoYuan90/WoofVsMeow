using UnityEngine;
using System.Collections;

public class StartingPageGUI : MonoBehaviour 
{
	public Texture2D m_bgTexture;
	public GUIStyle m_playerNameStyle;
	public GUIStyle m_textStyle;
	public GUIStyle m_buttonStyle;
	
	private string m_playerName;
	private string m_namePrefix;
	
	//We store the x,y,height and width of the GUI elements here for easy modification
	private Rect m_playerNameRect = new Rect(0,20,250,100);
	private Rect m_namePrefixRect = new Rect(0,20,350,100);
	private Rect m_mainButtonRect = new Rect(0,0,300,80);
	
	//Modify this to control the gap between Save and the left of the screen
	private float m_mainBtnOffsetX = 350.0f;
	private float m_mainBtnOffsetY = 80.0f;
	
	readonly private float m_optimalWidth = 1600.0f;
	//readonly private float m_optimalHeight = 900.0f;
	
	void Start()
	{
		m_playerName = PlayerPrefs.GetString("playername");
		m_namePrefix = "Hello: ";	
	}
	
	void OnGUI()
	{
		//background
		GUI.DrawTexture(new Rect(0f,0f,Screen.width,Screen.height),m_bgTexture);
		//buttons
		float ratio = Screen.width/m_optimalWidth;
		Rect buttonRect = m_mainButtonRect;
		buttonRect = new Rect(m_mainBtnOffsetX*ratio, Screen.height-(m_mainBtnOffsetY + m_mainButtonRect.height)*ratio, 
			buttonRect.width*ratio, buttonRect.height*ratio);
		if (GUI.Button(buttonRect,"Start",m_buttonStyle))
		{
			PlayerPrefs.SetString ("playername",m_playerName);
			Application.LoadLevel("NetworkMenu");
		}
		buttonRect = new Rect(Screen.width-(m_mainBtnOffsetX+m_mainButtonRect.width)*ratio, buttonRect.y , buttonRect.width, buttonRect.height);
		if (GUI.Button(buttonRect,"Exit",m_buttonStyle))
		{
			Application.Quit();
		}
		//name label
		//ratio is applied here so that label and box stay in the right position if the window size is changed
		GUI.Label(new Rect(Screen.width/2 - m_playerNameRect.width*ratio/2 - (m_namePrefixRect.width + 10)*ratio, m_namePrefixRect.y*ratio, 
			m_namePrefixRect.width*ratio, m_namePrefixRect.height*ratio),m_namePrefix,m_textStyle);
        m_playerName = GUI.TextField(new Rect(Screen.width/2 - m_playerNameRect.width*ratio/2, m_playerNameRect.y, 
			m_playerNameRect.width*ratio, m_playerNameRect.height*ratio), m_playerName, 10 ,m_playerNameStyle);
		
	}
}
