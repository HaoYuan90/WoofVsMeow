using UnityEngine;
using System.Collections;

public class GameEngine : MonoBehaviour {
	
	private GridLogic m_gridLogic;
	private APSequenceController m_apController;
	
	private GameObject m_unitToMove;
	private bool m_inTurn;
	
	private bool m_isReadyToMove;

	void Start () {
		//boot all the components on start
		InitialiseGridLogic();
		InitUnitsAndBuildings();
		InitialiseAPSequenceController();
		
		m_unitToMove = null;
		m_inTurn = false;
		
		m_isReadyToMove = false;
	}
	
	private void InitUnitsAndBuildings()
	{
		m_gridLogic.InitUnitsAndBuildings(this);
	}
	
	private void InitialiseGridLogic()
	{
		m_gridLogic = GetComponent<GridLogic>();
		m_gridLogic.Initialise();
	}
	
	private void InitialiseAPSequenceController()
	{
		m_apController = GetComponent<APSequenceController>();
		m_apController.Initialise();
		//add units to ap list
		if(GameObject.Find ("Units")!= null){
			foreach (Transform child in GameObject.Find ("Units").transform){
				m_apController.AddNewUnit(child.gameObject);
			}
		}
	}
	
	public void UnitTurnEnded()
	{
		//determine how much ap to replenish according to type of movement
		m_unitToMove.GetComponent<APnControlModel>().ReplenishAP(1);
		m_apController.OnTurnEnd();
		m_inTurn = false;
		m_unitToMove = null;
	}
	
	public void ProcessMovementRange(GameObject unit)
	{
		MovementController temp = unit.GetComponent<MovementController>();
		m_gridLogic.ProcessMovementRange(temp.m_currentGrid,temp.m_movementRange);
		m_isReadyToMove = true;
	}
	
	void Update () 
	{
		if(!m_inTurn){
			m_gridLogic.ClearAllMasks();
			m_unitToMove = m_apController.OnTurnBegin();
			if(m_unitToMove != null){
				m_unitToMove.GetComponent<UnitController>().Activate();
				m_inTurn = true;
			}
		}
		
		if(m_isReadyToMove){
			if(Input.GetButtonDown("LeftClick")) {
				RaycastHit grid;
				Ray selection = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(selection,out grid)){				
					GameObject dest = grid.collider.gameObject;
					//make sure selected node is in range and is not the src node itself
					if(dest.GetComponent<HexGridModel>().m_prevNode != null){
						m_gridLogic.ClearAllMasks();
						dest.GetComponent<MaskManager>().RedMaskOn();
						m_unitToMove.GetComponent<UnitController>().Move(dest);
						m_isReadyToMove = false;
					}
				}
			}
		}
		/*
		if(Input.GetButtonDown("Fire1")) {
			RaycastHit grid;
			Ray selection = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(selection,out grid)){				
				if (!m_unitSelected) { //select an unit
					//Debug.Log("Clicking on: "+grid.collider.gameObject.GetComponent<HexGridModel>().m_row.ToString()
					//					+", "+grid.collider.gameObject.GetComponent<HexGridModel>().m_col.ToString());
					m_gridLogic.ClearAllMasks();
					m_selectedGrid=grid.collider.gameObject;
					m_selectedGrid.GetComponent<MaskManager>().RedMaskOn();
					if(m_selectedGrid.GetComponent<TnGAttribute>().m_unit != null){
						m_unitSelected = true;
						GameObject unit = m_selectedGrid.GetComponent<TnGAttribute>().m_unit;
						m_gridLogic.HighlightMovementRange(m_selectedGrid, unit.GetComponent<MovementController>().m_movementRange);
					}
				}
				else { //if unit is already selected, choose the destination
					//Debug.Log("Destination: "+grid.collider.gameObject.GetComponent<HexGridModel>().m_row.ToString()
					//					+", "+grid.collider.gameObject.GetComponent<HexGridModel>().m_col.ToString());
					GameObject dest = grid.collider.gameObject;
					//make sure selected node is in range and is not the src node itself
					if(dest.GetComponent<HexGridModel>().m_prevNode != null){
						GameObject unit = m_selectedGrid.GetComponent<TnGAttribute>().m_unit;
						unit.GetComponent<MovementController>().Move(m_gridLogic.GetMovementPath(m_selectedGrid,dest));
						dest.GetComponent<MaskManager>().RedMaskOn();
						m_unitSelected = false;
					}
				}
			}
		}*/
	}
}
