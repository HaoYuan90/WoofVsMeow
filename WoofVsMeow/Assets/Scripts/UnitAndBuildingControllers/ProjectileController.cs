using UnityEngine;
using System.Collections;

public class ProjectileController : MonoBehaviour 
{
	public GameObject m_target;
	private Vector3 m_tar;
	private int m_steps = 40;
	readonly private float m_yOffset = 3.0f;
	
	void Start ()
	{
		m_tar = new Vector3(m_target.transform.position.x,m_target.transform.position.y+m_yOffset,m_target.transform.position.z);
		transform.LookAt(m_tar);
	}
	
	void Update () 
	{
		transform.position = Vector3.Lerp(transform.position, m_tar, 1f/m_steps);
		m_steps --;
		if(m_steps == 0)
			GameObject.Destroy(gameObject);
	}
}
