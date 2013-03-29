using UnityEngine;
using System.Collections;

public class BuildingController : MonoBehaviour {

	public GameObject m_producibleUnitPrefab;
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
	
	public void InitialiseBuilding(GameEngine engine, GameObject currentGrid) {
		m_engine = engine;
		m_currentGrid = currentGrid;
		m_active = false;
		m_hideGUI = false;
		m_hasProduced = false;
		
		gameObject.GetComponent<APController>().Initialise(100);
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
		GameObject newUnit = (GameObject)Instantiate(m_producibleUnitPrefab);
		newUnit.transform.position = new Vector3(tar.transform.position.x,tar.renderer.bounds.max.y,tar.transform.position.z);
		newUnit.transform.parent = GameObject.Find("Units").transform;
		newUnit.GetComponent<UnitController>().InitialiseUnit(m_engine, tar);
		tar.GetComponent<TnGAttribute>().m_unit = newUnit;
		m_engine.GetComponent<APSequenceController>().AddNewUnit(newUnit);
		m_hasProduced = true;
		m_hideGUI = false;
	}
	
	void OnGUI() {
		if(m_active == true && !m_hideGUI){
			Vector3 temp = Camera.main.WorldToScreenPoint(gameObject.transform.position);
			float buttonX = temp.x+m_buttonXOffset;
			float buttonY = Screen.height - temp.y-m_firstButtonOffset;
			GUI.enabled = !m_hasProduced && (m_currentGrid.GetComponent<TnGAttribute>().m_unit == null); //enable if building has not produced and there no unit on it
			if(GUI.Button(new Rect(buttonX,buttonY,m_buttonWidth,m_buttonHeight),"Produce"))
			{
				m_hideGUI = true;
				m_engine.ProcessProductionRange(gameObject);
			}
			buttonY += m_buttonHeight+m_buttonYOffset;
			GUI.enabled = true;
			if (GUI.Button(new Rect(buttonX,buttonY,m_buttonWidth,m_buttonHeight),"End"))
			{
				m_active = false;
				m_engine.UnitTurnEnded();
			}
		}
	}
}
