using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct IntVector2  {
    public int x, y;
 
    public IntVector2 (int xi, int yi) {
        x = xi;
        y = yi;
    }   
}

public class GameEngine : MonoBehaviour 
{
	private int m_control;
	private GridLogic m_gridLogic;
	private APSequenceController m_apController;
	private GameObject m_camera;
	
	private GameObject m_currUnit;
	private bool m_inTurn;
	
	private bool m_isReadyToMove;
	private bool m_isReadyToAttack;
	private bool m_isReadyToProduce;
	public List<int> m_playerGold = new List<int>();

	void Start () 
	{
		if(Network.isServer)
			m_control = PlayerPrefs.GetInt("control");
		else if(Network.isClient)
			m_control = PlayerPrefs.GetInt("clientcontrol");
		else
			m_control = 0; //no network
		//boot all the components on start
		InitialiseGridLogic();
		InitUnitsAndBuildings();
		InitialiseAPSequenceController();
		InitialiseCamera();
		
		m_currUnit = null;
		m_inTurn = false;
		
		m_isReadyToMove = false;
		m_isReadyToAttack = false;
		
		//2 players
		m_playerGold.Add(1000);
		m_playerGold.Add(1000);
		
		GetComponent<GUIAPList>().Initialise();
		GetComponent<GUIBtmPanelAndMsgs>().Initialise(m_playerGold[m_control]);
	}
	
