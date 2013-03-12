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
	public int m_movementCost;
	public int m_movementLeft; 

	/*NOTE: 
	 * value 0 means movement stoped at this grid
	 * value positive means movement continues from this grid
	 * value -1 means this grid is not reached
	 */

	public GameObject m_prevNode;
	
	public void Initialise (Vector2 center, float width, float height, int row, int col)
	{
		m_center = center;
		m_width = width;
		m_height = height;
		m_row = row;
		m_col = col;

        //movement cost is set in editor
		m_movementLeft = -1;
	}
	
	
	public void ResetGraphStateVars ()
	{
		m_movementLeft = -1;
		m_prevNode = null;
	}
	
	//if this movementLeft is greater, replace its parent node
	public void UpdateMovementLeft (GameObject prevNode)
	{
		int movementLeftPrev = prevNode.GetComponent<HexGridModel>().m_movementLeft;
		int newMovementLeft = movementLeftPrev-m_movementCost;
		if (newMovementLeft > m_movementLeft && newMovementLeft >= 0) {
			m_movementLeft = newMovementLeft;
			m_prevNode = prevNode;
		}
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
	
	/*legacy A* path finding heuristics
		public void InitDistToDest(GameObject destNode) 
	{
		Vector2 destCenter = destNode.GetComponent<HexGridModel>().m_center;
		m_distToDest= Vector2.Distance(m_center,destCenter);
	} */
}
