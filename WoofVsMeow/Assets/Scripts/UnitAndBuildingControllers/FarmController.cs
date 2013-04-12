using UnityEngine;
using System.Collections;

public class FarmController : BuildingController {

	public override void LoseHealthBy(int amount, int attackerControl)
	{
		m_health -= amount;
		GetComponent<BuildingGUIController>().OnHealthLostBy(amount);
		//building is occupied
		if(m_health <= 0){
			m_health = m_maxHealth;
			int prevControl = m_control;
			m_control = attackerControl;
        	GetComponent<BuildingGUIController>().ResetHealth();
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
			
			if(prevControl == -1)
				m_engine.OnNeutralBuildingCaptured(gameObject);
		}
	} 
}
