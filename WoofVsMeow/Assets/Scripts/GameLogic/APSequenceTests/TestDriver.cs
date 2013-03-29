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
			objArray[i].AddComponent("APController");
			objArray[i].GetComponent<APController>().InitialiseTestInstance(i,Random.Range (0,1));
		}
		*/
		
		//Fixed Test Case
		for (int i=0; i<10; i++)
		{
			objArray[i] = new GameObject("Object"+ i.ToString());
			objArray[i].AddComponent("APController");
		}
		objArray[0].GetComponent<APController>().InitialiseTestInstance(0, 15, 35);
		objArray[1].GetComponent<APController>().InitialiseTestInstance(1, 50, 80);
		objArray[2].GetComponent<APController>().InitialiseTestInstance(2, 50, 88);
		objArray[3].GetComponent<APController>().InitialiseTestInstance(3, 55, 77);
		objArray[4].GetComponent<APController>().InitialiseTestInstance(4, 60, 68);
		objArray[5].GetComponent<APController>().InitialiseTestInstance(5, 80, 88);
		objArray[6].GetComponent<APController>().InitialiseTestInstance(6, 95, 105);
		objArray[7].GetComponent<APController>().InitialiseTestInstance(7, 95, 111);
		objArray[8].GetComponent<APController>().InitialiseTestInstance(8, 100, 111);
		objArray[9].GetComponent<APController>().InitialiseTestInstance(9, 111, 125);
		
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
			APController temp = thisObj.GetComponent<APController>();
			temp.Print();
			temp.ReplenishAP(1);
			cpu.OnTurnEnd();	
		}
		else if (Input.GetKeyDown(KeyCode.C))
		{
			cpu.DebugCount();
		}
	}
}
