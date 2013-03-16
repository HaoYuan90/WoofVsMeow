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
		
		//Random Test Case
		/*
		for (int i=0; i<10; i++)
		{
			objArray[i] = new GameObject("Object"+ i.ToString());
			objArray[i].AddComponent("GameUnitModel");
			objArray[i].GetComponent<GameUnitModel>().InitialiseTestInstance(i,Random.Range (0,1));
		}
		*/
		
		//Fixed Test Case
		for (int i=0; i<10; i++)
		{
			objArray[i] = new GameObject("Object"+ i.ToString());
			objArray[i].AddComponent("GameUnitModel");
		}
		objArray[0].GetComponent<GameUnitModel>().InitialiseTestInstance(0, 15, 35, 1);
		objArray[1].GetComponent<GameUnitModel>().InitialiseTestInstance(1, 50, 80, 2);
		objArray[2].GetComponent<GameUnitModel>().InitialiseTestInstance(2, 50, 88, 1);
		objArray[3].GetComponent<GameUnitModel>().InitialiseTestInstance(3, 55, 77, 2);
		objArray[4].GetComponent<GameUnitModel>().InitialiseTestInstance(4, 60, 68, 1);
		objArray[5].GetComponent<GameUnitModel>().InitialiseTestInstance(5, 80, 88, 1);
		objArray[6].GetComponent<GameUnitModel>().InitialiseTestInstance(6, 95, 105, 2);
		objArray[7].GetComponent<GameUnitModel>().InitialiseTestInstance(7, 95, 111, 2);
		objArray[8].GetComponent<GameUnitModel>().InitialiseTestInstance(8, 100, 111, 1);
		objArray[9].GetComponent<GameUnitModel>().InitialiseTestInstance(9, 111, 125, 2);
		
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
			temp.SetAP(temp.GetMax());
			cpu.OnTurnEnd();	
		}
		else if (Input.GetKeyDown(KeyCode.C))
		{
			cpu.DebugCount();
		}
	}
}
