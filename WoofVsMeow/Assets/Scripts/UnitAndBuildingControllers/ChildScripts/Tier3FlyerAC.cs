using UnityEngine;
using System.Collections;

public class Tier3FlyerAC : AttackController {
	
	public GameObject m_projectile;
	//use this to control where the projectile is shot
	public float m_yOffset;
	public float m_xOffset;
	
	//Overide this, instead of animating, shoot a projectile
	public override void DoAttack (GameObject tar)
	{
		//face the right direction and play animation
		Vector3 dir = tar.transform.position-transform.position;
		transform.rotation = Quaternion.LookRotation(new Vector3(dir.x,0,dir.z));
		//shoot projectile
		GameObject proj = (GameObject)GameObject.Instantiate(m_projectile);
		Vector3 offset = new Vector3(dir.x,0,dir.z);
		offset = offset.normalized*m_xOffset;
		proj.transform.position = new Vector3(transform.position.x, transform.position.y+m_yOffset, transform.position.z)+offset;
		proj.GetComponent<ProjectileController>().m_target = tar;
	}
}
