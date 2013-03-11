using UnityEngine;
using System.Collections;

public class TestDriver : MonoBehaviour 
{
 	private GameObject tempUnit; 
 	GameObject[] objArray;
	APSequenceController cpu;

	// Use this for initialization
	void Start () 
	{
		cpu = GetComponent<APSequenceController>();
		objArray = new GameObject[10];

		for (int i=0; i<10; i++)
		{
			objArray[i] = new GameObject("Object"+ i.ToString());
			objArray[i].AddComponent("GameUnitModel");
			objArray[i].GetComponent<GameUnitModel>().InitialiseTestInstance(i,Random.Range (0,1));
		}

		for (int j=0; j<10; j++)
		{
			cpu.AddNewUnit(objArray[j]);
		}

	}

	// Update is called once per frame
	void Update () 
	{
		//Press P to see the contents of the Queue
		if (Input.GetKeyDown(KeyCode.P))
		{
			cpu.Print();
		}
		//Press T to elapse the Turn
		else if (Input.GetKeyDown(KeyCode.T))
		{
			GameObject thisObj = cpu.OnTurnBegin();
			GameUnitModel temp = thisObj.GetComponent<GameUnitModel>();
			temp.Print();
			temp.Set_Ap(temp.Get_Max());
			cpu.OnTurnEnd();	
		}
		else if (Input.GetKeyDown(KeyCode.C))
		{
			cpu.DebugCount();
		}
	}
}
