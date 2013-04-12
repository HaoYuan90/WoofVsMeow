using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkController : MonoBehaviour 
{
	readonly private string m_gameName = "WoofVsMeow";

	private int m_refreshing = 0; //refresh timer
	readonly private int m_refreshTimer = 300;
	private bool m_hostListReceived = false;
	private bool m_isInitialisingServer = false;
	private bool m_isWaitingForPlayers = false;
	private bool m_isConnectingToServer = false;
	
	public GUIStyle buttonStyle; //Style used for buttons
	public GUIStyle messageStyle; //Style used for the messages
	public GUIStyle m_selectionGridStyle; //Style used for the messages

	private HostData[] m_hostdata;
	private int m_control = 1; //valid controls, host is set to 0, hence it start from 1

	private float m_optimalHeight = 900.0f; //Screen height in fixed Aspect ratio
	private float m_optimalWidth = 1600.0f; //Screen width in fixed Aspect ratio

	private int selectionIndex = -1; //The current selected index of the selection grid
	private Vector2 scrollPos = Vector2.zero; //The current scroll position of the scroll view
	
	private List<string> m_playerNames = new List<string>(); //A list of strings containing player names

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
				//Once the host list can be pulled, we clear the current list of names
				m_playerNames.Clear();
				//And for each hostdata in the host list, we pull out the player name
				//and add it to the list
				for (int i=0; i < m_hostdata.Length; i++)
				{
					m_playerNames.Add(m_hostdata[i].gameName);
				}
				return;
			}
			m_refreshing --;
		}
	}

	void OnGUI()
	{
		float ratio = Screen.width/m_optimalWidth;
		//Bottom panel for displaying messages
		GUI.Box (new Rect(0,Screen.height-(80*ratio),Screen.width,80*ratio),"");
		
		//Background box for the host list
		GUI.Box (new Rect(900*ratio,150*ratio,250*ratio,250*ratio),"");
		
		//Label for the host list
		GUI.Label(new Rect(950*ratio,10*ratio,150*ratio,140*ratio), "Existing Hosts",messageStyle);
		
		//show messages to players....
		if(m_refreshing > 0){
			GUI.Box(new Rect(0,Screen.height-(80*ratio), Screen.width, 80*ratio), "Contacting servers...", messageStyle);
			return;
		}
		if(m_isInitialisingServer){
			GUI.Box(new Rect(0, Screen.height-(80*ratio), Screen.width, 80*ratio), "Creating server...",messageStyle);
			return;
		}
		if(m_isWaitingForPlayers){
			GUI.Box(new Rect(0, Screen.height-(62*ratio), Screen.width, 62*ratio), "Waiting for players...",messageStyle);
			if (GUI.Button (new Rect(200*ratio, 100*ratio, 250*ratio, 150*ratio), "Cancel"))
			{
				UnregisterServer();
			}
			return;
		}
		if(m_isConnectingToServer){
			GUI.Box(new Rect(0, Screen.height-(80*ratio), Screen.width, 80*ratio), "Connecting to server...",messageStyle);
			if (GUI.Button (new Rect(200*ratio, 100*ratio, 250*ratio, 150*ratio), "Cancel"))
			{
				UndoConnectionToServer();
			}
			return;
		}

		if (!Network.isServer && !Network.isClient)
		{
			//The initialize server and refresh hosts buttons
			if (GUI.Button (new Rect(200*ratio, 100*ratio, 250*ratio, 100*ratio), "Initialize Server"))
			{
				InitializeServer(0);
			}
			if (GUI.Button (new Rect(200*ratio, 300*ratio,250*ratio, 100*ratio), "Refresh Host List"))
			{
				RefreshHostList();
			}
			if (GUI.Button (new Rect(200*ratio, 500*ratio,250*ratio, 100*ratio), "Return"))
			{
				Application.LoadLevel("MainMenu");
			}
		}

		if (m_hostdata != null)
		{
			if(m_hostdata.Length == 0)
			{
				//If no hosts can be detected, send this message
				GUI.Box(new Rect(0, Screen.height-80*ratio, Screen.width, 80*ratio), "No available host",messageStyle);
			}
			
			else
			{
				//Begin and End scroll view generate the scrollable host list
				//The sentence below is to keep the scroll position of the scroll view constantly updated with user input
				scrollPos = GUI.BeginScrollView(new Rect(900*ratio,150*ratio,300*ratio,250*ratio),scrollPos, 
					new Rect(900*ratio,150*ratio,250*ratio,(250+50*m_hostdata.Length)*ratio),false,true);
				//Selection grid auto generates a list of buttons(which are based on the list of player names) and only 1 can be selected
				//selection index needs to be constantly updated with the index of the currently selected button
				selectionIndex = GUI.SelectionGrid(new Rect(900*ratio,150*ratio,250*ratio,50*m_hostdata.Length*ratio), selectionIndex, m_playerNames.ToArray(), 1, m_selectionGridStyle);
				GUI.EndScrollView();				
				
				//Only if the selected host still can accept new players
				if (selectionIndex >= 0 && m_hostdata[selectionIndex].connectedPlayers < m_hostdata[selectionIndex].playerLimit)
				{
					//do we show the connect button that allows user to connect to host
					if (GUI.Button (new Rect(900*ratio,400*ratio,250*ratio,100*ratio), "Connect to "+m_playerNames[selectionIndex]))
					{
						Network.Connect(m_hostdata[selectionIndex]);
						m_isConnectingToServer = true;
					}
				}
			}
		}	
	}
	
	private void UndoConnectionToServer()
	{
		Network.Disconnect();
		m_isConnectingToServer = false;
	}
	
	private void UnregisterServer()
	{
		Network.Disconnect();
		if(Network.isServer){
			MasterServer.UnregisterHost();
		}
		m_isWaitingForPlayers = false;
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