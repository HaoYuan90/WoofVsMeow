using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class HexGridModel :MonoBehaviour{
	
	public Vector2 m_center;
	public float m_width;
	public float m_height;
	public int m_x;
	public int m_y;
	
	public void Initialise (Vector2 center, float width, float height, int x, int y)
	{
		m_center = center;
		m_width = width;
		m_height = height;
		m_x = x;
		m_y = y;
	}
	
	//debugging purpose
	public override string ToString()
	{
		return string.Format("({0},{1})",m_center.x,m_center.y);	
	}
}
