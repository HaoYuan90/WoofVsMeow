using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingController : MonoBehaviour 
{
	private GameObject m_producibleUnitPrefab;
	public GameObject m_producibleUnitPrefab0, m_producibleUnitPrefab1;
	private List<GameObject> m_producibleUnitPrefabList = new List<GameObject>();
	private GameEngine m_engine;
	public GameObject m_currentGrid;
	public int m_control;
	private bool m_active;
	private bool m_hideGUI;
	private bool m_hasProduced;

	public int m_buttonWidth = 100;
	public int m_buttonHeight = 30;
	public int m_buttonXOffset = 20;
	public int m_buttonYOffset = 2;
	public int m_firstButtonOffset = 80;

	public int m_maxAP;
	public bool m_canProduceGold;

	private int m_floatingTextLife;
	private string m_floatingText;
	private Rect m_textPos;
	private GUIStyle m_textStyle;

	readonly private float m_initHeight = 100;
	readonly private int m_floatingTextMaxLife = 120;
	readonly private Rect m_textBox = new Rect(0,0,100,100);

	public void InitialiseBuilding(GameEngine engine, GameObject currentGrid) {
		m_engine = engine;
		m_currentGrid = currentGrid;
		m_active = false;
		m_hideGUI = false;
		m_hasProduced = false;

		m_floatingTextLife = 0;
		m_floatingText = null;
		m_textPos = new Rect();

		m_producibleUnitPrefabList.Add(m_producibleUnitPrefab0);
		m_producibleUnitPrefabList.Add(m_producibleUnitPrefab1);

		gameObject.GetComponent<APController>().Initialise(m_maxAP);
	}

	public void Activate() {
		m_active = true;
		m_hideGUI = false;
		m_hasProduced = false;
	}

	public bool IsEnemyOf(int currControl) {
		return !(m_control == currControl);
	}

	public void Produce(GameObject tar) {
		m_hideGUI = true;

		m_engine.GetComponent<GameEngine>().playerGold[m_control] -= 100;
		DisplayFloatingText(m_currentGrid, "-100 Gold");

		GameObject newUnit = (GameObject)Instantiate(m_producibleUnitPrefab);
		newUnit.transform.position = new Vector3(tar.transform.position.x,tar.renderer.bounds.max.y,tar.transform.position.z);
		newUnit.transform.parent = GameObject.Find("Units").transform;
		newUnit.GetComponent<UnitController>().m_control = m_control; //for testing purpose only, to be deleted later
		tar.GetComponent<TnGAttribute>().m_unit = newUnit;
		newUnit.GetComponent<UnitController>().InitialiseUnit(m_engine, tar);
		m_engine.GetComponent<APSequenceController>().AddNewUnit(newUnit);

		m_hasProduced = true;
		m_hideGUI = false;
	}

	public void ProduceGold() {
		if (m_control == 0 || m_control == 1) //Will display "+100 Gold" text
		{
			m_engine.GetComponent<GameEngine>().playerGold[m_control] += 100;
			DisplayFloatingText(m_currentGrid, "+100 Gold");
		}
	}

	private void DisplayFloatingText(GameObject target, string msg)
	{
		m_floatingTextLife = m_floatingTextMaxLife;
		m_floatingText = msg;
		Vector3 temp = Camera.main.WorldToScreenPoint(target.transform.position);
		m_textPos = new Rect(temp.x,Screen.height-temp.y-m_initHeight,m_textBox.width,m_textBox.height);
	}

	public void CommandCancelled()
	{
		m_hideGUI = false;
	}

	void OnGUI() {
		if(m_active == true && !m_hideGUI){
			Vector3 temp = Camera.main.WorldToScreenPoint(gameObject.transform.position);
			float buttonX = temp.x+m_buttonXOffset;
			float buttonY = Screen.height - temp.y-m_firstButtonOffset;
			bool canProduce = (m_engine.GetComponent<GameEngine>().playerGold[m_control] >= 100);
			GUI.enabled = !m_hasProduced && canProduce; //enable if building has not produced
				for (int i=0; i < m_producibleUnitPrefabList.Count; i++) {
				if(GUI.Button(new Rect(buttonX,buttonY,m_buttonWidth,m_buttonHeight),m_producibleUnitPrefabList[i].name))
				{
					m_producibleUnitPrefab = m_producibleUnitPrefabList[i];
					m_hideGUI = true;
					m_engine.ProcessProductionRange(gameObject);
				}
				buttonY += m_buttonHeight+m_buttonYOffset;
			}
			GUI.enabled = true;
			if (GUI.Button(new Rect(buttonX,buttonY,m_buttonWidth,m_buttonHeight),"End"))
			{
				m_active = false;
				GetComponent<APController>().ReplenishAP(1);
				m_engine.UnitTurnEnded();
			}
		}
		if(m_floatingTextLife > 0)
		{
			m_textStyle = new GUIStyle(GUI.skin.label);
			m_textStyle.fontSize = 30;
			m_textStyle.fontStyle = FontStyle.Bold;
			m_floatingTextLife --;
			m_textPos = new Rect(m_textPos.x, m_textPos.y-0.25f, m_textBox.width, m_textBox.height);
			GUI.Label(m_textPos, m_floatingText,m_textStyle);
		}
	}
}