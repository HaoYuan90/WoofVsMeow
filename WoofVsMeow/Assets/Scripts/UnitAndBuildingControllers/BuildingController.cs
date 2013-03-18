using UnityEngine;
using System.Collections;

public class BuildingController : MonoBehaviour {

	public void InitialiseBuilding()
	{
		gameObject.GetComponent<APnControlModel>().Initialise();
	}
}
