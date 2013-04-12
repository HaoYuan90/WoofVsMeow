using UnityEngine;
using System.Collections;

public class GUIBtmPanelAndMsgs : MonoBehaviour 
{
	private float fixedHeight = 598.0f; //height of screen in free aspect ratio
	private float fixedWidth = 1366.0f; //width of screen in free aspect ratio
	
	private float widthRatio;
	private float heightRatio;
	private float combinedRatio;
	
	public Texture2D money_Tex;
	public Texture2D commander_Tex;
	
	public GUIStyle boxStyle;//For the boxes carrying textures
	public GUIStyle labelStyle;//For the labels in the bottom panel
	public GUIStyle messageStyle;//For the messages that appear at the top of the screen. They need a larger font
	
	private string playerName;
	private double m_money;
	private string commanderName;
	
	private bool m_gameOver;
	private string m_winner;
	private bool m_connectionError;
	private string m_errorMsg;
	private bool m_waitingForTurn;

	// Use this for initialization
	public void Initialise (int money) 
	{
		playerName = PlayerPrefs.GetString("playername");
		m_money = money;
		commanderName = "Joker";
		
		m_gameOver = false;
		m_connectionError = false;
		m_waitingForTurn = false;
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
	
	public void OnMyTurn()
	{
		m_waitingForTurn = false;
	}
	public void OnOthersTurn()
	{
		m_waitingForTurn = true;
	}
	
	void OnGUI()
	{
		//Bottom panel
		GUI.Box(new Rect(0, Screen.height-110.0f*combinedRatio, Screen.width, 110.0f*combinedRatio),"");
		//Name Label
		GUI.Label(new Rect(150.0f*combinedRatio, Screen.height-75.0f*combinedRatio, 300.0f*combinedRatio, 40.0f*combinedRatio), 
			"Name : " + playerName, labelStyle);
		//Money texture
		GUI.Box (new Rect(550.0f*combinedRatio, Screen.height-94.0f*combinedRatio, 66.0f*combinedRatio, 66.0f*combinedRatio), 
			money_Tex, boxStyle);
		//Money label
		GUI.Label(new Rect(620.0f*combinedRatio, Screen.height-75.0f*combinedRatio, 200.0f*combinedRatio, 40.0f*combinedRatio), 
			"m_money : $" + m_money.ToString(), labelStyle);
		//Commander texture
		GUI.Box (new Rect(950.0f*combinedRatio, Screen.height-94.0f*combinedRatio, 66.0f*combinedRatio, 66.0f*combinedRatio), 
			commander_Tex, boxStyle);
		//Commander label
		GUI.Label(new Rect(1020.0f*combinedRatio, Screen.height-75.0f*combinedRatio, 200.0f*combinedRatio, 40.0f*combinedRatio), 
			"Commander : " + commanderName, labelStyle);
		
		if(m_gameOver){
			GUI.Label(new Rect(0, 20*combinedRatio, Screen.width, 40.0f*combinedRatio), 
			m_winner + " has won!", messageStyle);
			if(GUI.Button(new Rect(199*combinedRatio, 558*combinedRatio, 250.0f*combinedRatio, 200.0f*combinedRatio),"Return to menu"))
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
			m_errorMsg, messageStyle);
			if(GUI.Button(new Rect(199*combinedRatio, 558*combinedRatio, 250.0f*combinedRatio, 200.0f*combinedRatio),"Return to menu"))
			{
				Application.LoadLevel("NetworkMenu");
			}
		}
		else if(m_waitingForTurn){
			GUI.Label(new Rect(0, 20*combinedRatio, Screen.width, 40.0f*combinedRatio), 
			"Waiting for opponent's move...", messageStyle);
		}
	}
}
