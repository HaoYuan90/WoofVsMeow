using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (BuildingGUIController))]
public class BuildingController : MonoBehaviour 
{
	private int m_producibleUnitNo;
	public List<GameObject> m_producibleUnitPrefabList;
	public List<int> m_producibleUnitPriceList;
	private GameEngine m_engine;
	public GameObject m_currentGrid;
	public int m_control;
	private bool m_active;
	
	public int m_maxHealth;
	[SerializeField]
	private int m_health;
	public ArmorType m_armorType;
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

	public void Activate() {
		m_active = true;
		GetComponent<BuildingGUIController>().EnableGUI();
	}

	public bool IsEnemyOf(int currControl) {
		return !(m_control == currControl);
	}
	
	public void LoseHealthBy(int amount)
	{
		m_health -= amount;
		GetComponent<BuildingGUIController>().OnHealthLostBy(amount);
		if(m_health <= 0){
			m_currentGrid.GetComponent<TnGAttribute>().m_unit = null;
			//death animation?
			m_engine.OnUnitDeath(gameObject);
		}
	}
	
	public void Produce(GameObject tar) {
		GetComponent<BuildingGUIController>().DisableGUI();

		m_engine.GetComponent<GameEngine>().playerGold[m_control] -= m_producibleUnitPriceList[m_producibleUnitNo];
		GetComponent<BuildingGUIController>().OnGoldChangeBy(-m_producibleUnitPriceList[m_producibleUnitNo]);
		GameObject newUnit = (GameObject)Instantiate(m_producibleUnitPrefabList[m_producibleUnitNo]);
		newUnit.transform.position = new Vector3(tar.transform.position.x,tar.renderer.bounds.max.y,tar.transform.position.z);
		newUnit.transform.parent = GameObject.Find("Units").transform;
		//newUnit.GetComponent<UnitController>().m_control = m_control; //for testing purpose only, to be deleted later
		tar.GetComponent<TnGAttribute>().m_unit = newUnit;
		newUnit.GetComponent<UnitController>().InitialiseUnit(m_engine, tar);
		m_engine.GetComponent<APSequenceController>().AddNewUnit(newUnit);

		GetComponent<BuildingGUIController>().EnableGUI();
	}

	public void ProduceGold() {
		if (m_control == 0 || m_control == 1) //Will display "+100 Gold" text
		{
			m_engine.GetComponent<GameEngine>().playerGold[m_control] += 100;
			GetComponent<BuildingGUIController>().OnGoldChangeBy(100);
		}
	}

	public void CommandCancelled()
	{
		GetComponent<BuildingGUIController>().EnableGUI();
	}
	
	/*
	void OnGUI() {
		if(m_active == true && !m_hideGUI){
			Vector3 temp = Camera.main.WorldToScreenPoint(gameObject.transform.position);
			float buttonX = temp.x+m_buttonXOffset;
			float buttonY = Screen.height - temp.y-m_firstButtonOffset;
			for (int i=0; i < m_producibleUnitPriceList.Count; i++) {
				GUI.enabled = (m_engine.GetComponent<GameEngine>().playerGold[m_control] >= m_producibleUnitPriceList[i]);
				if(GUI.Button(new Rect(buttonX,buttonY,m_buttonWidth,m_buttonHeight),m_producibleUnitPrefabList[i].name))
				{
					m_producibleUnitNo = i;
					GetComponent<BuildingGUIController>().DisableGUI();
					m_engine.ProcessProductionRange(gameObject);
				}
				buttonY += m_buttonHeight+m_buttonYOffset;
			}
			GUI.enabled = true;
			if (GUI.Button(new Rect(buttonX,buttonY,m_buttonWidth,m_buttonHeight),"End"))
			{
				m_active = false;
				GetComponent<BuildingGUIController>().DisableGUI();
				GetComponent<APController>().ReplenishAP(1);
				m_engine.UnitTurnEnded();
			}
		}
	}*/
}