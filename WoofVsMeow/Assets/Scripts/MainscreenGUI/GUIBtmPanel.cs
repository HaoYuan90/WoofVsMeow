using UnityEngine;
using System.Collections;

public class GUIBtmPanel : MonoBehaviour 
{
	private float fixedHeight = 598.0f;
	private float fixedWidth = 1366.0f;
	
	private float widthRatio;
	private float heightRatio;
	
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
		GUI.Box(new Rect(0, Screen.height-110.0f*heightRatio, Screen.width, 110.0f*heightRatio),"");
		GUI.Label(new Rect(150.0f*widthRatio, Screen.height-75.0f*heightRatio, 300.0f*widthRatio, 40.0f*heightRatio), "Name : " + playerName, labelStyle);
		GUI.Box (new Rect(750.0f*widthRatio, Screen.height-94.0f*heightRatio, 66.0f*widthRatio, 66.0f*heightRatio), money_Tex, boxStyle);
		GUI.Label(new Rect(820.0f*widthRatio, Screen.height-75.0f*heightRatio, 200.0f*widthRatio, 40.0f*heightRatio), "Money : $" + money.ToString(), labelStyle);
		GUI.Box (new Rect(1050.0f*widthRatio, Screen.height-94.0f*heightRatio, 66.0f*widthRatio, 66.0f*heightRatio), commander_Tex, boxStyle);
		GUI.Label(new Rect(1120.0f*widthRatio, Screen.height-75.0f*heightRatio, 200.0f*widthRatio, 40.0f*heightRatio), "Commander : " + commanderName, labelStyle);
	}
}
