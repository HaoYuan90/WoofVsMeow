using UnityEngine;
using System.Collections.Generic;

public enum ArmorType{
	heavy,
	light,
	flyer
}

public enum AttackType{
	normal,
	pierce,
	strafe
}

public class UnitController : MonoBehaviour 
{
	protected GameEngine m_engine;
	protected GameObject m_currentGrid;
	public GameObject currentGrid
	{
		get{return m_currentGrid;}
	}
	//control states
	public int m_control; //which player owns this unit
	protected bool m_active;
	protected bool m_hasMoved;
	protected bool m_hasAttacked;
	
	//unit stats
	public int m_maxHealth;
	[SerializeField]
	private int m_health;
	public int m_damage;
	public ArmorType m_armorType;
	public AttackType m_attackType;
	public int m_movementRange;
	public int m_attackRange;
	public int m_maxAP;
	
	public void InitialiseUnit (GameEngine engine, GameObject currentGrid) 
	{
		m_engine = engine;
		m_currentGrid = currentGrid;
		m_active = false;
		m_hasMoved = false;
		m_hasAttacked = false;
		
		m_health = m_maxHealth;
		
		GetComponent<MovementController>().Initialise();
		GetComponent<APController>().Initialise(m_maxAP);
		GetComponent<AttackController>().Initialise();
		GetComponent<UnitGUIController>().Initialise(m_health,m_maxHealth);
	}
	
	public void Activate()
	{
		m_active = true;
		m_hasMoved = false;
		m_hasAttacked = false;
		UpdateButtonStatus();
		GetComponent<UnitGUIController>().EnableGUI();
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
		GetComponent<UnitGUIController>().DisableGUI();
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
		m_engine.CameraLookAt(gameObject);
		m_hasMoved = true;
		UpdateButtonStatus();
		if(m_active)
			GetComponent<UnitGUIController>().EnableGUI();
	}
	
	//return damage of this attack
	public int Attack(GameObject tar)
	{
		if(m_active){
			GetComponent<UnitGUIController>().DisableGUI();
			//attacking unit
			if(tar.GetComponent<UnitController>()!= null){
				return GetComponent<AttackController>().AttackUnit(tar);
			}
			//attacking building
			else{
				return GetComponent<AttackController>().AttackBuilding(tar);
			}
		} else {
			Debug.LogWarning("Inactive unit should not be initiating an attack");
			return 0;
		}
	}
	
	public void LoseHealthBy(int amount)
	{
		m_health -= amount;
		GetComponent<UnitGUIController>().OnHealthLostBy(amount);
		if(m_health <= 0){
			m_currentGrid.GetComponent<TnGAttribute>().m_unit = null;
			//death animation?
			m_engine.OnUnitDeath(gameObject);
		}
	}
	
	public void AttackFinished()
	{
		m_hasAttacked = true;
		//DO NOT ALLOW MOVE AFTER ATTACK
		m_hasMoved = true;
		UpdateButtonStatus();
		if(m_active)
			GetComponent<UnitGUIController>().EnableGUI();
	}
	
	public void CommandCancelled()
	{
		GetComponent<UnitGUIController>().EnableGUI();
	}
	
	private void UpdateButtonStatus()
	{
		GetComponent<UnitGUIController>().UpdateButtonStatus(m_hasMoved,m_hasAttacked);
	}
	public virtual void MoveButtonAction()
	{
		GetComponent<UnitGUIController>().DisableGUI();
		m_engine.ProcessMovementRange(gameObject,false);
	}
	public void AttackButtonAction()
	{
		GetComponent<UnitGUIController>().DisableGUI();
		m_engine.ProcessAttackRange(gameObject,false);
	}
	public void EndButtonAction()
	{
		m_active = false;
		GetComponent<UnitGUIController>().DisableGUI();
		GetComponent<APController>().ReplenishAP(1);
		m_engine.UnitTurnEnded();
	}
}
