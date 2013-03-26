using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MovementController : MonoBehaviour 
{
	public GameObject m_currentGrid;
	
	public int m_movementRange;
	//A list of grids that a unit needs to walk onto
	private List<GameObject> m_pathList;
	private GameObject lastGrid;

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
			if(!animation.IsPlaying("run"))
				animation.Play("run");
			//have reached the next node which should be index 0
			if(m_movementStepLeft == 0){
				//set this line 
				//transform.position = m_pathList[0].transform.position;
				//have not yet reached destination
				if(m_pathList.Count > 1) {
					//face the next node
					Vector3 dir = m_pathList[1].transform.position-transform.position;
					transform.rotation = Quaternion.LookRotation(new Vector3(dir.x,0,dir.z));
				}
				else{
					m_pathList = new List<GameObject>();
					//inform unit controller
					GetComponent<UnitController>().MoveFinished();
					//setup animation
					animation.Play ("still");
					return;
				}
				lastGrid = m_pathList[0];
				m_pathList.RemoveAt(0);
				m_movementStepLeft = m_movementSpeed;
			}
			
			Vector3 src = transform.position;
			Vector3 dest = m_pathList[0].transform.position;
			dest.y = m_pathList[0].renderer.bounds.max.y;
			Vector3 nextPosition = Vector3.Lerp(src,dest,(float)1/m_movementStepLeft);
			if(Mathf.Abs(lastGrid.renderer.bounds.max.y-m_pathList[0].renderer.bounds.max.y) > 0.01){ //jumping
				float maxJumpHeight = (float)3 + Math.Max(lastGrid.renderer.bounds.max.y, m_pathList[0].renderer.bounds.max.y);
				if (m_movementStepLeft > m_movementSpeed/2) { //jumping up
					float deltaY = maxJumpHeight - lastGrid.renderer.bounds.max.y;
					float coeff = (float)(m_movementStepLeft - m_movementSpeed/2)/(m_movementSpeed/2);
					deltaY = deltaY * (1-coeff*coeff);
					nextPosition.y = lastGrid.renderer.bounds.max.y + deltaY;
					//Debug.Log(m_movementStepLeft.ToString()+", "+(1-coeff*coeff).ToString());
				}
				else { //jumping down
					float deltaY = maxJumpHeight - m_pathList[0].renderer.bounds.max.y;
					float coeff = (float)m_movementStepLeft/(m_movementSpeed/2);
					deltaY = deltaY * (coeff*coeff);
					//Debug.Log(m_movementStepLeft.ToString()+", "+(coeff*coeff).ToString());
					nextPosition.y = m_pathList[0].renderer.bounds.max.y + deltaY;
				}
			}
			transform.position = nextPosition;
			
			m_movementStepLeft --;
		}
	}
}

