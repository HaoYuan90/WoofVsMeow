using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CPU : MonoBehaviour
{
  private List<GameObject> list;
	private int control;
	private UnitComparer uc;

	// Use this for initialization
	void Start () 
	{
		control = 0;
		list = new List<GameObject>();
		uc = new UnitComparer();
		gameObject.AddComponent("TestCase2");
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	public void Insert(GameObject obj)
	{
		int index;
		if (list.Count == 0)
		{
			list.Add(obj);
		}
		else
		{		
			index = list.BinarySearch(obj,uc);	
			if (index < 0)
			{
				list.Insert(~index, obj);
			}
			else
			{
				list.Insert(index+1, obj);
			}
		}
	}
	
	public void Remove(GameObject obj)
	{
		int index = list.IndexOf(obj);
		if (index != -1)
		{
			list.RemoveAt(index);
		}
	}
	public void Remove(int index)
	{

		list.RemoveAt(index);

	}
	
	public void TurnBegin()
	{
		int apMinus;
		GameUnitModel gum;
		GameObject obj;
		
		obj = list[0];
		apMinus = obj.GetComponent<GameUnitModel>().Get_AP();
		Remove(0);
		gum = obj.GetComponent<GameUnitModel>();
		
		for (int i=0; i<list.Count; i++)
		{
			list[i].GetComponent<GameUnitModel>().Modify_Ap(-apMinus);
		}
		
		gum.Activate();
	  control = gum.Get_Control();
	}
	public void TurnEnd(GameObject obj)
	{
		GameUnitModel gum = obj.GetComponent<GameUnitModel>();
		gum.Set_Ap(Random.Range(1, gum.Get_Max()));
		Insert(obj);
		control = 0;
	}
	
	public void Print()
	{
		for (int i=0; i<list.Count; i++)
		{
			list[i].GetComponent<GameUnitModel>().Print();
		}
	}
	
	public void DebugCount()
	{
		Debug.Log(list.Count);
	}
}
