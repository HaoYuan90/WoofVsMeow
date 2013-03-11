using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class APSequenceController : MonoBehaviour
{
	//private class used to compare AP of gameobjects
	class APComparer : IComparer<GameObject>
	{
		public int Compare(GameObject objA, GameObject objB)
		{
			GameUnitModel gumA = objA.GetComponent<GameUnitModel>();
			GameUnitModel gumB = objB.GetComponent<GameUnitModel>();		
			return gumA.Get_AP() - gumB.Get_AP();
		}
	}
	
	//list sorted according to ap
 	private List<GameObject> m_gameUnits;
	private APComparer m_APComparer;

	// Use this for initialization
	void Start () 
	{
		m_gameUnits = new List<GameObject>();
		m_APComparer = new APComparer();
		gameObject.AddComponent("TestDriver");
	}

	private void InsertUnit(GameObject unit)
	{
		int index;
		if (m_gameUnits.Count == 0)
		{
			m_gameUnits.Add(unit);
		}
		else
		{		
			index = m_gameUnits.BinarySearch(unit,m_APComparer);	
			if (index < 0)
			{
				m_gameUnits.Insert(~index, unit);
			}
			else
			{
				//find the last occurence and append after it
				int temp = unit.GetComponent<GameUnitModel>().Get_AP();
				index = m_gameUnits.FindLastIndex(x => x.GetComponent<GameUnitModel>().Get_AP() <= temp)+1;
				m_gameUnits.Insert(index, unit);
			}
		}
	}
	
	private void RemoveUnit(GameObject unit)
	{
		int index = m_gameUnits.IndexOf(unit);
		if (index != -1){
			m_gameUnits.RemoveAt(index);
		}
		else{
			//should not happen
			Debug.LogWarning("RemoveUnit() is called trying to remove a unit that does not exist in AP list");
		}
	}
	
	//put the unit into proper place in the ap list
	public void ArrangeUnit(GameObject unit)
	{
		RemoveUnit(unit);
		InsertUnit(unit);
	}
	
	public void AddNewUnit(GameObject unit)
	{
		InsertUnit(unit);
	}
	
	/* obseleted
	public void Remove(int index)
	{
		m_gameUnits.RemoveAt(index);
	}*/
	
	public GameObject OnTurnBegin()
	{
		int apModAmt;
		GameObject unit;
		
		if(m_gameUnits.Count > 0){
			unit = m_gameUnits[0];
			apModAmt = -unit.GetComponent<GameUnitModel>().Get_AP();
			
			//reduce ap of all units in the list by the amount of the 1st unit
			foreach(GameObject e in m_gameUnits)
			{
				e.GetComponent<GameUnitModel>().Modify_Ap(apModAmt);
			}
			return unit;
		}
		else 
			return null;
	}
	
	/*
	 * Implement!!!
	 * Unit deactivation and stuff
	 */
	
	public void OnTurnEnd()
	{
		if(m_gameUnits.Count > 0)
			ArrangeUnit(m_gameUnits[0]);
	}

	public void Print()
	{
		for (int i=0; i<m_gameUnits.Count; i++)
		{
			m_gameUnits[i].GetComponent<GameUnitModel>().Print();
		}
	}

	public void DebugCount()
	{
		Debug.Log(m_gameUnits.Count);
	}
}
