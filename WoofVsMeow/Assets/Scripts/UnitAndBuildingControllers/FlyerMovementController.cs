using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class FlyerMovementController : MovementController {
	
	//remove animation
	public override void FixedUpdate () {
		if(m_pathList.Count > 0)
		{
			//have reached the next node which should be index 0
			if(m_movementStepLeft == 0){
				m_pathList[0].GetComponent<MaskManager>().OutlineMaskOn();
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
					return;
				}
				lastGrid = m_pathList[0];
				m_pathList.RemoveAt(0);
				m_movementStepLeft = m_movementSpeed;
			}
			
			//directly go up/down as opposed to jumping
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
