using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class HexGridModel :MonoBehaviour{
	
	public Vector2 m_center;
	public float m_width;
	public float m_height;
	public int m_row;
	public int m_col;
	
	//graph algo related state variables
	private int m_movementLeft; 
	/*NOTE: 
	 * value 0 means movement stoped at this grid
	 * value positive means movement continues from this grid
	 * value -1 means this grid is not reached
	*/
	private float m_distToDest;
	public GameObject m_prevNode;
	
	public int movementLeft
	{
		get{return m_movementLeft;}
	}
	
	public float distToDest
	{
		get{return m_distToDest;}
	}
	
	public void Initialise (Vector2 center, float width, float height, int row, int col)
	{
		m_center = center;
		m_width = width;
		m_height = height;
		m_row = row;
		m_col = col;

		m_movementLeft = -1;
	}
	
	
	
	public void ResetGraphStateVars ()
	{
		m_movementLeft = -1;
		m_distToDest = int.MaxValue;
		m_prevNode = null;
	}
	
	//if this movementLeft is greater, replace 
	//otherwise ignore
	public void UpdateMovementLeft (int movementLeftPrev)
	{
		int movementLeftThis = movementLeftPrev-1; // -cost of the grid, to be implemented
		m_movementLeft = (m_movementLeft < movementLeftThis) ? movementLeftThis: m_movementLeft;
	}
	
	public void InitMovementLeft (int movementLeft)
	{
		m_movementLeft = movementLeft;
	}
	
	public void InitDistToDest(GameObject destNode) 
	{
		Vector2 destCenter = destNode.GetComponent<HexGridModel>().m_center;
		m_distToDest= Vector2.Distance(m_center,destCenter);
	} 
	
	public bool CanPass(String currentPlayerTag)
	{
		bool canPass = true;
		TnGAttribute attri = GetComponent<TnGAttribute>();
		if(attri.m_unit != null)
		{
			//compare tag
		}
		if(attri.m_terrainType == TerrainType.obstacle)
			canPass = false;
		return canPass;
	}
	
	//debugging purpose
	public override string ToString()
	{
		return string.Format("({0},{1})",m_center.x,m_center.y);	
	}
}
