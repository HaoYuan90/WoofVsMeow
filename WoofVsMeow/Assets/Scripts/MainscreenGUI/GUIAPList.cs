using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIAPList : MonoBehaviour 
{
	public Texture2D apPanelTex;
	public Texture2D unitToMoveTex;
	
	public GUIStyle panelStyle;
	public GUIStyle unitToMoveStyle;
	public GUIStyle labelStyle;
	
	private APSequenceController apsc;
	
	private float portraitX = 22.0f;
	private float portraitY = 18.0f;
	
	private float fixedWidth = 1366.0f;
	private float fixedHeight = 598.0f;
	
	private float widthRatio;
	private float heightRatio;
	
	private float portraitHeight = 80.0f;
	private float portraitWidth = 80.0f;
	private bool onTurnBegin = false;
	private bool onIdle = true;
	private bool firstItem = true;
	private bool canTurnEnd = false;
	//When we perform OnTurnBegin
	private float alpha;
	
	public void Initialise () 
	{
		apsc = GetComponent<APSequenceController>();
	}
	
	public void OnTurnBegin()
	{
		alpha = 0.0f;
		onTurnBegin = true;
		onIdle = false;
		canTurnEnd = false;
	}
	
	public void OnTurnEnd()
	{
		canTurnEnd = false;
	}
	public void OnIdle()
	{
		onTurnBegin = false;
		onIdle = true;
		canTurnEnd = false;
	}
	public bool HasTurnBegun()
	{
		return onTurnBegin;
	}
	public bool IsIdling()
	{
		return onIdle;
	}
	public bool CanTurnEnd()
	{
		return canTurnEnd;
	}
	void OnGUI()
	{
		widthRatio = Screen.width / fixedWidth;
		heightRatio = Screen.height / fixedHeight;
		//GUI.color = Color.green;
		GUI.backgroundColor = Color.grey;
		GUI.Box (new Rect(0,-5*heightRatio,140*widthRatio,500*heightRatio),apPanelTex, panelStyle);
		GUI.backgroundColor = Color.blue;
		GUI.Box (new Rect(140*widthRatio,15*heightRatio,120*widthRatio,120*heightRatio),unitToMoveTex, unitToMoveStyle);
		
		List<GameObject> units = apsc.GetUnits();
		
		portraitY = 38.0f;
		firstItem = true;
		foreach(GameObject e in units)
		{
			if (onTurnBegin)
			{
				if (firstItem)
				{
					GUI.backgroundColor = new Color(0.2f,0.8f,0.8f, 1-alpha);
					GUI.Box(new Rect(portraitX*widthRatio, portraitY*heightRatio, portraitWidth*widthRatio, portraitHeight*heightRatio), e.GetComponent<APController>().ToString());
					GUI.backgroundColor = new Color(0.2f,0.8f,0.8f,alpha);
					GUI.Box(new Rect(160.0f*widthRatio, (portraitY+0.0f)*heightRatio, portraitWidth*widthRatio, portraitHeight*heightRatio), e.GetComponent<APController>().ToString());
					portraitY = (120.0f - alpha*80.0f);
					if (alpha < 1.0f)
					{
						alpha += 0.01f;
					}
					else
					{
						OnIdle();
						canTurnEnd = true;
					}
					firstItem =false;
				}
				else
				{
					GUI.backgroundColor = new Color(0.2f,0.8f,0.8f,1.0f);
					GUI.Box(new Rect(portraitX*widthRatio, portraitY*heightRatio, portraitWidth*widthRatio, portraitHeight*heightRatio), e.GetComponent<APController>().ToString());
					portraitY += 90.0f;
				}
			
				if (portraitY > fixedHeight - 190)
				{
					break;
				}
			}
			else
			{
				if (canTurnEnd)
				{	
					if (firstItem)
					{
						GUI.backgroundColor = new Color(0.2f,0.8f,0.8f,1.0f);
						GUI.Box(new Rect(160.0f*widthRatio, (portraitY+0.0f)*heightRatio, portraitWidth*widthRatio, portraitHeight*heightRatio), e.GetComponent<APController>().ToString());
						firstItem = false;
					}
					else
					{
						GUI.backgroundColor = new Color(0.2f,0.8f,0.8f,1.0f);
						GUI.Box(new Rect(portraitX*widthRatio, portraitY*heightRatio, portraitWidth*widthRatio, portraitHeight*heightRatio), e.GetComponent<APController>().ToString());
						portraitY += 90.0f;
					}
				
					if (portraitY > fixedHeight - 190)
					{
						break;
					}
				}
				else
				{
					GUI.backgroundColor = new Color(0.2f,0.8f,0.8f,1.0f);
					GUI.Box(new Rect(portraitX*widthRatio, portraitY*heightRatio, portraitWidth*widthRatio, portraitHeight*heightRatio), e.GetComponent<APController>().ToString());
					portraitY += 90.0f;
				
					if (portraitY > fixedHeight - 190)
					{
						break;
					}
				}
			}
		}
		
		GUI.Label(new Rect(0,0,126*widthRatio,50*heightRatio), "AP", labelStyle);
		GUI.Label(new Rect(138*widthRatio,0,126*widthRatio,50*heightRatio), "Unit to Move", labelStyle);
	}
}
