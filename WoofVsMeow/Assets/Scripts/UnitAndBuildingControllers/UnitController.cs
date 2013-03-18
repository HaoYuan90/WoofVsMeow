using UnityEngine;
using System.Collections.Generic;

public class UnitController : MonoBehaviour 
{
	private GameEngine m_engine;
	
	public int m_control;
	private bool m_active;
	private bool m_hideGUI;
	
	private bool m_hasMoved;
	private bool m_hasAttacked;
	
	public int m_buttonWidth = 100;
	public int m_buttonHeight = 30;
	public int m_buttonXOffset = 20;
	public int m_buttonYOffset = 2;
	public int m_firstButtonOffset = 80;
	
	public void InitialiseUnit (GameEngine engine, GameObject currentGrid) 
	{
		m_engine = engine;
		m_active = false;
		m_hideGUI = false;
		m_hasMoved = false;
		m_hasAttacked = false;
		
		gameObject.GetComponent<MovementController>().Initialise(currentGrid);
		gameObject.GetComponent<APnControlModel>().Initialise();
		gameObject.GetComponent<AttackController>().Initialise(currentGrid);
	}
	
	public void Activate()
	{
		m_active = true;
		m_hideGUI = false;
		m_hasMoved = false;
		m_hasAttacked = false;
	}
	
	public bool IsEnemyOf(int currControl)
	{
		return !(m_control == currControl);
	}
	
	public void Move(GameObject dest)
	{
		m_hideGUI = true;
		gameObject.GetComponent<MovementController>().Move(dest);
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
				m_engine.ProcessMovementRange(gameObject);
			}
			buttonY += m_buttonHeight+m_buttonYOffset;
			GUI.enabled = !m_hasAttacked; //enable if unit has not attacked
			if(GUI.Button(new Rect(buttonX,buttonY,m_buttonWidth,m_buttonHeight),"Attack"))
			{
				m_hideGUI = true;
				//give attack module the current current grid
				gameObject.GetComponent<AttackController>().m_currentGrid = 
					gameObject.GetComponent<MovementController>().m_currentGrid;
				m_engine.ProcessAttackRange(gameObject);
			}
			buttonY += m_buttonHeight+m_buttonYOffset;
			GUI.enabled = true;
			if(GUI.Button(new Rect(buttonX,buttonY,m_buttonWidth,m_buttonHeight),"End"))
			{
				m_active = false;
				m_engine.UnitTurnEnded();
			}
		}
	}
}
