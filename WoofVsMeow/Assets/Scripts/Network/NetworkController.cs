using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkController : MonoBehaviour
{
    readonly private string m_gameName = "WoofVsMeow";
	
	private HostData[] m_hostdata;
    private int m_control = 1; //valid controls, host is set to 0, hence it start from 1
    private int m_refreshing = 0; //refresh timer
    readonly private int m_refreshTimer = 300;
    private bool m_hostListReceived = false;
    private bool m_isInitialisingServer = false;
    private bool m_isWaitingForPlayers = false;
    private bool m_isConnectingToServer = false;
	private bool m_isSelectingMap = false;
	private string m_hostedMapName;
	
	public List<string> m_mapNames = new List<string>(); //Map names
	private List<string> m_playerNames = new List<string>(); //A list of strings containing player names
	
	public Texture2D m_bgTex;
	public Texture2D m_btmBarTex;
	
    public GUIStyle m_buttonStyle; //Style used for buttons
    public GUIStyle m_mesageStyle; //Style used for the messages
    public GUIStyle m_listStyle; //Style used for selection grid
	public GUIStyle m_mapNameLabelStyle;
	
	private string m_btmMsg;
	
    //Can modify button width/button height. Code will auto-centre everything
    private float m_buttonWidth = 400.0f; 
    private float m_buttonHeight = 80.0f;

    private float m_buttonX;
    private float m_buttonY;
	//Height of the cells in the selection grid. (OPTIMAL 50~100)
    private float m_cellHeight = 50.0f;

    private int m_overflow; //to modify the selectrion grid if there is an overflow
	
	private Rect m_listRect = new Rect(700,140,700,600);
    private float m_hostListHeight = 600.0f;//Height of the host list OPTIMAL = 600

    private float m_screenLimit; //Limit the ratio for the sake of the host list.

    private int m_selectionIndex = -1; //The current selected index of the selection grid
    private Vector2 m_scrollPos = Vector2.zero; //The current scroll position of the scroll view
	
	//The buttons are all centred with respect to the host list as well as the button height.
	//So this ratio will help account for unideal button height values (!= 100)
	private float m_listButtonRatio;
	
	readonly private float m_optimalWidth = 1600.0f; //Screen width in 16:9 ratio
    //readonly private float m_optimalHeight = 900.0f; //Screen height in 16:9 ratio

    //refresh host list
    void Start()
    {
        PlayerPrefs.SetInt("control", 0);
        PlayerPrefs.SetInt("clientcontrol", 1);
		m_btmMsg = "";
		m_hostedMapName = "";
		
		float ratio = Screen.width / m_optimalWidth;
		m_listRect = new Rect(m_listRect.x*ratio, m_listRect.y*ratio,
			m_listRect.width*ratio, m_listRect.height*ratio);
		PlayerPrefs.SetInt("map", -1);
        m_screenLimit = 1200.0f / m_optimalWidth;
		
		m_listButtonRatio = (m_hostListHeight/m_buttonHeight)/6.0f;
    }
	
	//to refresh hostlist
    void Update()
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
                for (int i = 0; i < m_hostdata.Length; i++)
                {
                    m_playerNames.Add(m_hostdata[i].gameName);
                }
                return;
            }
            m_refreshing--;
        }
    }

    void OnGUI()
    {
		bool inAction = false;
		m_btmMsg = "";
		float ratio = Screen.width / m_optimalWidth;
		//draw background
		GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),m_bgTex);
		//draw btm bar
		GUI.DrawTexture(new Rect(0, Screen.height-110.0f*ratio, Screen.width, 110.0f*ratio),m_btmBarTex);
		
        //This is because the Host List overflows the screen when screen width is bigger than 1200.0f
        //Makes the gui looks ok if the screen size is too big
        if (ratio > m_screenLimit)
        {
            ratio = m_screenLimit;
        }
		
		//m_buttonY is set to be 1/2 button height below the top of the host list.
        m_buttonY = m_listRect.y+m_hostListHeight/6;
		//m_buttonX is set to centre the button between the host list and the left edge of the screen
        m_buttonX = m_listRect.x/2+m_buttonWidth*ratio-300f;

        //Background box for the host list (Centered)
        GUI.Box(m_listRect, "");
		
		Rect labelRect = new Rect(m_listRect.x,80f*ratio,m_listRect.width,100f*ratio);
        //Label for the host list
		if (!m_isSelectingMap)
		{
        	GUI.Label(labelRect, "Existing Hosts", m_mesageStyle);
		}
		else
		{
			GUI.Label(labelRect, "Available Maps", m_mesageStyle);
		}
		
		Rect firstButtonRect = new Rect(m_buttonX * ratio, m_buttonY, m_buttonWidth * ratio, m_buttonHeight * ratio);
		Rect secButtonRect = new Rect(firstButtonRect.x, firstButtonRect.y + firstButtonRect.height*1.5f*m_listButtonRatio, 
				firstButtonRect.width, firstButtonRect.height);
		Rect thirdButtonRect = new Rect(firstButtonRect.x, firstButtonRect.y + firstButtonRect.height*3f*m_listButtonRatio, 
				firstButtonRect.width, firstButtonRect.height);
		Rect fourthButtonRect = new Rect(firstButtonRect.x, firstButtonRect.y + firstButtonRect.height*4.5f*m_listButtonRatio, 
				firstButtonRect.width, firstButtonRect.height);
        //show messages to players....
        if (m_refreshing > 0)
        {
            m_btmMsg = "Contacting servers...";
			inAction = true;
        }
        if (m_isInitialisingServer)
        {
            m_btmMsg = "Creating server...";
			inAction = true;
        }
        if (m_isWaitingForPlayers)
        {
            m_btmMsg = "Waiting for players...";
			inAction = true;
            if (GUI.Button(firstButtonRect, "Cancel", m_buttonStyle))
            {
                UnregisterServer();
            }
        }
        if (m_isConnectingToServer)
        {
            m_btmMsg = "Connecting to server...";
			inAction = true;
            if (GUI.Button(firstButtonRect, "Cancel", m_buttonStyle))
            {
                UndoConnectionToServer();
            }
        }
		
		if(!inAction){
	        if (!Network.isServer && !Network.isClient && !m_isSelectingMap)
	        {
	            //The initialize server, refresh hosts and return buttons
				//Centred to the left of the host list.
	            if (GUI.Button(firstButtonRect, "Initialize Server",m_buttonStyle))
	            {
					m_selectionIndex = -1;
	                m_isSelectingMap = true;
	            }
	            if (GUI.Button(secButtonRect, "Refresh Host List", m_buttonStyle))
	            {
					m_selectionIndex = -1;
	                RefreshHostList();
	            }
	            if (GUI.Button(thirdButtonRect, "Return", m_buttonStyle))
	            {
	                Application.LoadLevel("MainMenu");
	            }
	        }
			
			//If player presses initialize server.
			if (m_isSelectingMap)
			{
				 if (m_mapNames.Count > m_hostListHeight / m_cellHeight - 4)
	            {
	                m_overflow = 1;
	            }
	            else
				{
					m_overflow = 0;
				}
	            m_scrollPos = GUI.BeginScrollView(m_listRect,m_scrollPos,m_listRect, false, true);
	
	            m_selectionIndex = GUI.SelectionGrid(new Rect(m_listRect.x, m_listRect.y, m_listRect.width, m_mapNames.Count * m_cellHeight), 
					m_selectionIndex, m_mapNames.ToArray(), 1, m_listStyle);
	
	            GUI.EndScrollView();
			
				if (m_selectionIndex >= 0)
	            {
					//Create a new host on the selected map button
	                if (GUI.Button(fourthButtonRect, "Create Host", m_buttonStyle))
	                {
						m_isSelectingMap = false;
						m_hostedMapName = m_mapNames[m_selectionIndex];
	                    InitializeServer(0);
	                }
	            }
				m_btmMsg = "Please select a map";
				if (GUI.Button(firstButtonRect, "Cancel", m_buttonStyle))
	            {
	                m_isSelectingMap = false;
	            }
			} else if (m_hostdata != null){
	            if (m_hostdata.Length == 0)
	            {
	                //If no hosts can be detected, send this message
	                 m_btmMsg = "No available hosts";
	            }
	            else
	            {
	                if (m_hostdata.Length > m_hostListHeight / m_cellHeight - 4)
	                {
	                    m_overflow = 1;
	                }
	                else
	                {
	                    m_overflow = 0;
	                }
	                
	                m_scrollPos = GUI.BeginScrollView(m_listRect,m_scrollPos,m_listRect, false, true);
	
	                m_selectionIndex = GUI.SelectionGrid(new Rect(m_listRect.x, m_listRect.y, m_listRect.width, m_mapNames.Count * m_cellHeight),
						m_selectionIndex, m_playerNames.ToArray(), 1, m_listStyle);
	
	                GUI.EndScrollView();
	
	                //Only if the selected host still can accept new players
	                if (m_selectionIndex >= 0 && m_hostdata[m_selectionIndex].connectedPlayers < m_hostdata[m_selectionIndex].playerLimit)
					{	
						//Connect to the selected host button
	                    if (GUI.Button(fourthButtonRect, "Connect", m_buttonStyle))
	                    {
	                        Network.Connect(m_hostdata[m_selectionIndex]);
	                        m_isConnectingToServer = true;
	                    }
	                }
	            }
	        }
		}
		GUI.Label(new Rect(Screen.width/2-300f*ratio, Screen.height-90.0f*ratio, 600.0f*ratio, 40.0f*ratio), 
			m_btmMsg, m_mesageStyle);
    }			

    private void UndoConnectionToServer()
    {
        Network.Disconnect();
        m_isConnectingToServer = false;
    }

    private void UnregisterServer()
    {
        Network.Disconnect();
        if (Network.isServer)
        {
            MasterServer.UnregisterHost();
        }
        m_isWaitingForPlayers = false;
		m_hostedMapName = "";
    }

    private void RefreshHostList()
    {
        MasterServer.RequestHostList(m_gameName);
        m_refreshing = m_refreshTimer;
    }

    private void InitializeServer(int control)
    {
        m_isInitialisingServer = true;
        Network.InitializeServer(1, 25001, !Network.HavePublicAddress());
        MasterServer.RegisterHost(m_gameName, PlayerPrefs.GetString("playername")+" on "+m_hostedMapName);
    }

    void OnMasterServerEvent(MasterServerEvent mse)
    {
        if (mse == MasterServerEvent.RegistrationSucceeded)
        {
            //server is registered on masterhost, so now wait for players :D
            m_isInitialisingServer = false;
            m_isWaitingForPlayers = true;
            PlayerPrefs.SetInt("control", 0);
        }
        else if (mse == MasterServerEvent.HostListReceived)
        {
            m_hostListReceived = true;

        }
        else
        {
            PlayerPrefs.SetInt("control", -1);
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
        networkView.RPC("SetControl", RPCMode.Others, m_control);
        m_control++;
        if (Network.connections.Length == Network.maxConnections)
        {
            networkView.RPC("LoadHostedLevel", RPCMode.AllBuffered, m_hostedMapName);
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
        PlayerPrefs.SetInt("clientcontrol", control);
    }
}