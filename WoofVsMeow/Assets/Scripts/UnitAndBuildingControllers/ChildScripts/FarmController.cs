using UnityEngine;
using System.Collections;

public class FarmController : BuildingController {
	
	public override void OnBuildingDestroyed(int attackerControl, float delay)
	{
		m_health = m_maxHealth;
		int prevControl = m_control;
		m_control = attackerControl;
    	StartCoroutine(DelayedHPBarReset(delay));
		GetComponent<APController>().ReplenishAP(1);
		
		GameObject flag = transform.FindChild("flag1").gameObject;
		GameObject roof = transform.FindChild("building:DrawCall_3").gameObject;
		if (attackerControl == 0){
			flag.renderer.material.color = Color.red;
			roof.renderer.material.color = Color.red;
		}
		else if(attackerControl == 1){
			flag.renderer.material.color = Color.blue;
			roof.renderer.material.color = Color.blue;
		}
		
		m_engine.OnNeutralBuildingCaptured(gameObject, prevControl);
	}
}
