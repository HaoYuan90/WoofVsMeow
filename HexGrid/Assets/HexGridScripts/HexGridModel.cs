using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class HexGridModel : ScriptableObject{
	
	[SerializeField]
	private GameObject m_grid;
	[SerializeField]
	private Vector2 m_center;
	[SerializeField]
	private float m_width;
	[SerializeField]
	private float m_height;
	
	public GameObject grid
	{
		get {return m_grid;}
	}
	/* scriptable object does not use constructors
	public HexGridModel(GameObject grid, Vector2 center, float width, float height)
	{
		m_grid = grid;
		m_center = center;
		m_width = width;
		m_height = height;
	}
	*/
	
	public void OnEnable() 
	{ 
		hideFlags = HideFlags.HideInInspector;
	}
	
	public void Initialise (GameObject grid, Vector2 center, float width, float height)
	{
		m_grid = grid;
		m_center = center;
		m_width = width;
		m_height = height;
	}
	
	//debugging purpose
	public override string ToString()
	{
		return string.Format("({0},{1})",m_center.x,m_center.y);	
	}
}
