using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class HexGridModel :MonoBehaviour{
	
	public Vector2 m_center;
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
	
	public void Initialise (Vector2 center, int row, int col)
	{
		m_center = center;
		m_row = row;
		m_col = col;

        m_movementCost = 0;
		m_movementLeft = -1;
		m_prevNode = null;
	}
	
	public IntVector2 GetPositionOnMap()
	{
		return new IntVector2(m_row,m_col);
	}
	
	public void UpdateEnemyBlockageCost()
	{
		m_movementCost = m_movementCost+1;
		if(m_movementCost > 5)
			m_movementCost = 5;
	}
	
	private void SetMovementCost(bool considerTerrains)
	{
		if(considerTerrains){
			TerrainType temp = GetComponent<TnGAttribute>().m_terrainType;
			switch(temp){
			case TerrainType.normal:
				m_movementCost = 1;
				break;
			case TerrainType.forest:
				m_movementCost = 2;
				break;
				//obstacles will not be counted in algorithm
			case TerrainType.obstacle:
				m_movementCost = 0;
				break;
			}
		}
		else
			m_movementCost = 1;
	}
	
	public void ResetGraphStateVars (bool considerTerrains)
	{
		SetMovementCost(considerTerrains);
		m_movementLeft = -1;
		m_prevNode = null;
	}
	
	//if this movementLeft is greater, replace its parent node
	public void UpdateMovementLeft (GameObject prevNode, int costToReach)
	{
		int movementLeftPrev = prevNode.GetComponent<HexGridModel>().m_movementLeft;
		int newMovementLeft = movementLeftPrev-costToReach;
		if (newMovementLeft > m_movementLeft && newMovementLeft >= 0) {
			m_movementLeft = newMovementLeft;
			m_prevNode = prevNode;
		}
	}
	
	public void UpdateRangeLeft (GameObject prevNode, int newRange)
	{
		if (newRange > m_movementLeft && newRange >= 0) {
			m_movementLeft = newRange;
			m_prevNode = prevNode;
		}
	}
	
	//determine if unit carrying currentUnitControl can pass this grid
	public bool CanPass(int currentUnitControl, bool flying)
	{
		bool canPass = true;
		TnGAttribute tng = GetComponent<TnGAttribute>();
		if(tng.m_building != null)
		{
			int tempControl = tng.m_building.GetComponent<BuildingController>().m_control;
			if (tempControl != currentUnitControl)
				canPass = false;
		}
		if(tng.m_unit != null)
		{
			if(tng.m_unit.GetComponent<UnitController>().m_control != currentUnitControl)
				canPass = false;
		}
		if(!flying){
			if(tng.m_terrainType == TerrainType.obstacle)
				canPass = false;
		}
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
