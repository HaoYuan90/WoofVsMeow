using UnityEngine;
using System.Collections;

public class FlyerUnitController : UnitController {

	public override void MoveButtonAction()
	{
		GetComponent<UnitGUIController>().DisableGUI();
		m_engine.ProcessFlyingRange(gameObject,false);
	}
}
