using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class MaskManager : MonoBehaviour {
	
	[SerializeField]
	private GameObject m_redMask;
	[SerializeField]
	private GameObject m_blueMask;
	[SerializeField]
	private GameObject m_greenMask;
	[SerializeField]
	private GameObject m_outlineMask;
	
	public float m_maskYOffset = 0.05f;
	
	public void InitMasks (GameObject r, GameObject g, GameObject b, GameObject ol)
	{
		m_redMask = r;
		m_blueMask = b;
		m_greenMask = g;
		m_outlineMask = ol;
		//group masks under grid
		m_blueMask.transform.parent = transform;
		m_redMask.transform.parent = transform;
		m_greenMask.transform.parent = transform;
		m_outlineMask.transform.parent = transform;
		
		//position of mask
		float maskHeight = renderer.bounds.size.y+m_maskYOffset;
		Vector3 maskPos = new Vector3(transform.position.x, transform.position.y + maskHeight, transform.position.z);
		//apply masks
		m_blueMask.transform.position = maskPos;
		m_redMask.transform.position = maskPos;
		m_greenMask.transform.position = maskPos;
		m_outlineMask.transform.position = maskPos;
		//show grid by default
		OutlineMaskOn();
	}
	
	public void TurnOffAllMasks ()
	{
		m_redMask.SetActive(false);
		m_blueMask.SetActive(false);
		m_greenMask.SetActive(false);
		m_outlineMask.SetActive(false);
	}
	
	public void RedMaskOn()
	{
		TurnOffAllMasks();
		m_redMask.SetActive(true);
	}
	
	public void BlueMaskOn()
	{
		TurnOffAllMasks();
		m_blueMask.SetActive(true);
	}
	
	public void GreenMaskOn()
	{
		TurnOffAllMasks();
		m_greenMask.SetActive(true);
	}
	
	public void OutlineMaskOn()
	{
		TurnOffAllMasks();
		m_outlineMask.SetActive(true);
	}
}
