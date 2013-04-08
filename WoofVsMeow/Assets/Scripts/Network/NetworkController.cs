using UnityEngine;
using System.Collections;

public class NetworkController : MonoBehaviour 
{
	readonly private string m_gameName = "WoofVsMeow";
	
	private int m_refreshing = 0; //refresh timer
	readonly private int m_refreshTimer = 300;
	private bool m_hostListReceived = false;
	private bool m_isInitialisingServer = false;
	private bool m_isWaitingForPlayers = false;
	private bool m_isConnectingToServer = false;
	
	private HostData[] m_hostdata;
	private int m_control = 1; //valid controls, host is set to 0, hence it start from 1
	
	readonly private float m_width = 200.0f;
	readonly private float m_height = 50.0f;
	readonly private float m_top = 50.0f;
	readonly private float m_left = 50.0f;
	
	//refresh host list
	void Start()
	{
		PlayerPrefs.SetInt("control",0);
		PlayerPrefs.SetInt("clientcontrol",1);
	}
	
	void Update () 
	{
		if (m_refreshing > 0)
		{
			if (m_hostListReceived)
			{
				m_refreshing = 0;
				m_hostListReceived = false;
				m_hostdata = MasterServer.PollHostList();
				return;
			}
			m_refreshing --;
		}
	}
	
	void OnGUI()
	{
		//show messages to players....
		if(m_refreshing > 0){
			GUI.Label(new Rect(m_left,m_top,m_width,m_height), "Contacting servers...");
			return;
		}
		if(m_isInitialisingServer){
			GUI.Label(new Rect(m_left,m_top,m_width,m_height), "Creating server...");
			return;
		}
		if(m_isWaitingForPlayers){
			GUI.Label(new Rect(m_left,m_top,m_width,m_height), "Waiting for players...");
			return;
		}
		if(m_isConnectingToServer){
			GUI.Label(new Rect(m_left,m_top,m_width,m_height), "Connecting to server...");
			return;
		}
		
		if (!Network.isServer && !Network.isClient)
		{
			if (GUI.Button (new Rect(m_left,m_top,m_width,m_height), "Initialize Server"))
			{
				InitializeServer(0);
			}
			if (GUI.Button (new Rect(m_left,m_top + m_height + 15,m_width,m_height), "Refresh Host List"))
			{
				RefreshHostList();
			}
		}
		
		if (m_hostdata != null)
		{
			int j=0;
			if(m_hostdata.Length == 0){
				GUI.Label(new Rect(m_left + 220,m_top + 15,m_width,m_height), "No available host");
			}
			for (int i=0; i < m_hostdata.Length; i++)
			{
				//check if you can actually join this host
				if (m_hostdata[i].connectedPlayers < m_hostdata[i].playerLimit){
					if (GUI.Button (new Rect(m_left + 200,m_top + j*m_height + 15,m_width,m_height), m_hostdata[i].gameName))
					{
						Network.Connect(m_hostdata[i]);
						m_isConnectingToServer = true;
					}
					j++;
				}
			}
		}	
	}
	
	private void RefreshHostList()
	{
		MasterServer.RequestHostList(m_gameName);
		m_refreshing = m_refreshTimer;
	}
	
	private void InitializeServer(int control)
	{
		m_isInitialisingServer = true;
		Network.InitializeServer(1,25001, !Network.HavePublicAddress());
		MasterServer.RegisterHost(m_gameName,PlayerPrefs.GetString("playername")+"'s game");
	}
	
	void OnMasterServerEvent( MasterServerEvent mse)
	{
		if (mse == MasterServerEvent.RegistrationSucceeded)
		{
			//server is registered on masterhost, so now wait for players :D
			m_isInitialisingServer = false;
			m_isWaitingForPlayers = true;
			PlayerPrefs.SetInt("control",0);
		}
		else if(mse == MasterServerEvent.HostListReceived)
		{
			m_hostListReceived = true;
		}
		else{
			PlayerPrefs.SetInt("control",-1);
			//error!!!! no internet or some other shits
		}
	}
	
	void OnServerInitialized()
	{
		//quite useless it seems.....
	}
	
	void OnConnectedToServer()
	{
		//wait for server to load level
	}
	
	void OnFailedToConnect()
	{
		m_isConnectingToServer = false;
	}
	
	void OnPlayerConnected()
	{
		Debug.Log("player connected");
		networkView.RPC("SetControl",RPCMode.Others, m_control);
		m_control ++;
		if(Network.connections.Length == Network.maxConnections)
		{
			networkView.RPC("LoadHostedLevel",RPCMode.AllBuffered, "HYTestScene");
		}
	}
	
	[RPC]
	private void LoadHostedLevel(string levelName)
	{
		Application.LoadLevel(levelName);
	}
	
	[RPC]
	private void SetControl(int control)
	{
		//if player does not hold a valid control now
		PlayerPrefs.SetInt("clientcontrol",control);
	}
}