	private void InitialiseCamera()
	{
		m_camera = GameObject.Find("Main Camera");
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
				//Only Add non-neutral buildings into AP Sequence
				if (child.gameObject.GetComponent<BuildingController>().m_control != -1)
					m_apController.AddNewUnit(child.gameObject);
			}
		}
	}
	
	public void UnitTurnEnded()
	{
		//determine how much ap to replenish according to type of movement
		if(Network.isClient || Network.isServer){
			networkView.RPC("PlayerTurnEnded",RPCMode.OthersBuffered,m_currUnit.GetComponent<APController>().currentAP);
		}
		m_apController.OnTurnEnd();
		m_inTurn = false;
		m_currUnit = null;
	}
	
	//called when someone's gold has changed, may not be the player using this client
	//change the gui displaying gold amount
	public void OnGoldChange()
	{
		GetComponent<GUIBtmPanelAndMsgs>().SetMoney(m_playerGold[m_control]);
	}
	
	//add building to aplist
	public void OnNeutralBuildingCaptured(GameObject building)
	{
		m_apController.AddNewUnit(building);
	}
	
	//remove unit from ap list and destroy it
	public void OnUnitDeath(GameObject unit)
	{
		m_apController.RemoveUnit(unit);
		StartCoroutine(DestroyUnit(unit));
	}
	
	IEnumerator DestroyUnit(GameObject unit) 
	{
        yield return new WaitForSeconds(1.0f);
       	Destroy(unit);
    }
	
	public void OnGameEnds(int winnerControl)
	{
		if(winnerControl == m_control){
			networkView.RPC("VictoryClaimedBy",RPCMode.AllBuffered,PlayerPrefs.GetString("playername"));
		}
	}
	
	public void ProcessMovementRange(GameObject unit, bool isRPC)
	{
		UnitController temp = unit.GetComponent<UnitController>();
		m_gridLogic.ProcessMovementRange(temp.currentGrid,temp.m_movementRange, isRPC);
		m_isReadyToMove = true;
	}
	
	public void ProcessFlyingRange(GameObject unit, bool isRPC)
	{
		UnitController temp = unit.GetComponent<UnitController>();
		m_gridLogic.ProcessFlyingRange(temp.currentGrid,temp.m_movementRange, isRPC);
		m_isReadyToMove = true;
	}
	
	public void ProcessAttackRange(GameObject unit, bool isRPC)
	{
		UnitController temp = unit.GetComponent<UnitController>();
		m_gridLogic.ProcessAttackRange(temp.currentGrid,temp.m_attackRange, isRPC);
		m_isReadyToAttack = true;
	}

	public void ProcessProductionRange(GameObject unit) 
	{
		m_gridLogic.ProcessProductionRange(unit.GetComponent<BuildingController>().currentGrid);
		m_isReadyToProduce = true;
	}
	
	public void PlaceUnitAt(GameObject unit, GameObject grid)
	{
		unit.transform.position = new Vector3(grid.transform.position.x,grid.renderer.bounds.max.y,grid.transform.position.z);
		unit.transform.parent = GameObject.Find("Units").transform;
		//set grid attributes
		grid.GetComponent<TnGAttribute>().m_unit = unit;
		unit.GetComponent<UnitController>().InitialiseUnit(this, grid);
		//add unit to aplist
		m_apController.AddNewUnit(unit);
		OnGoldChange();
	}
	
	public void CameraLookAt(GameObject unit)
	{
		float tempX = unit.transform.position.x;
		float tempY = unit.transform.position.y + 20f;
		float tempZ = unit.transform.position.z + 20f;
		m_camera.transform.position = new Vector3(tempX,tempY,tempZ);
		m_camera.transform.LookAt(unit.transform);
	}
	
	private void ProcessTurnBegin ()
	{
		if(m_currUnit != null){
			if(m_currUnit.tag == "Unit"){
				m_inTurn = true;
				int thisControl = m_currUnit.GetComponent<UnitController>().m_control;
				if(thisControl == m_control){
					//only activate the object if it is yours
					GetComponent<GUIBtmPanelAndMsgs>().OnMyTurn();
					m_currUnit.GetComponent<UnitController>().Activate();
					//set camera
					CameraLookAt(m_currUnit);
				}else{
					GetComponent<GUIBtmPanelAndMsgs>().OnOthersTurn();
				}
			}
			else if(m_currUnit.tag == "Building"){
				m_inTurn = true;
				int thisControl = m_currUnit.GetComponent<BuildingController>().m_control;
				if (m_currUnit.GetComponent<BuildingController>().m_unitCostList.Count == 0){
					if(thisControl == m_control){
						CameraLookAt(m_currUnit);
						StartCoroutine("WaitForTurnEnd",3f);
					}
        		    m_currUnit.GetComponent<BuildingController>().ProduceGold();
				} else {	
					if(thisControl == m_control){
						m_currUnit.GetComponent<BuildingController>().Activate();
						CameraLookAt(m_currUnit);
					}
        		    m_currUnit.GetComponent<BuildingController>().ProduceGold();
				}
			}
			else{
				Debug.LogWarning("weird stuff is taking its turn now...");
			}
		}
	}
	
	IEnumerator WaitForTurnEnd (float seconds) {
        yield return new WaitForSeconds(seconds);
       	m_currUnit.GetComponent<APController>().ReplenishAP(1);
		UnitTurnEnded();
    }
	
	private void ProcessUnitAttack(GameObject tar)
	{
		//make sure selected node is in range
		if(tar.GetComponent<HexGridModel>().m_prevNode != null){
			GameObject unit = tar.GetComponent<TnGAttribute>().m_unit;
			GameObject building = tar.GetComponent<TnGAttribute>().m_building;
			int currentControl = m_currUnit.GetComponent<UnitController>().m_control;
			//check if there is unit on that grid
			if(unit!=null){
				m_gridLogic.ClearAllMasks();
				tar.GetComponent<MaskManager>().RedMaskOn();
				int dmg = m_currUnit.GetComponent<UnitController>().Attack(unit);
				if(Network.isClient || Network.isServer){
					IntVector2 temp1 = m_currUnit.GetComponent<UnitController>().GetPositionOnMap();
					IntVector2 temp2 = unit.GetComponent<UnitController>().GetPositionOnMap();
					networkView.RPC("UnitAttackUnit",RPCMode.OthersBuffered,temp1.x,temp1.y,temp2.x,temp2.y, dmg);
				}
				m_isReadyToAttack = false;
			}
			if(building!=null){
				m_gridLogic.ClearAllMasks();
				tar.GetComponent<MaskManager>().RedMaskOn();
				int dmg = m_currUnit.GetComponent<UnitController>().Attack(building);
				if(Network.isClient || Network.isServer){
					IntVector2 temp1 = m_currUnit.GetComponent<UnitController>().GetPositionOnMap();
					IntVector2 temp2 = building.GetComponent<BuildingController>().GetPositionOnMap();
					networkView.RPC("UnitAttackBuilding",RPCMode.OthersBuffered,
						temp1.x,temp1.y,temp2.x,temp2.y, dmg, currentControl);
				}
				m_isReadyToAttack = false;
			}
		}
	}
	
	private void ProcessUnitMove(GameObject dest)
	{
		//make sure selected node is in range
		if (dest.GetComponent<HexGridModel>().m_prevNode != null){
			//check if the dest grid is empty
			if (dest.GetComponent<TnGAttribute>().m_unit == null
				&&dest.GetComponent<TnGAttribute>().m_building == null){
				m_gridLogic.ClearAllMasks();
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
	
	private void ProcessBuildingProduction(GameObject tar)
	{
		if (tar.GetComponent<HexGridModel>().m_prevNode != null) {
			m_gridLogic.ClearAllMasks();
			int unitIndex = m_currUnit.GetComponent<BuildingController>().ProduceUnitAt(tar);
			m_isReadyToProduce = false;
			if(Network.isClient || Network.isServer){
				IntVector2 temp1 = m_currUnit.GetComponent<BuildingController>().GetPositionOnMap();
				IntVector2 temp2 = tar.GetComponent<HexGridModel>().GetPositionOnMap();
				networkView.RPC("BuildingProduceUnitAt",RPCMode.OthersBuffered,
					temp1.x,temp1.y,temp2.x,temp2.y, unitIndex);
			}
		}
	}
	
	void Update () 
	{
		if(!m_inTurn){
			m_gridLogic.ClearAllMasks();
			m_currUnit = m_apController.OnTurnBegin();
			ProcessTurnBegin();
		}
		//actual move
		if(m_isReadyToMove){
			if(Input.GetButtonDown("LeftClick")) {
				RaycastHit grid;
				Ray selection = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(selection,out grid)){				
					GameObject dest = grid.collider.gameObject;
					ProcessUnitMove(dest);
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
					ProcessUnitAttack(tar);
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
					//make sure target is within production range and it does not hold a unit
					ProcessBuildingProduction(tar);
				}
			}
		}
		//cancel
		if(m_isReadyToAttack || m_isReadyToMove || m_isReadyToProduce){
			if(Input.GetButtonDown("RightClick")) {
				m_gridLogic.ClearAllMasks();
				m_isReadyToMove = false;
				m_isReadyToAttack = false;
				m_isReadyToProduce = false;
				if(m_currUnit.GetComponent<UnitController>()!= null)
					m_currUnit.GetComponent<UnitController>().CommandCancelled();
				else if(m_currUnit.GetComponent<BuildingController>()!= null)
					m_currUnit.GetComponent<BuildingController>().CommandCancelled();
				else {
					Debug.LogWarning("should not happen");
				}
			}
		}
	}
	
	//network supports, remote procedure calls and their envelopes
	[RPC]
	private void MoveUnitToDest(int unitX, int unitY, int destX, int destY)
	{
		GameObject unit = m_gridLogic.GetUnitAt(unitX,unitY);
		ProcessMovementRange(unit,true);
		m_isReadyToMove = false;
		unit.GetComponent<UnitController>().Move(m_gridLogic.GetGridAt(destX,destY));
	}
	
	[RPC]
	private void UnitAttackUnit(int unitX, int unitY, int tarX, int tarY, int dmg)
	{
		GameObject unit = m_gridLogic.GetUnitAt(unitX,unitY);
		GameObject tar = m_gridLogic.GetUnitAt(tarX,tarY);
		ProcessAttackRange(unit,true);
		m_isReadyToAttack = false;
		unit.GetComponent<AttackController>().DoAttack(tar);
		tar.GetComponent<UnitController>().LoseHealthBy(dmg);
	}
	
	[RPC]
	private void UnitAttackBuilding(int unitX, int unitY, int tarX, int tarY, int dmg, int control)
	{
		GameObject unit = m_gridLogic.GetUnitAt(unitX,unitY);
		GameObject tar = m_gridLogic.GetBuildingAt(tarX,tarY);
		ProcessAttackRange(unit,true);
		m_isReadyToAttack = false;
		unit.GetComponent<AttackController>().DoAttack(tar);
		tar.GetComponent<BuildingController>().LoseHealthBy(dmg, control);
	}
	
	[RPC]
	private void PlayerTurnEnded(int newUnitAP)
	{
		m_currUnit.GetComponent<APController>().RPCSetAP(newUnitAP);
		m_apController.OnTurnEnd();
		m_inTurn = false;
		m_currUnit = null;
	}
	
	[RPC]
	private void BuildingProduceUnitAt(int buildingX, int buildingY, int tarX, int tarY, int unitIndex)
	{
		GameObject tar = m_gridLogic.GetGridAt(tarX,tarY);
		GameObject building = m_gridLogic.GetBuildingAt(buildingX,buildingY);
		building.GetComponent<BuildingController>().RPCProduceUnitAt(tar,unitIndex);
		m_isReadyToProduce = false;
	}
	
	[RPC]
	private void VictoryClaimedBy(string name)
	{
		m_apController.Initialise();
		GetComponent<GUIBtmPanelAndMsgs>().GameWonBy(name);
	}
	
	void OnPlayerDisconnected()
	{
		m_apController.Initialise();
		GetComponent<GUIBtmPanelAndMsgs>().ConnectionError("Player has disconnected from hosted game");

		MasterServer.UnregisterHost();
	}
	
	void OnDisconnectedFromServer()
	{
		m_apController.Initialise();
		GetComponent<GUIBtmPanelAndMsgs>().ConnectionError("You have disconnected from server");
	}
}
