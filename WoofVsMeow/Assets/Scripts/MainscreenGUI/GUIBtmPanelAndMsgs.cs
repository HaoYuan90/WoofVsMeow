using UnityEngine;
using System.Collections;

public class GUIBtmPanelAndMsgs : MonoBehaviour 
{
	private float fixedHeight = 598.0f;
	private float fixedWidth = 1366.0f;
	
	private float widthRatio;
	private float heightRatio;
	private float combinedRatio;
	
	public Texture2D money_Tex;
	public Texture2D commander_Tex;
	
	public GUIStyle boxStyle;
	public GUIStyle labelStyle;
	
	private string playerName;
	private double m_money;
	private string commanderName;
	
	private bool m_gameOver;
	private string m_winner;
	private bool m_connectionError;
	private string m_errorMsg;

	// Use this for initialization
	public void Initialise (int money) 
	{
		playerName = PlayerPrefs.GetString("playername");
		m_money = money;
		commanderName = "Joker";
		
		m_gameOver = false;
		m_connectionError = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		widthRatio = Screen.width/fixedWidth;
		heightRatio = Screen.height/fixedHeight;
		if (widthRatio<heightRatio){combinedRatio = widthRatio;}
		else{combinedRatio = heightRatio;}
	}
	
	public void GameWonBy (string name)
	{
		m_winner = name;
		m_gameOver = true;
	}
	
	public void ConnectionError (string errorMsg)
	{
		m_errorMsg = errorMsg;
		m_connectionError = true;
	}
	
	public void SetMoney (int money)
	{
		m_money = money;
	}
	
	void OnGUI()
	{
		GUI.Box(new Rect(0, Screen.height-110.0f*combinedRatio, Screen.width, 110.0f*combinedRatio),"");
		GUI.Label(new Rect(150.0f*combinedRatio, Screen.height-75.0f*combinedRatio, 300.0f*combinedRatio, 40.0f*combinedRatio), 
			"Name : " + playerName, labelStyle);
		GUI.Box (new Rect(750.0f*combinedRatio, Screen.height-94.0f*combinedRatio, 66.0f*combinedRatio, 66.0f*combinedRatio), 
			money_Tex, boxStyle);
		GUI.Label(new Rect(820.0f*combinedRatio, Screen.height-75.0f*combinedRatio, 200.0f*combinedRatio, 40.0f*combinedRatio), 
			"m_money : $" + m_money.ToString(), labelStyle);
		GUI.Box (new Rect(1050.0f*combinedRatio, Screen.height-94.0f*combinedRatio, 66.0f*combinedRatio, 66.0f*combinedRatio), 
			commander_Tex, boxStyle);
		GUI.Label(new Rect(1120.0f*combinedRatio, Screen.height-75.0f*combinedRatio, 200.0f*combinedRatio, 40.0f*combinedRatio), 
			"Commander : " + commanderName, labelStyle);
		
		if(m_gameOver){
			GUI.Label(new Rect(0, 0, 200.0f*combinedRatio, 40.0f*combinedRatio), 
			m_winner + "has won!", labelStyle);
			if(GUI.Button(new Rect(200f, 200f, 200.0f*combinedRatio, 40.0f*combinedRatio),"Return to menu"))
			{
				//close connections
				Network.Disconnect();
				if(Network.isServer){
				MasterServer.UnregisterHost();
				}
				Application.LoadLevel("NetworkMenu");
			}
		}
		else if(m_connectionError){
			GUI.Label(new Rect(0, 0, 200.0f*combinedRatio, 40.0f*combinedRatio), 
			m_errorMsg, labelStyle);
			if(GUI.Button(new Rect(200f, 200f, 200.0f*combinedRatio, 40.0f*combinedRatio),"Return to menu"))
			{
				Application.LoadLevel("NetworkMenu");
			}
		}
	}
}
