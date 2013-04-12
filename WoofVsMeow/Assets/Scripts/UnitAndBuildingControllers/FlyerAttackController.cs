using UnityEngine;
using System.Collections;

public class FlyerAttackController : AttackController {

	//Overide this, instead of animating, shoot a projectile
	public override void DoAttack (GameObject tar)
	{
		//face the right direction and play animation
		Vector3 dir = tar.transform.position-transform.position;
		transform.rotation = Quaternion.LookRotation(new Vector3(dir.x,0,dir.z));
		//shoot projectile
	}
}
