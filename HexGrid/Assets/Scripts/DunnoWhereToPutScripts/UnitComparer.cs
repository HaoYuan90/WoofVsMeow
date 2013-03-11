using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitComparer : IComparer<GameObject>
{

  // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public int Compare(GameObject objA, GameObject objB)
	{
		GameUnitModel gumA = objA.GetComponent<GameUnitModel>();
		GameUnitModel gumB = objB.GetComponent<GameUnitModel>();		

		return gumA.Get_AP() - gumB.Get_AP();
	}
}
