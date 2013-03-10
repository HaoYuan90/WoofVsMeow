using UnityEngine;
using System.Collections;

//game logic unit?
//pass control to players and stuff

public class Controller : MonoBehaviour 
{
	private int control;
	private PriorityQueue pq;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	public void SetControl(int c)
	{
		control = c;
	}
}
