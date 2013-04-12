using UnityEngine;
using System.Collections;

public class GUIBtmPanelAndMsgs : MonoBehaviour 
{
	private float m_optimalHeight = 900.0f; //height of screen in free aspect ratio
	private float m_optimalWidth = 1600.0f; //width of screen in free aspect ratio
	
	public Texture2D money_Tex;
	public Texture2D commander_Tex;
	
	public GUIStyle boxStyle;//For the boxes carrying textures
	public GUIStyle labelStyle;//For the labels in the bottom panel
	public GUIStyle messageStyle;//For the messages that appear at the top of the screen. They need a larger font
	
	private string m_playerName;
	private double m_money;
	
	private bool m_gameOver;
	private string m_winner;
	private bool m_connectionError;
	private string m_errorMsg;
	private bool m_waitingForTurn;

	// Use this for initialization
	public void Initialise (int money) 
	{
		m_playerName = PlayerPrefs.GetString("playername");
		m_money = money;
		
		m_gameOver = false;
		m_connectionError = false;
		m_waitingForTurn = false;
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
		float ratio = Screen.width/m_optimalWidth;
		//Bottom panel
		GUI.Box(new Rect(0, Screen.height-110.0f*ratio, Screen.width, 110.0f*ratio),"");
		//Name Label
		GUI.Label(new Rect(150.0f*ratio, Screen.height-75.0f*ratio, 300.0f*ratio, 40.0f*ratio), 
			"Name : " + m_playerName, labelStyle);
		//Money texture
		GUI.Box (new Rect(550.0f*ratio, Screen.height-94.0f*ratio, 66.0f*ratio, 66.0f*ratio), 
			money_Tex, boxStyle);
		//Money label
		GUI.Label(new Rect(620.0f*ratio, Screen.height-75.0f*ratio, 200.0f*ratio, 40.0f*ratio), 
			"m_money : $" + m_money.ToString(), labelStyle);
		
		if(m_gameOver){
			GUI.Label(new Rect(0, 20*ratio, Screen.width, 40.0f*ratio), 
			m_winner + " has won!", messageStyle);
			if(GUI.Button(new Rect(199*ratio, 558*ratio, 250.0f*ratio, 200.0f*ratio),"Return to menu"))
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
			GUI.Label(new Rect(0, 0, 200.0f*ratio, 40.0f*ratio), 
			m_errorMsg, messageStyle);
			if(GUI.Button(new Rect(199*ratio, 558*ratio, 250.0f*ratio, 200.0f*ratio),"Return to menu"))
			{
				Network.Disconnect();
				if(Network.isServer){
					MasterServer.UnregisterHost();
				}
				Application.LoadLevel("NetworkMenu");
			}
		}
		else if(m_waitingForTurn){
			GUI.Label(new Rect(0, 20*ratio, Screen.width, 40.0f*ratio), 
			"Waiting for opponent's move...", messageStyle);
		}
	}
}
