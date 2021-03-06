using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class MaskManager : MonoBehaviour {
	
	[SerializeField]
	private GameObject m_mask;
	[SerializeField]
	private Material m_redMat;
	[SerializeField]
	private Material m_darkRedMat;
	[SerializeField]
	private Material m_blueMat;
	[SerializeField]
	private Material m_greenMat;
	[SerializeField]
	private Material m_outlineMat;
	
	public float m_maskYOffset = 0.05f;
	
	public void InitMasks (GameObject mask, Material r, Material dr, Material g, Material b, Material ol)
	{
		m_mask = mask;
		//put mask under grid
		m_mask.transform.parent = transform;
		//init material
		m_redMat = r;
		m_darkRedMat = dr;
		m_blueMat = b;
		m_greenMat = g;
		m_outlineMat = ol;
		
		//position of mask
		float maskHeight = renderer.bounds.size.y+m_maskYOffset;
		Vector3 maskPos = new Vector3(transform.position.x, transform.position.y + maskHeight, transform.position.z);
		//apply masks
		m_mask.transform.position = maskPos;
		//show grid by default
		OutlineMaskOn();
	}
	
	public void DeactivateMask ()
	{
		m_mask.SetActive(false);
	}
	public void ActivateMask ()
	{
		m_mask.SetActive(true);
	}
	
	public void RedMaskOn()
	{
		m_mask.renderer.material = m_redMat;
	}
	
	public void DarkRedMaskOn()
	{
		m_mask.renderer.material = m_darkRedMat;
	}
	
	public void BlueMaskOn()
	{
		m_mask.renderer.material = m_blueMat;
	}
	
	public void GreenMaskOn()
	{
		m_mask.renderer.material = m_greenMat;
	}
	
	public void OutlineMaskOn()
	{
		m_mask.renderer.material = m_outlineMat;
	}
}
