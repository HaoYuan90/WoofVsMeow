using UnityEngine;
using System.Collections;

public class GameEngine : MonoBehaviour {
	
	private GridLogic m_gridLogic;
	private APSequenceController m_apController;
	
	private GameObject m_currUnit;
	private bool m_inTurn;
	
	private bool m_isReadyToMove;
	private bool m_isReadyToAttack;

	void Start () {
		//boot all the components on start
		InitialiseGridLogic();
		InitUnitsAndBuildings();
		InitialiseAPSequenceController();
		
		m_currUnit = null;
		m_inTurn = false;
		
		m_isReadyToMove = false;
		m_isReadyToAttack = false;
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
		//add buildings to ap list
		if(GameObject.Find ("Buildings")!= null){
			foreach (Transform child in GameObject.Find ("Buildings").transform){
				m_apController.AddNewUnit(child.gameObject);
			}
		}
	}
	
	public void UnitTurnEnded()
	{
		//determine how much ap to replenish according to type of movement
		m_currUnit.GetComponent<APnControlModel>().ReplenishAP(1);
		m_apController.OnTurnEnd();
		m_inTurn = false;
		m_currUnit = null;
	}
	
	public void ProcessMovementRange(GameObject unit)
	{
		MovementController temp = unit.GetComponent<MovementController>();
		m_gridLogic.ProcessMovementRange(temp.m_currentGrid,temp.m_movementRange);
		m_isReadyToMove = true;
	}
	
	public void ProcessAttackRange(GameObject unit)
	{
		AttackController temp = unit.GetComponent<AttackController>();
		m_gridLogic.ProcessAttackRange(temp.m_currentGrid,temp.attackRange);
		m_isReadyToAttack = true;
	}
	
	void Update () 
	{
		if(!m_inTurn){
			m_gridLogic.ClearAllMasks();
			m_currUnit = m_apController.OnTurnBegin();
			if(m_currUnit != null){
				if(m_currUnit.tag == "Unit"){
					m_currUnit.GetComponent<UnitController>().Activate();
					m_inTurn = true;
				}
				else if(m_currUnit.tag == "Building"){
				}
				else{
					Debug.LogWarning("weird stuff is taking its turn now...");
				}
			}
		}
		//actual move
		if(m_isReadyToMove){
			if(Input.GetButtonDown("LeftClick")) {
				RaycastHit grid;
				Ray selection = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(selection,out grid)){				
					GameObject dest = grid.collider.gameObject;
					if(dest.tag == "Grid"){
						//make sure selected node is in range and is not the src node itself
						if(dest.GetComponent<HexGridModel>().m_prevNode != null){
							m_gridLogic.ClearAllMasks();
							dest.GetComponent<MaskManager>().RedMaskOn();
							m_currUnit.GetComponent<UnitController>().Move(dest);
							m_isReadyToMove = false;
						}
					}
				}
			}
		}
		//actual attack
		if(m_isReadyToAttack){
			if(Input.GetButtonDown("LeftClick")) {
				RaycastHit grid;
				Ray selection = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(selection,out grid)){				
					GameObject tar = grid.collider.gameObject;
					if(tar.tag == "Grid"){
						//make sure target is within attack range and it does hold a unit
						if(tar.GetComponent<HexGridModel>().m_prevNode != null)
						{
							//single unit attack
							GameObject unit = tar.GetComponent<TnGAttribute>().m_unit;
							//check if it is enemy
							if(unit!=null){
								m_gridLogic.ClearAllMasks();
								tar.GetComponent<MaskManager>().RedMaskOn();
								m_currUnit.GetComponent<UnitController>().Attack(tar);
								m_isReadyToAttack = false;
							}
						}
					}
				}
			}
		}
		//cancel move/attack
		if(m_isReadyToMove || m_isReadyToAttack){
			//cancel
			if(Input.GetButtonDown("RightClick")) {
				m_gridLogic.ClearAllMasks();
				m_isReadyToMove = false;
				m_isReadyToAttack = false;
				m_currUnit.GetComponent<UnitController>().CommandCancelled();
			}
		}
	}
}
