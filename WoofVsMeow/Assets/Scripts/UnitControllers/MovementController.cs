using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MovementController : MonoBehaviour 
{
	private GridLogic m_gridLogic; 
	//private GameObject m_currentGrid;
	
	public int m_movementRange;
	//storing last grid walked past and the next grids that needs to walk onto
	private List<GameObject> m_pathList; 
	//make sure unit is not interrupted while doing something
	//GUI layer will again make sure of this
	bool m_isInAction;

	private int m_movementSpeed;       //frames required to walk from a grid to another
	private int m_movementStepLeft;         //frames left to reach the next node

	// Use this for initialization
	public void Initialise (GridLogic gridLogic/*, GameObject grid*/)
	{
		m_pathList = new List<GameObject>();
		m_isInAction = false;
		m_gridLogic = gridLogic;
			//m_currentGrid = grid;
		m_movementRange = 5; //get these values from unit model to be implemented
		m_movementSpeed = 30;
		m_movementStepLeft = 0;
	}
	
	public void Move (List<GameObject> pathList){
		//gameobject should not be moving alreay
		//pathList should at least have a src and destination
		if(!m_isInAction && pathList.Count>=2){
			m_pathList = pathList;
			m_pathList[0].GetComponent<TnGAttribute>().m_unit = null;
		}
	}
	
	void FixedUpdate () {
		if(m_pathList.Count > 0)
		{
			gameObject.animation.Play("Take 001");
			//have reached the next node which should be index 0
			if(m_movementStepLeft == 0){
				gameObject.transform.position = m_pathList[0].transform.position;
				//have not yet reached destination
				if(m_pathList.Count > 1) {
					//face the next node
					gameObject.transform.rotation = 
						Quaternion.LookRotation(m_pathList[1].transform.position-gameObject.transform.position);
				}
				else{
					//if destination is reached, destination node should claim ownership
					m_pathList[0].GetComponent<TnGAttribute>().m_unit = gameObject;
					m_pathList = new List<GameObject>();
					return;
				}
				m_pathList.RemoveAt(0);
				m_movementStepLeft = m_movementSpeed;
			}
			
			Vector3 src = gameObject.transform.position;
			Vector3 dest = m_pathList[0].transform.position;
			gameObject.transform.position = Vector3.Lerp(src,dest,(float)1/m_movementStepLeft);
			m_movementStepLeft --;
		}
			/*
		if (pathList.Count <= 1) { //there is no grid to walk onto
			if (Input.GetButtonDown("Fire1")) {
				RaycastHit grid;
				Ray selection = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(selection,out grid)){				
					if (srcGrid == null) {
						srcGrid = grid.collider.gameObject;
						gridLogic.GetComponent<GridLogic>().HighlightMovementRange(srcGrid, 3);
						if (currentUnit!=null)
							Destroy(currentUnit);
						currentUnit = (GameObject)Instantiate(m_meow);               //spawn unit
			        	currentUnit.transform.parent = unitGroup.transform;          //assign parent
			        	currentUnit.transform.localScale = new Vector3(1, 1, 1);  
			        	currentUnit.transform.position = srcGrid.transform.position; //move unit onto grid
						currentUnit.animation.AddClip(GameObject.Find("UnitController").animation.GetClip("Take 001"),"Take 001");
					}                                                                //attach the animation require onto the unit
					else { //set the state of the unit to movement
						var destGrid = grid.collider.gameObject;
						pathList = gridLogic.GetComponent<GridLogic>().HighlightMovementPath(srcGrid, destGrid);
	                    movementStepLeft = movementSpeed;
						currentUnit.animation.Play("Take 001");
						srcGrid = null;
					}
				}
			}
		}*/
		/*
		else {
			//let the unit face the direction to walk
			currentUnit.transform.rotation = Quaternion.LookRotation(pathList[1].transform.position-currentUnit.transform.position);
			//move the unit by one step
			currentUnit.transform.position = Vector3.Lerp(currentUnit.transform.position,pathList[1].transform.position,(float)1/movementStepLeft);
			if (currentUnit.transform.position == pathList[1].transform.position) {
	            movementStepLeft = movementSpeed;
				pathList.RemoveAt(0);
			}
			else
				movementStepLeft--;
		}*/
	}
}

