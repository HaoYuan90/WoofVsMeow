using UnityEngine;
using System.Collections;

//this class assumes the map has at least one object in the aplist
public class CameraController : MonoBehaviour {

	public int m_scrollSpeed;
	public int m_zoomSpeed;
	public int m_rotationSpeed;
	
	private float m_zoom = 1f;
	private float m_distToUnit = 20f;
	private GameObject m_current;
	
	//look at method to be invoked by other classes
	public void LookAt(GameObject unit)
	{
		float tempX = unit.transform.position.x;
		float tempY = unit.transform.position.y + m_distToUnit*m_zoom;
		float tempZ = unit.transform.position.z + m_distToUnit*m_zoom;
		transform.position = new Vector3(tempX,tempY,tempZ);
		transform.LookAt(unit.transform);
		m_current = unit;
	}
	//look at method to be used by self, preserves transformation
	private void LookAt(float oldZoom)
	{
		float tempX = transform.position.x;
		float tempY = transform.position.y - m_distToUnit*oldZoom;
		float tempZ = transform.position.z - m_distToUnit*oldZoom;
		Vector3 virtualTar = new Vector3(tempX,tempY,tempZ);
		tempX = virtualTar.x;
		tempY = virtualTar.y + m_distToUnit*m_zoom;
		tempZ = virtualTar.z + m_distToUnit*m_zoom;
		transform.position = new Vector3(tempX,tempY,tempZ);
		transform.LookAt(virtualTar);
	}
	
	private void ResetCamera()
	{
		m_zoom = 1f;
		LookAt(m_current);
	}
	
	void Update () 
	{
		if (Input.GetButton("RotateRight")){
			transform.Translate(Vector3.right*m_rotationSpeed*Time.deltaTime);
			transform.LookAt(m_current.transform);
			//transform.RotateAroundLocal(Vector3.up, m_rotationSpeed * Time.deltaTime);
			return;
		}
		if (Input.GetButton("RotateLeft")){
			transform.Translate(Vector3.left*m_rotationSpeed*Time.deltaTime);
			transform.LookAt(m_current.transform);
			//transform.RotateAroundLocal(Vector3.up, -m_rotationSpeed * Time.deltaTime);
			return;
		}
		//zoom out
		if (Input.GetAxis("Mouse ScrollWheel")>0){
			float oldZoom = m_zoom;
			m_zoom -= m_zoomSpeed*Time.deltaTime;
			LookAt(oldZoom);
			return;
		}
		//zoom in
		if (Input.GetAxis("Mouse ScrollWheel")<0){
			float oldZoom = m_zoom;
			m_zoom += m_zoomSpeed*Time.deltaTime;
			LookAt(oldZoom);
			return;
		}
	    if (Input.GetButton("ScrollLeft")){
			transform.Translate(-Vector3.right * m_scrollSpeed * Time.deltaTime); 
			return;
		}
	    if (Input.GetButton("ScrollRight")){
	    	transform.Translate(Vector3.right * m_scrollSpeed * Time.deltaTime); 
			return;
		}
		Vector3 forward = transform.TransformDirection(Vector3.forward);
		forward = new Vector3(forward.x,0f,forward.z);
		forward.Normalize();
	    if (Input.GetButton("ScrollDown")){
			transform.position = transform.position - forward * m_scrollSpeed * Time.deltaTime;
			return;
		}
	    if (Input.GetButton("ScrollUp")){
	    	transform.position = transform.position + forward * m_scrollSpeed * Time.deltaTime;
			return;
		}
		if (Input.GetButton("ResetCamera")){
			ResetCamera();
			return;
		}
    }
}
