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
	private bool m_isSelectingMap = false; //indicates whether user is currently selecting a map.

	
	public string testName1;
	public string testName2;
	public string testName3;
	public string testName4;
	public string testName5;
	public string testName6;
	public string testName7;

    public GUIStyle buttonStyle; //Style used for buttons
    public GUIStyle messageStyle; //Style used for the messages
    public GUIStyle m_hostListStyle; //Style used for the host list selection grid
	public GUIStyle m_mapListStyle; //Style used for the map selection grid
	public GUIStyle m_mapNameLabelStyle;
	public GUIStyle m_mapDescLabelStyle;
	
	//Map names
	private string[] m_mapNames;
	
	//Map Descriptions
	private string[] m_mapDescs;
	
    private HostData[] m_hostdata;
    private int m_control = 1; //valid controls, host is set to 0, hence it start from 1

    //Can modify button width/button height. Code will auto-centre everything
    private float m_buttonWidth = 350.0f; //OPTIMAL = 350
    private float m_buttonHeight = 100.0f; //OPTIMAL = 100

    private float m_buttonX;
    private float m_buttonY;
	//Height of the cells in the selection grid. (OPTIMAL 50~100)
    private float m_cellHeight = 50.0f;

    private int m_overflow; //to modify the selectrion grid if there is an overflow

    private float m_messageHeight = 80.0f; //Height of the bottom panel and messages

    private float m_hostListWidth = 700.0f; //Width of the host list OPTIMAL = 700
    private float m_hostListHeight = 600.0f;//Height of the host list OPTIMAL = 600

    private float m_optimalHeight = 900.0f; //Screen height in 16:9 ratio
    private float m_optimalWidth = 1600.0f; //Screen width in 16:9 ratio

    private float m_screenLimit; //Limit the ratio for the sake of the host list.

    private int m_selectionIndex = -1; //The current selected index of the selection grid
    private Vector2 m_scrollPos = Vector2.zero; //The current scroll position of the scroll view

    private List<string> m_playerNames = new List<string>(); //A list of strings containing player names
	
	//The buttons are all centred with respect to the host list as well as the button height.
	//So this ratio will help account for unideal button height values (!= 100)
	private float m_listButtonRatio;

    //refresh host list
    void Start()
    {
        PlayerPrefs.SetInt("control", 0);
        PlayerPrefs.SetInt("clientcontrol", 1);
		PlayerPrefs.SetInt("map", -1);
        m_screenLimit = 1200.0f / m_optimalWidth;
		
		string[] strArray = {testName1,testName2,testName3,testName4,testName5,testName6,testName7};
		
		SetMaps (strArray, null);
		m_listButtonRatio = (m_hostListHeight/m_buttonHeight)/6.0f;
    }
	
	public void SetMaps(string[] mapNames, string[] mapDescs)
	{
		m_mapNames = mapNames;
		m_mapDescs = mapDescs;
	}

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
        float ratio = Screen.width / m_optimalWidth;

        //This is because the Host List overflows the screen when screen width is bigger than 1200.0f
        //Makes the gui looks ok if the screen size is too big
        if (ratio > m_screenLimit)
        {
            ratio = m_screenLimit;
        }
		
		//m_buttonY is set to be 1/2 button height below the top of the host list.
        m_buttonY = Screen.height / 2 - (m_hostListHeight / 2) * ratio + (m_hostListHeight - m_buttonHeight * 5*m_listButtonRatio) / 2 * ratio;
		//m_buttonX is set to centre the button between the host list and the left edge of the screen
        m_buttonX = (Screen.width / 2 - (m_hostListWidth / 2) * ratio) / 2 - 0.5f * m_buttonWidth* ratio;

        //Bottom panel for displaying messages
        GUI.Box(new Rect(0, Screen.height - (m_messageHeight * ratio), Screen.width, m_messageHeight * ratio), "");

        //Background box for the host list (Centered)
        GUI.Box(new Rect(Screen.width / 2 - (m_hostListWidth / 2) * ratio, Screen.height / 2 - (m_hostListHeight / 2) * ratio,
            m_hostListWidth * ratio, m_hostListHeight * ratio), "");
		
		//Box for displaying map name and desc
		//Ymin value of the box = Ymin value of the host list
		//X value of the box is centered between the host list and the right side of the screen.
		GUI.Box (new Rect(Screen.width - (m_buttonX + m_buttonWidth) * ratio, m_buttonY - (0.5f*m_listButtonRatio*m_buttonHeight)*ratio, m_buttonWidth * ratio, (m_hostListHeight - 1.5f*m_listButtonRatio*m_buttonHeight) * ratio), "");

        //Label for the host list
		if (!m_isSelectingMap)
		{
        	GUI.Label(new Rect(Screen.width / 2 - (m_hostListWidth / 2) * ratio, Screen.height / 2 - (m_hostListHeight / 2 + m_messageHeight + 10) * ratio, m_hostListWidth * ratio, m_messageHeight * ratio), "Existing Hosts", messageStyle);
		}
		else
		{
			GUI.Label(new Rect(Screen.width / 2 - (m_hostListWidth / 2) * ratio, Screen.height / 2 - (m_hostListHeight / 2 + m_messageHeight + 10) * ratio, m_hostListWidth * ratio, m_messageHeight * ratio), "Available Maps", messageStyle);
		}

        //show messages to players....
        if (m_refreshing > 0)
        {
            GUI.Box(new Rect(0, Screen.height - (m_messageHeight * ratio), Screen.width, m_messageHeight * ratio), "Contacting servers...", messageStyle);
            return;
        }
        if (m_isInitialisingServer)
        {
            GUI.Box(new Rect(0, Screen.height - (m_messageHeight * ratio), Screen.width, m_messageHeight * ratio), "Creating server...", messageStyle);
            return;
        }
        if (m_isWaitingForPlayers)
        {
            GUI.Box(new Rect(0, Screen.height - (m_messageHeight * ratio), Screen.width, m_messageHeight * ratio), "Waiting for players...", messageStyle);
            if (GUI.Button(new Rect(m_buttonX * ratio, m_buttonY, m_buttonWidth * ratio, m_buttonHeight * ratio), "Cancel", buttonStyle))
            {
                UnregisterServer();
            }
            return;
        }
		
		//If player presses initialize server.
		if (m_isSelectingMap)
		{
			 if (m_mapNames.Length > m_hostListHeight / m_cellHeight - 4)
                {
                    m_overflow = 1;
                }
                else
                {
                    m_overflow = 0;
                }
			
				
                //First rect (position) is same as the position for the hostdata box
                //Second rectangle has the width extended to place the scrollbar outside the box
                //Attribute overflow will expand the viewRect only when there is actually an overflow.
                m_scrollPos = GUI.BeginScrollView(new Rect(Screen.width / 2 - (m_hostListWidth / 2) * ratio, Screen.height / 2 - (m_hostListHeight / 2) * ratio,
                    (m_hostListWidth + 25) * ratio, (m_hostListHeight) * ratio), m_scrollPos,
                        new Rect(Screen.width / 2 - (m_hostListWidth / 2) * ratio, Screen.height / 2 - (m_hostListHeight / 2) * ratio,
                            (m_hostListWidth) * ratio, (m_hostListHeight + (m_overflow * (m_hostListHeight / m_cellHeight - 4) * m_cellHeight)) * ratio), false, true);

                m_selectionIndex = GUI.SelectionGrid(new Rect(Screen.width / 2 - (m_hostListWidth / 2) * ratio, Screen.height / 2 - (m_hostListHeight / 2) * ratio, m_hostListWidth * ratio, m_mapNames.Length * m_cellHeight), m_selectionIndex, m_mapNames, 1, m_mapListStyle);

                GUI.EndScrollView();
			
				if (m_selectionIndex >= 0)
                {
					//The Box and Label responsible for displaying map data.
					GUI.Box(new Rect(Screen.width - (m_buttonX + m_buttonWidth) * ratio, m_buttonY - (0.5f*m_listButtonRatio*m_buttonHeight)*ratio, m_buttonWidth * ratio, (m_messageHeight) * ratio), m_mapNames[m_selectionIndex], m_mapNameLabelStyle);
					GUI.Label(new Rect(Screen.width - (m_buttonX + m_buttonWidth) * ratio, m_buttonY - (0.5f*m_listButtonRatio*m_buttonHeight)*ratio + (m_messageHeight) * ratio, m_buttonWidth * ratio, (m_hostListHeight - 1.5f*m_listButtonRatio*m_buttonHeight) * ratio - (m_messageHeight) * ratio), m_mapNames[m_selectionIndex],m_mapDescLabelStyle);
					
					//Create a new host on the selected map button
                    if (GUI.Button(new Rect(Screen.width - (m_buttonX + m_buttonWidth) * ratio, m_buttonY + m_buttonHeight * 4.5f*m_listButtonRatio * ratio, m_buttonWidth * ratio, m_buttonHeight * ratio), "Create a new host on " + m_mapNames[m_selectionIndex], buttonStyle))
                    {
						m_isSelectingMap = false;
						PlayerPrefs.SetInt("map", m_selectionIndex);
                        InitializeServer(0);
                    }
                }
			GUI.Box(new Rect(0, Screen.height - (m_messageHeight * ratio), Screen.width, m_messageHeight * ratio), "Please select a map", messageStyle);
			if (GUI.Button(new Rect(m_buttonX * ratio, m_buttonY , m_buttonWidth * ratio, m_buttonHeight * ratio), "Cancel", buttonStyle))
            {
                m_isSelectingMap = false;
            }
		}
		
        if (m_isConnectingToServer)
        {
            GUI.Box(new Rect(0, Screen.height - (m_messageHeight * ratio), Screen.width, m_messageHeight * ratio), "Connecting to server...", messageStyle);
            if (GUI.Button(new Rect(m_buttonX * ratio, m_buttonY, m_buttonWidth * ratio, m_buttonHeight * ratio), "Cancel", buttonStyle))
            {
                UndoConnectionToServer();
            }
            return;
        }

        if (!Network.isServer && !Network.isClient && !m_isSelectingMap)
        {
            //The initialize server, refresh hosts and return buttons
			//Centred to the left of the host list.
            if (GUI.Button(new Rect(m_buttonX * ratio, m_buttonY, m_buttonWidth * ratio, m_buttonHeight * ratio), "Initialize Server",buttonStyle))
            {
                m_isSelectingMap = true;
            }
            if (GUI.Button(new Rect(m_buttonX * ratio, m_buttonY + m_buttonHeight * 2*m_listButtonRatio * ratio, m_buttonWidth * ratio, m_buttonHeight * ratio), "Refresh Host List", buttonStyle))
            {
                RefreshHostList();
            }
            if (GUI.Button(new Rect(m_buttonX * ratio, m_buttonY + m_buttonHeight * 4*m_listButtonRatio * ratio, m_buttonWidth * ratio, m_buttonHeight * ratio), "Return", buttonStyle))
            {
                Application.LoadLevel("MainMenu");
            }
        }

        if (m_hostdata != null)
        {
            if (m_hostdata.Length == 0)
            {
                //If no hosts can be detected, send this message
                GUI.Box(new Rect(0, Screen.height - m_messageHeight * ratio, Screen.width, m_messageHeight * ratio), "No available host", messageStyle);
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
                
                m_scrollPos = GUI.BeginScrollView(new Rect(Screen.width / 2 - (m_hostListWidth / 2) * ratio, Screen.height / 2 - (m_hostListHeight / 2) * ratio,
                    (m_hostListWidth + 25) * ratio, (m_hostListHeight) * ratio), m_scrollPos,
                        new Rect(Screen.width / 2 - (m_hostListWidth / 2) * ratio, Screen.height / 2 - (m_hostListHeight / 2) * ratio,
                            (m_hostListWidth) * ratio, (m_hostListHeight + (m_overflow * (m_hostListHeight / m_cellHeight - 4) * m_cellHeight)) * ratio), false, true);

                m_selectionIndex = GUI.SelectionGrid(new Rect(Screen.width / 2 - (m_hostListWidth / 2) * ratio, Screen.height / 2 - (m_hostListHeight / 2) * ratio, m_hostListWidth * ratio, m_hostdata.Length * m_cellHeight), m_selectionIndex, m_playerNames.ToArray(), 1, m_mapListStyle);

                GUI.EndScrollView();

                //Only if the selected host still can accept new players
                if (m_selectionIndex >= 0 && m_hostdata[m_selectionIndex].connectedPlayers < m_hostdata[m_selectionIndex].playerLimit)
                {
                    //The Box and Label responsible for displaying map data.
					GUI.Box(new Rect(Screen.width - (m_buttonX + m_buttonWidth) * ratio, m_buttonY - (0.5f*m_listButtonRatio*m_buttonHeight)*ratio, m_buttonWidth * ratio, (m_messageHeight) * ratio), m_mapNames[int.Parse(m_hostdata[m_selectionIndex].comment)], m_mapNameLabelStyle);
					GUI.Label(new Rect(Screen.width - (m_buttonX + m_buttonWidth) * ratio, m_buttonY - (0.5f*m_listButtonRatio*m_buttonHeight)*ratio + (m_messageHeight) * ratio, m_buttonWidth * ratio, (m_hostListHeight - 1.5f*m_listButtonRatio*m_buttonHeight) * ratio - (m_messageHeight) * ratio), m_mapNames[int.Parse(m_hostdata[m_selectionIndex].comment)],m_mapDescLabelStyle);
					
					//Connect to the selected host button
                    if (GUI.Button(new Rect(Screen.width - (m_buttonX + m_buttonWidth) * ratio, m_buttonY + m_buttonHeight * 4.5f*m_listButtonRatio * ratio, m_buttonWidth * ratio, m_buttonHeight * ratio), "Connect to " + m_playerNames[m_selectionIndex], buttonStyle))
                    {
                        Network.Connect(m_hostdata[m_selectionIndex]);
                        m_isConnectingToServer = true;
                    }
                }

            }
        }
    }
	
	/*private GUIContent[] FormatHostList()
	{
		GUIContent[] content = new GUIContent[m_hostdata.Length];
		
		for (int i=0; i<m_hostdata.Length; i++)
		{
			content[i] = new GUIContent(m_hostdata[i].gameName, m_mapTextures[int.Parse(m_hostdata[i].comment)]);
		}
		return content;
	}*/
				

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
        MasterServer.RegisterHost(m_gameName, PlayerPrefs.GetString("playername") + "'s game", PlayerPrefs.GetInt("map").ToString());
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
            networkView.RPC("LoadHostedLevel", RPCMode.AllBuffered, "HYTestScene");
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