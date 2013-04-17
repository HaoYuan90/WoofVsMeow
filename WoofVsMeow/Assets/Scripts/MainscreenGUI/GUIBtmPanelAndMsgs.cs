using UnityEngine;
using System.Collections;

public class GUIBtmPanelAndMsgs : MonoBehaviour 
{
	public Texture2D m_btmBarTex;

	public GUIStyle m_labelStyle;//For the labels in the bottom panel
	public GUIStyle m_messageStyle;//For the messages that appear at the top of the screen. They need a larger font
	public GUIStyle m_buttonStyle;

	private string m_playerName;
	private double m_money;

	private bool m_gameOver;
	private string m_winner;
	private bool m_connectionError;
	private string m_errorMsg;
	private bool m_waitingForTurn;
	
	private Rect m_msgRect = new Rect(0,100,600,100);
	private Rect m_buttonRect = new Rect(0,400,500,100);
	
	//readonly private float m_optimalHeight = 900.0f; //height of screen in free aspect ratio
	readonly private float m_optimalWidth = 1600.0f; //width of screen in free aspect ratio

	// Use this for initialization
	public void Initialise (int money) 
	{
		m_playerName = PlayerPrefs.GetString("playername");
		m_money = money;

		m_gameOver = false;
		m_connectionError = false;
		m_waitingForTurn = false;
		
		float ratio = Screen.width/m_optimalWidth;
		m_msgRect = new Rect(Screen.width/2-m_msgRect.width*ratio/2, m_msgRect.y*ratio, 
			m_msgRect.width*ratio, m_msgRect.height*ratio);
		m_buttonRect = new Rect(Screen.width/2-m_buttonRect.width*ratio/2, m_buttonRect.y*ratio, 
			m_buttonRect.width*ratio, m_buttonRect.height*ratio);
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
		GUI.DrawTexture(new Rect(0, Screen.height-110.0f*ratio, Screen.width, 110.0f*ratio),m_btmBarTex);
		//Name Label
		GUI.Label(new Rect(400.0f*ratio, Screen.height-75.0f*ratio, 300.0f*ratio, 40.0f*ratio), 
			"Name : " + m_playerName, m_labelStyle);
		//Money label
		GUI.Label(new Rect(950.0f*ratio, Screen.height-75.0f*ratio, 200.0f*ratio, 40.0f*ratio), 
			"$$$ : " + m_money.ToString(), m_labelStyle);
		
		if(m_gameOver){
			GUI.Label(m_msgRect, m_winner + " has won!", m_messageStyle);
			if(GUI.Button(m_buttonRect,"Return to menu",m_buttonStyle))
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
			GUI.Label(m_msgRect, m_errorMsg, m_messageStyle);
			if(GUI.Button(m_buttonRect,"Return to menu",m_buttonStyle))
			{
				Network.Disconnect();
				if(Network.isServer){
					MasterServer.UnregisterHost();
				}
				Application.LoadLevel("NetworkMenu");
			}
		}
		else if(m_waitingForTurn){
			GUI.Label(m_msgRect, "Waiting for opponent's move...", m_messageStyle);
		}
	}
}