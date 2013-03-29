using UnityEngine;
using System.Collections.Generic;

public class UnitController : MonoBehaviour 
{
	private GameEngine m_engine;
	private GameObject m_currentGrid;
	public GameObject currentGrid
	{
		get{return m_currentGrid;}
	}
	//control states
	public int m_control; //which player owns this unit
	private bool m_active;
	private bool m_hideGUI;
	private bool m_hasMoved;
	private bool m_hasAttacked;
	
	readonly private int m_buttonWidth = 100;
	readonly private int m_buttonHeight = 30;
	readonly private int m_buttonXOffset = 20;
	readonly private int m_buttonYOffset = 2;
	readonly private int m_firstButtonOffset = 80;
	
	//unit stats
	public int m_movementRange;
	public int m_attackRange;
	public int m_maxAP;
	
	public void InitialiseUnit (GameEngine engine, GameObject currentGrid) 
	{
		m_engine = engine;
		m_currentGrid = currentGrid;
		m_active = false;
		m_hideGUI = true;
		m_hasMoved = false;
		m_hasAttacked = false;
		
		gameObject.GetComponent<MovementController>().Initialise();
		gameObject.GetComponent<APController>().Initialise(m_maxAP);
		gameObject.GetComponent<AttackController>().Initialise();
	}
	
	public void Activate()
	{
		m_active = true;
		m_hideGUI = false;
		m_hasMoved = false;
		m_hasAttacked = false;
	}
	
	public IntVector2 GetPositionOnMap()
	{
		HexGridModel temp = m_currentGrid.GetComponent<HexGridModel>();
		return new IntVector2(temp.m_row,temp.m_col);
	}
	
	public bool IsEnemyOf(int currControl)
	{
		return !(m_control == currControl);
	}
	
	public void Move(GameObject dest)
	{
		m_hideGUI = true;
		//valid movement, give control to destination
		if(GetComponent<MovementController>().Move(dest))
		{
			m_currentGrid.GetComponent<TnGAttribute>().m_unit = null;
			//give control to destination node
			dest.GetComponent<TnGAttribute>().m_unit = gameObject;
			m_currentGrid = dest;
		}
	}
	
	public void MoveFinished()
	{
		m_hideGUI = false;
		m_hasMoved = true;
	}
	
	public void Attack(GameObject tar)
	{
		m_hideGUI = true;
		gameObject.GetComponent<AttackController>().Attack(tar);
	}
	
	public void AttackFinished()
	{
		m_hideGUI = false;
		m_hasAttacked = true;
		//DO NOT ALLOW MOVE AFTER ATTACK
		m_hasMoved = true;
	}
	
	public void CommandCancelled()
	{
		m_hideGUI = false;
	}
	
	void OnGUI()
	{
		if(m_active == true && !m_hideGUI){
			Vector3 temp = Camera.main.WorldToScreenPoint(gameObject.transform.position);
			float buttonX = temp.x+m_buttonXOffset;
			float buttonY = Screen.height - temp.y-m_firstButtonOffset;
			GUI.enabled = !m_hasMoved; //enable if unit has not moved
			if(GUI.Button(new Rect(buttonX,buttonY,m_buttonWidth,m_buttonHeight),"Move"))
			{
				m_hideGUI = true;
				m_engine.ProcessMovementRange(gameObject,false);
			}
			buttonY += m_buttonHeight+m_buttonYOffset;
			GUI.enabled = !m_hasAttacked; //enable if unit has not attacked
			if(GUI.Button(new Rect(buttonX,buttonY,m_buttonWidth,m_buttonHeight),"Attack"))
			{
				m_hideGUI = true;
				m_engine.ProcessAttackRange(gameObject,false);
			}
			buttonY += m_buttonHeight+m_buttonYOffset;
			GUI.enabled = true;
			if(GUI.Button(new Rect(buttonX,buttonY,m_buttonWidth,m_buttonHeight),"End"))
			{
				m_active = false;
				GetComponent<APController>().ReplenishAP(1);
				m_engine.UnitTurnEnded();
			}
		}
	}
}
