using UnityEngine;
using System.Collections;

public struct IntVector2  {
    public int x, y;
 
    public IntVector2 (int xi, int yi) {
        x = xi;
        y = yi;
    }   
}

public class GameEngine : MonoBehaviour {
	
	private GridLogic m_gridLogic;
	private APSequenceController m_apController;
	
	private GameObject m_currUnit;
	private bool m_inTurn;
	
	private bool m_isReadyToMove;
	private bool m_isReadyToAttack;
	private bool m_isReadyToProduce;

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
		m_apController.OnTurnEnd();
		m_inTurn = false;
		m_currUnit = null;
	}
	
	public void ProcessMovementRange(GameObject unit)
	{
		UnitController temp = unit.GetComponent<UnitController>();
		m_gridLogic.ProcessMovementRange(temp.currentGrid,temp.m_movementRange);
		m_isReadyToMove = true;
	}
	
	public void ProcessAttackRange(GameObject unit)
	{
		UnitController temp = unit.GetComponent<UnitController>();
		m_gridLogic.ProcessAttackRange(temp.currentGrid,temp.m_attackRange);
		m_isReadyToAttack = true;
	}

	public void ProcessProductionRange(GameObject unit) {
		m_gridLogic.ProcessProductionRange(unit.GetComponent<BuildingController>().m_currentGrid);
		m_isReadyToProduce = true;
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
					m_currUnit.GetComponent<BuildingController>().Activate();
					m_inTurn = true;
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
						//make sure selected node is in range and there is not units and buildings occupying it
						if (dest.GetComponent<HexGridModel>().m_prevNode != null){
							if (dest.GetComponent<TnGAttribute>().m_unit == null){
								m_gridLogic.ClearAllMasks();
								dest.GetComponent<MaskManager>().RedMaskOn();
								if(Network.isClient || Network.isServer){
									IntVector2 temp1 = m_currUnit.GetComponent<UnitController>().GetPositionOnMap();
									IntVector2 temp2 = dest.GetComponent<HexGridModel>().GetPositionOnMap();
									networkView.RPC("MoveUnitToDest",RPCMode.OthersBuffered,temp1.x,temp1.y,temp2.x,temp2.y);
								}
								m_currUnit.GetComponent<UnitController>().Move(dest);
								m_isReadyToMove = false;
							}
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
							//check if there is unit on that grid
							if(unit!=null){
								int currentControl = m_currUnit.GetComponent<UnitController>().m_control;
								int tarControl = unit.GetComponent<UnitController>().m_control;
								//if it is enemy
								if(currentControl != tarControl){
									m_gridLogic.ClearAllMasks();
									tar.GetComponent<MaskManager>().RedMaskOn();
									if(Network.isClient || Network.isServer){
										IntVector2 temp1 = m_currUnit.GetComponent<UnitController>().GetPositionOnMap();
										IntVector2 temp2 = unit.GetComponent<UnitController>().GetPositionOnMap();
										networkView.RPC("UnitAttackUnit",RPCMode.OthersBuffered,temp1.x,temp1.y,temp2.x,temp2.y);
									}
									m_currUnit.GetComponent<UnitController>().Attack(unit);
									m_isReadyToAttack = false;
								}
							}
						}
					}
				}
			}
		}
		//actual produce
		if (m_isReadyToProduce){
			if(Input.GetButtonDown("LeftClick")) {
				RaycastHit grid;
				Ray selection = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(selection,out grid)){				
					GameObject tar = grid.collider.gameObject;
					if(tar.tag == "Grid"){
						//make sure target is within production range and it does not hold a unit
						if (tar.GetComponent<HexGridModel>().m_prevNode != null) {
							if(tar.GetComponent<TnGAttribute>().m_unit==null) {
								m_gridLogic.ClearAllMasks();
								m_currUnit.GetComponent<BuildingController>().Produce(tar);
								m_isReadyToProduce = false;
							}
						}
					}
				}
			}
		}
		//cancel move/attack
		if(m_isReadyToMove || m_isReadyToAttack || m_isReadyToProduce) {
			//cancel
			if(Input.GetButtonDown("RightClick")) {
				m_gridLogic.ClearAllMasks();
				m_isReadyToMove = false;
				m_isReadyToAttack = false;
				m_isReadyToProduce = false;
				m_currUnit.GetComponent<UnitController>().CommandCancelled();
			}
		}
	}
	
	//network supports, remote procedure calls and their envelopes
	[RPC]
	void MoveUnitToDest(int unitX, int unitY, int destX, int destY)
	{
		GameObject unit = m_gridLogic.GetUnitAt(unitX,unitY);
		Debug.Log(unitX);
		Debug.Log(unitY);
		ProcessMovementRange(unit);
		unit.GetComponent<UnitController>().Move(m_gridLogic.GetGridAt(destX,destY));
	}
	
	[RPC]
	void UnitAttackUnit(int unitX, int unitY, int tarX, int tarY)
	{
		GameObject unit = m_gridLogic.GetUnitAt(unitX,unitY);
		ProcessAttackRange(unit);
		unit.GetComponent<UnitController>().Attack(m_gridLogic.GetUnitAt(tarX,tarY));
	}
}
