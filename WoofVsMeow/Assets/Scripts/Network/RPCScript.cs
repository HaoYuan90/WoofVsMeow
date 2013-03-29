using UnityEngine;
using System.Collections;

public class RPCScript : MonoBehaviour 
{
	 bool scream = false;
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	
	void OnTriggerEnter()
	{
		networkView.RPC("Scream", RPCMode.AllBuffered);
	}
	
	void OnTriggerExit()
	{
		networkView.RPC("Unscream", RPCMode.AllBuffered);
	}
	
	[RPC]
	void Scream()
	{
		scream = true;
	}
	
	[RPC]
	void Unscream()
	{
		scream = false;
	}
	
	void OnGUI()
	{
		if (scream)
		{
			GUI.Label(new Rect(500,200,200,100),"SCREAM!!!");
		}
	}
}
