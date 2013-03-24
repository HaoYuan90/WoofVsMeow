using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MovementController : MonoBehaviour 
{
	public GameObject m_currentGrid;
	
	public int m_movementRange;
	//storing last grid walked past and the next grids that needs to walk onto
	private List<GameObject> m_pathList; 

	private int m_movementSpeed;       //frames required to walk from a grid to another
	private int m_movementStepLeft;         //frames left to reach the next node

	// Use this for initialization
	public void Initialise (GameObject currentGrid)
	{
		m_pathList = new List<GameObject>();
		m_currentGrid = currentGrid;
		m_movementRange = 5; //get these values from unit model to be implemented
		m_movementSpeed = 30;
		m_movementStepLeft = 0;
	}
	
	public void Move (GameObject dest)
	{
		//get the movement path
		List<GameObject> pathList = new List<GameObject>();
		GameObject currentNode = dest;		
		while (currentNode != null) {
			pathList.Insert(0,currentNode);
			currentNode=currentNode.GetComponent<HexGridModel>().m_prevNode;
		}
		//pathList should at least have a src and destination
		if(pathList.Count>=2){
			m_pathList = pathList;
			m_pathList[0].GetComponent<TnGAttribute>().m_unit = null;
			//give control to destination node
			dest.GetComponent<TnGAttribute>().m_unit = gameObject;
			m_currentGrid = dest;
		}
	}
	
	void FixedUpdate () {
		if(m_pathList.Count > 0)
		{
			if(!gameObject.animation.IsPlaying("Take 001"))
				gameObject.animation.Play("Take 001");
			//have reached the next node which should be index 0
			if(m_movementStepLeft == 0){
				//set this line 
				//gameObject.transform.position = m_pathList[0].transform.position;
				//have not yet reached destination
				if(m_pathList.Count > 1) {
					//face the next node
					Vector3 dir = m_pathList[1].transform.position-gameObject.transform.position;
					gameObject.transform.rotation = Quaternion.LookRotation(new Vector3(dir.x,0,dir.z));
				}
				else{
					m_pathList = new List<GameObject>();
					//inform unit controller
					gameObject.GetComponent<UnitController>().MoveFinished();
					return;
				}
				m_pathList.RemoveAt(0);
				m_movementStepLeft = m_movementSpeed;
			}
			Vector3 src = gameObject.transform.position;
			Vector3 temp = m_pathList[0].transform.position;
			Vector3 dest = new Vector3(temp.x,m_pathList[0].renderer.bounds.max.y,temp.z);
			gameObject.transform.position = Vector3.Lerp(src,dest,(float)1/m_movementStepLeft);
			m_movementStepLeft --;
		}
		else
		{
			if(gameObject.animation.IsPlaying("Take 001"))
				gameObject.animation.Stop();
		}
	}
}

