using UnityEngine;
using System.Collections;

public class GUIBtmPanel : MonoBehaviour 
{
	private float fixedHeight = 598.0f;
	private float fixedWidth = 1366.0f;
	
	private float widthRatio;
	private float heightRatio;
	private float combinedRatio;
	
	public Texture2D money_Tex;
	public Texture2D commander_Tex;
	
	public GUIStyle boxStyle;
	public GUIStyle labelStyle;
	
	private string playerName;
	private double money;
	private string commanderName;

	// Use this for initialization
	void Start () 
	{
		playerName = "Some name";
		money = 0;
		commanderName = "Joker";
	}
	
	// Update is called once per frame
	void Update () 
	{
		widthRatio = Screen.width/fixedWidth;
		heightRatio = Screen.height/fixedHeight;
		if (widthRatio<heightRatio){combinedRatio = widthRatio;}
		else{combinedRatio = heightRatio;}
	}
	
	public void SetPlayerName(string name)
	{
		playerName = name;
	}
	
	public void SetMoney(double money)
	{
		this.money = money;
	}
	
	public void SetCommanderName(string name)
	{
		commanderName = name;
	}
	
	void OnGUI()
	{
		GUI.Box(new Rect(0, Screen.height-110.0f*combinedRatio, Screen.width, 110.0f*combinedRatio),"");
		GUI.Label(new Rect(150.0f*combinedRatio, Screen.height-75.0f*combinedRatio, 300.0f*combinedRatio, 40.0f*combinedRatio), "Name : " + playerName, labelStyle);
		GUI.Box (new Rect(750.0f*combinedRatio, Screen.height-94.0f*combinedRatio, 66.0f*combinedRatio, 66.0f*combinedRatio), money_Tex, boxStyle);
		GUI.Label(new Rect(820.0f*combinedRatio, Screen.height-75.0f*combinedRatio, 200.0f*combinedRatio, 40.0f*combinedRatio), "Money : $" + money.ToString(), labelStyle);
		GUI.Box (new Rect(1050.0f*combinedRatio, Screen.height-94.0f*combinedRatio, 66.0f*combinedRatio, 66.0f*combinedRatio), commander_Tex, boxStyle);
		GUI.Label(new Rect(1120.0f*combinedRatio, Screen.height-75.0f*combinedRatio, 200.0f*combinedRatio, 40.0f*combinedRatio), "Commander : " + commanderName, labelStyle);
	}
}
