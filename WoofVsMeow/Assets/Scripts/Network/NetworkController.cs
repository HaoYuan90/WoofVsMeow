using UnityEngine;
using System.Collections;

public class NetworkController : MonoBehaviour 
{
	public bool m_isInNetworkMode;
	private string m_gameName = "WoofVsMeow";
	private bool m_refreshing = false;
	private bool m_connected = false;
	private HostData[] m_hostdata;
	
	private float m_width = 200.0f;
	private float m_height = 140.0f;
	private float m_top = 0.0f;
	private float m_left = 0.0f;

	void Start () {
	}
	
	//refresh host list
	void Update () {
		if (m_refreshing)
		{
			if (MasterServer.PollHostList().Length > 0)
			{
				m_refreshing = false;
				m_hostdata = MasterServer.PollHostList();
			}
		}
	}
	
	void OnGUI()
	{
		if(m_isInNetworkMode){
			if (!Network.isServer && !Network.isClient)
			{
				if (GUI.Button (new Rect(m_left,m_top,m_width,m_height), "Initialize Server"))
				{
					InitializeServer();
				}
				if (GUI.Button (new Rect(m_left,m_top + m_height + 15,m_width,m_height), "Refresh Host List"))
				{
					RefreshHostList();
				}
			}
			if(!m_connected){
				if (m_hostdata != null)
				{
					int j=0;
					for (int i=0; i < m_hostdata.Length; i++)
					{
						//check if you can actually join this host
						if (m_hostdata[i].connectedPlayers < m_hostdata[i].playerLimit){
							if (GUI.Button (new Rect(m_left + 200,m_top + j*m_height + 15,m_width,m_height), m_hostdata[i].gameName))
							{
								Network.Connect(m_hostdata[i]);
							}
							j++;
						}
					}
				}	
			}
		}
	}
	
	private void RefreshHostList()
	{
		MasterServer.RequestHostList(m_gameName);
		m_refreshing = true;
	}
	
	private void InitializeServer()
	{
		Network.InitializeServer(1,25001, !Network.HavePublicAddress());
		MasterServer.RegisterHost(m_gameName,"HY's game");
	}
	
	void OnMasterServerEvent( MasterServerEvent mse)
	{
		if (mse == MasterServerEvent.RegistrationSucceeded)
		{
			Debug.Log("Successfully Registered");
		}
	}
	
	void OnServerInitialized()
	{
		Debug.Log ("Server initialized");
	}
	
	void OnConnectedToServer()
	{
		m_connected = true;
	}
}
