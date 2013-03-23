using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public int m_scrollSpeed = 35;
	public int m_zoomSpeed = 50;
	public int m_rotationSpeed = 3;
	public float m_edgeWidth = 2;
	private bool m_isDragging = false;
	private bool m_disabled = false;
	private int m_disableCounter = 0;
	
	// Update is called once per frame
	void Update () 
	{
		if(m_disableCounter > 0){
			m_disableCounter --;
			return;
		}
		if(m_disableCounter == 0){
			m_disabled = false;
		}
		float mousePosX = Input.mousePosition.x; 
	    float mousePosY = Input.mousePosition.y; 
	    
		if (Input.GetMouseButtonDown(1)){
			m_isDragging = true;
		}
		if (Input.GetMouseButtonUp(1)){
			m_isDragging = false;
			m_disabled = true;
			m_disableCounter = 20;
		}
	 	
		if(!m_isDragging && !m_disabled){
			//translate left
		    if (mousePosX < m_edgeWidth){
				transform.Translate(-Vector3.right * m_scrollSpeed * Time.deltaTime); 
				return;
			}
			//translate right
		    if (mousePosX >= Screen.width - m_edgeWidth){
		    	transform.Translate(Vector3.right * m_scrollSpeed * Time.deltaTime); 
				return;
			}
			//translate forward
			Vector3 forward = transform.TransformDirection(Vector3.forward);
			forward = new Vector3(forward.x,0f,forward.z);
			forward.Normalize();
		    if (mousePosY < m_edgeWidth){
				transform.position = transform.position - forward * m_scrollSpeed * Time.deltaTime;
				return;
			}
			//translate backward
		    if (mousePosY >= Screen.height - m_edgeWidth){
		    	transform.position = transform.position + forward * m_scrollSpeed * Time.deltaTime;
				return;
			}
			if (Input.GetAxis("Mouse ScrollWheel")>0){
				transform.Translate(Vector3.forward * m_zoomSpeed * Time.deltaTime);
				return;
			}
			if (Input.GetAxis("Mouse ScrollWheel")<0){
				transform.Translate(-Vector3.forward * m_zoomSpeed * Time.deltaTime);
				return;
			}
		}
		else{
			if (Input.GetAxis("Mouse X")>0){
				transform.RotateAroundLocal(Vector3.up, m_rotationSpeed * Time.deltaTime);
				return;
			}
			if (Input.GetAxis("Mouse X")<0){
				transform.RotateAroundLocal(Vector3.up, -m_rotationSpeed * Time.deltaTime);
				return;
			}
		}
    }
}
