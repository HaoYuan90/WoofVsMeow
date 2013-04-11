using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//this GUI is controlled by APSequenceController
public class GUIAPList : MonoBehaviour 
{
	public List<Texture2D> m_unitPortraits;
	
	private bool m_isAnimating = false;
	private float m_alpha;
	
	private float m_portraitX;
	private float m_portraitY;
	private float m_portraitSize;
	private float m_firstPortraitSize;
	private float m_portraitYOffset;
	//gui runs optimally in 16:9
	readonly private float m_optimalWidth = 1600.0f;
	readonly private float m_optimalHeight = 900.0f;
	
	public void Initialise () 
	{
		m_portraitSize = 100.0f*Screen.width/m_optimalWidth;
		m_firstPortraitSize = 120.0f*Screen.width/m_optimalWidth;
		m_portraitX = 20.0f*Screen.width/m_optimalWidth;
		m_portraitY = 20.0f*Screen.width/m_optimalWidth;
		m_portraitYOffset = 5.0f*Screen.width/m_optimalWidth;
	}
	
	//display GUI for a new turn, called in APSequenceController
	//start animation
	public void OnTurnBegin()
	{
		m_alpha = 0.0f;
		m_isAnimating = true;
	}
	
	private void AnimationFinished()
	{
		m_isAnimating = false;
	}
	
	void OnGUI()
	{	
		List<GameObject> units = GetComponent<APSequenceController>().GetUnits();
		int limit = 0; //only show top 8 units in aplist
		
		float currentY = m_portraitY;
		foreach(GameObject e in units)
		{
			if(limit >= 7)
				break;
			Texture2D portrait = m_unitPortraits[0]; //assign this to pacify the compiler...
			//locate the portrait
			foreach(Texture2D text in m_unitPortraits){
				if(e.name == text.name){
					portrait = text;
					break;
				}
			}
			
			if (m_isAnimating)
			{
				if (limit == 0)
				{
					GUI.contentColor = new Color(1.0f,1.0f,1.0f, m_alpha);
					GUI.Box(new Rect(m_portraitX , currentY, m_firstPortraitSize, m_firstPortraitSize), portrait);
					currentY += (m_firstPortraitSize+m_portraitYOffset);
					GUI.contentColor = new Color(1.0f,1.0f,1.0f, 1-m_alpha);
					GUI.Box(new Rect(m_portraitX, currentY, m_portraitSize , m_portraitSize ), portrait);
					currentY += (1-m_alpha)*m_portraitSize;
					if (m_alpha < 1.0f)
					{
						m_alpha += 0.01f;
					}
					else
					{
						AnimationFinished();
					}
				}
				else
				{
					GUI.contentColor = new Color(1.0f,1.0f,1.0f, 1.0f);
					GUI.Box(new Rect(m_portraitX , currentY , m_portraitSize , m_portraitSize ), portrait);
					currentY += (m_portraitSize+m_portraitYOffset);
				}
			}
			else
			{
				GUI.contentColor = new Color(1.0f,1.0f,1.0f, 1.0f);
				if (limit == 0)
				{
					GUI.Box(new Rect(m_portraitX , currentY, m_firstPortraitSize, m_firstPortraitSize), portrait);
					currentY += (m_firstPortraitSize+m_portraitYOffset);
				}
				else
				{
					GUI.Box(new Rect(m_portraitX , currentY , m_portraitSize , m_portraitSize ), portrait);
					currentY += (m_portraitSize+m_portraitYOffset);
				}

			}
			limit ++;
		}
	}
}
