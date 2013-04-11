using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingController : MonoBehaviour 
{
	private GameEngine m_engine;
	private GameObject m_currentGrid;
	public GameObject currentGrid
	{
		get{return m_currentGrid;}
	}
	public int m_control;
	private bool m_active;
	
	public bool m_isBase;
	private int m_unitToProduce;
	public List<GameObject> m_unitList;
	public List<int> m_unitCostList;
	public int m_maxHealth;
	[SerializeField]
	private int m_health;
	public int m_maxAP;

	public void InitialiseBuilding(GameEngine engine, GameObject currentGrid) 
	{
		m_engine = engine;
		m_currentGrid = currentGrid;
		m_active = false;
		
		m_health = m_maxHealth;
		
		GetComponent<APController>().Initialise(m_maxAP);
		GetComponent<BuildingGUIController>().Initialise(m_health, m_maxHealth);
	}
	
	public IntVector2 GetPositionOnMap()
	{
		HexGridModel temp = m_currentGrid.GetComponent<HexGridModel>();
		return new IntVector2(temp.m_row,temp.m_col);
	}
	
	public void Activate() 
	{
		m_active = true;
		GetComponent<BuildingGUIController>().InitGUI();
	}

	public bool IsEnemyOf(int currControl) 
	{
		return !(m_control == currControl);
	}
	
	public void LoseHealthBy(int amount, int attackerControl)
	{
		m_health -= amount;
		GetComponent<BuildingGUIController>().OnHealthLostBy(amount);
		//building is occupied
		if(m_health <= 0){
			if(!m_isBase){
				m_health = m_maxHealth;
				m_control = attackerControl;
				GetComponent<BuildingGUIController>().ResetHealth();
				GetComponent<APController>().ReplenishAP(1);
			}else //if this building is a base, game ends, this player loses
			{
				m_engine.OnGameEnds(attackerControl);
			}
		}
	}
	
	public int ProduceUnitAt(GameObject tar) 
	{
		m_engine.m_playerGold[m_control] -= m_unitCostList[m_unitToProduce];
		GetComponent<BuildingGUIController>().OnGoldChangeBy(-m_unitCostList[m_unitToProduce]);
		GameObject newUnit = (GameObject)Instantiate(m_unitList[m_unitToProduce]);
		newUnit.name = m_unitList[m_unitToProduce].name;
		//moved to game engine, too much coupling
		m_engine.PlaceUnitAt(newUnit, tar);
		
		if(m_active)
			GetComponent<BuildingGUIController>().EnableGUI();
		
		return m_unitToProduce;
	}
	
	public void RPCProduceUnitAt(GameObject tar, int index) 
	{
		m_unitToProduce = index;
		ProduceUnitAt(tar);
	}

	public void ProduceGold() 
	{
		if (m_control != -1) //Will display "+100 Gold" text
		{
			m_engine.m_playerGold[m_control] += 100;
			GetComponent<BuildingGUIController>().OnGoldChangeBy(100);
		}
		m_engine.OnGoldChange();
	}

	public void CommandCancelled()
	{
		GetComponent<BuildingGUIController>().InitGUI();
	}
	
	public List<bool> GetUnitListStatus()
	{
		List<bool> status = new List<bool>();
		for (int i=0; i < m_unitCostList.Count; i++) {
			bool temp = (m_engine.GetComponent<GameEngine>().m_playerGold[m_control] >= m_unitCostList[i]);
			status.Add (temp);
		}
		return status;
	}
	
	public void EndButtonAction()
	{
		m_active = false;
		GetComponent<BuildingGUIController>().DisableGUI();
		GetComponent<APController>().ReplenishAP(1);
		m_engine.UnitTurnEnded();
	}
	
	public void BuildButtonAction(int index)
	{
		m_unitToProduce = index;
		GetComponent<BuildingGUIController>().DisableGUI();
		m_engine.ProcessProductionRange(gameObject);
	}
}