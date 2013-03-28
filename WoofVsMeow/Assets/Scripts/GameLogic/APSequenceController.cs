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
			APnControlModel A = objA.GetComponent<APnControlModel>();
			APnControlModel B = objB.GetComponent<APnControlModel>();		
			return A.currentAP - B.currentAP;
		}
	}
	
	//list sorted according to ap
	public bool m_debug;
 	private List<GameObject> m_gameUnits;
	private APComparer m_APComparer;

	// Use this for initialization
	public void Initialise () 
	{
		m_gameUnits = new List<GameObject>();
		m_APComparer = new APComparer();
		
		if(m_debug){
			gameObject.AddComponent("FixedTestDriver");
			gameObject.GetComponent<FixedTestDriver>().RunTest();
		}
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
				int temp = unit.GetComponent<APnControlModel>().currentAP;
				index = m_gameUnits.FindLastIndex(x => x.GetComponent<APnControlModel>().currentAP <= temp)+1;
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
	
	public GameObject OnTurnBegin()
	{
		int apModAmt;
		GameObject unit;
		
		if(m_gameUnits.Count > 0){
			unit = m_gameUnits[0];
			apModAmt = unit.GetComponent<APnControlModel>().currentAP;
			
			//reduce ap of all units in the list by the amount of the 1st unit
			foreach(GameObject e in m_gameUnits)
			{
				e.GetComponent<APnControlModel>().DecreaseAP(apModAmt);
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
	
	public int GetCount()
	{
		return m_gameUnits.Count;
	}
	
	
	//debug functions 
	public void Print()
	{
		for (int i=0; i<m_gameUnits.Count; i++)
		{
			m_gameUnits[i].GetComponent<APnControlModel>().Print();
		}
	}
	
	public void DebugCount()
	{
		Debug.Log(m_gameUnits.Count);
	}
	
	public string ListToString()
	{
		string result = "";
		
		if (m_gameUnits.Count < 1)
		{
			return result;
		}
		else
		{
			result = m_gameUnits[0].GetComponent<APnControlModel>().ToString();
			for (int i=1; i<m_gameUnits.Count; i++)
			{
				result += " ";
				result += m_gameUnits[i].GetComponent<APnControlModel>().ToString();
			}
		}
		
		return result;
	}
}
