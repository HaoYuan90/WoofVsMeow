using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitController : MonoBehaviour {
    public GameObject m_meow; //for importing the meow model
	GameObject unitGroup;     //units spawned will be put into this group
	GameObject currentUnit;
	private GameObject gridLogic; //for easier script access
	List<GameObject> pathList;    //storing last grid walked past and the next grids that needs to walk onto
	private GameObject srcGrid;
	int movementSpeed = 30;       //frames required to walk from a grid to another
	int movementStepLeft;         //frames left to reach the destination grid

	// Use this for initialization
	void Start () {
	    gridLogic = GameObject.Find("GridLogic");
	    pathList = new List<GameObject>();
		if (GameObject.Find("Units") == null) { 
			unitGroup = new GameObject("Units");
		}
		else {
			unitGroup = GameObject.Find("Units");
		}
	}
	
	// Update is called once per frame
	void Update () {
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
		}
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
		}
	}
}

