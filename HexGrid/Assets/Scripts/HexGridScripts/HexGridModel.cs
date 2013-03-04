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
	
	public float m_movementLeft;
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
		
		m_movementLeft = (float)-0.5;
	}
	
	public void ResetGraphStateVars ()
	{
		m_movementLeft = (float)-0.5;
		m_prevNode = null;
	}
	
	public void SetMovementLeft (float movementLeft) {
		m_movementLeft = movementLeft;
	}
	
	//update the movementLeft and prevNode only if it is larger than the current one
	public void UpdateMovementLeft (GameObject prevNode) {
		float newMovementLeft=prevNode.GetComponent<HexGridModel>().m_movementLeft-(float)1.0;
		if (newMovementLeft > m_movementLeft && newMovementLeft >=0) {
			m_movementLeft = newMovementLeft;
			m_prevNode = prevNode;
		}
	}

	//debugging purpose
	public override string ToString()
	{
		return string.Format("({0},{1})",m_center.x,m_center.y);	
	}
}
