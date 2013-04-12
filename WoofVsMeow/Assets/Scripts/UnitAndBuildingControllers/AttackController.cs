using UnityEngine;
using System.Collections;

public class AttackController : MonoBehaviour 
{
	public void Initialise ()
	{
		//do nothing it seems....
	}
	
	//perform the animation related to attack
	public void DoAttack (GameObject tar)
	{
		//face the right direction and play animation
		Vector3 dir = tar.transform.position-transform.position;
		transform.rotation = Quaternion.LookRotation(new Vector3(dir.x,0,dir.z));
		animation.Play("attack");
		animation.PlayQueued("still");
		GameObject.Find("SoundEffectController").GetComponent<SoundEffectController>().PlayAttackSoundEffect(gameObject);
	}
	
	public int AttackUnit (GameObject tar)
	{
		DoAttack (tar);
		int dmg = GetComponent<UnitController>().m_damage;
		int realDmg = (int)(dmg*ComputeDmgModifier(tar));
		tar.GetComponent<UnitController>().LoseHealthBy(realDmg);
		GetComponent<UnitController>().AttackFinished();
		return realDmg;
	}
	
	public int AttackBuilding (GameObject tar)
	{
		DoAttack (tar);
		int dmg;
		//neutral building, it only takes 2 hits to occupy
		if(tar.GetComponent<BuildingController>().m_control == -1){
			dmg = 50;
		}
		else{
			if(GetComponent<UnitController>().m_attackType == AttackType.normal)
				dmg = 20;
			else
				dmg = 10;
		}
		int attackerControl = GetComponent<UnitController>().m_control;
		tar.GetComponent<BuildingController>().LoseHealthBy(dmg,GetComponent<UnitController>().m_control);
		GetComponent<UnitController>().AttackFinished();
		return dmg;
	}
	
	private double ComputeDmgModifier (GameObject tar)
	{
		//attack and armor type
		AttackType at = GetComponent<UnitController>().m_attackType;
		ArmorType dt = tar.GetComponent<UnitController>().m_armorType;
		double mod = 1;
		switch(at){
		case AttackType.normal:
			if(dt == ArmorType.light)
				mod = 2;
			else if(dt == ArmorType.flyer)
				mod = 0.5;
			break;
		case AttackType.pierce:
			if(dt == ArmorType.flyer)
				mod = 2;
			break;
		case AttackType.strafe:
			if(dt == ArmorType.heavy)
				mod = 2;
			break;
		}
		//terrain type
		GameObject tarGrid = tar.GetComponent<UnitController>().currentGrid;
		TerrainType tt = tarGrid.GetComponent<TnGAttribute>().m_terrainType;
		if(tt == TerrainType.forest)
			mod *= 0.8;
		//height
		if(at != AttackType.strafe){
			GameObject selfGrid = GetComponent<UnitController>().currentGrid;
			int tarHeight = tarGrid.GetComponent<TnGAttribute>().m_height;
			int selfHeight = selfGrid.GetComponent<TnGAttribute>().m_height;
			if(selfHeight - tarHeight > 2)
				mod *= 1.2;
			else if(tarHeight - selfHeight > 2)
				mod *= 0.8;
		}
		return mod;
	}
}
