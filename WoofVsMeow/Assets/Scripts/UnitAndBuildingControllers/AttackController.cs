using UnityEngine;
using System.Collections;

public class AttackController : MonoBehaviour 
{
	public void Initialise ()
	{
	}
	
	//perform the animation related to attack
	public void DoAttack (GameObject tar)
	{
		//face the right direction and play animation
		Vector3 dir = tar.transform.position-transform.position;
		transform.rotation = Quaternion.LookRotation(new Vector3(dir.x,0,dir.z));
		animation.Play("attack");
		animation.PlayQueued("still");
	}
	
	public int AttackUnit (GameObject tar, int dmg)
	{
		DoAttack (tar);
		//calculate damage and decrease target hp
		//get target armor type and calculate
		int realDmg = dmg;
		tar.GetComponent<UnitController>().LoseHealthBy(realDmg);
		GetComponent<UnitController>().AttackFinished();
		return realDmg;
	}
	
	public int AttackBuilding (GameObject tar, int dmg)
	{
		DoAttack (tar);
		int realDmg = dmg;
		/*
		tar.GetComponent<UnitController>().LoseHealthBy(realDmg);*/
		GetComponent<UnitController>().AttackFinished();
		return realDmg;
	}
}
