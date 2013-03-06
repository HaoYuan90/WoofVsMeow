using UnityEngine;
using System.Collections;

public class TestCase3 : MonoBehaviour 
{
  private GameObject tempUnit; 
	GameObject[] objArray;
	CPU cpu;

	// Use this for initialization
	void Start () 
	{
		cpu = GetComponent<CPU>();
		objArray = new GameObject[10];
		
		for (int i=0; i<10; i++)
		{
			objArray[i] = new GameObject("Object"+ (i+1).ToString());
			objArray[i].AddComponent("GameUnitModel");
			objArray[i].GetComponent<GameUnitModel>().Initialize(i+1, Random.Range(1,500), 500, Random.Range(1,2));
		}
		
		for (int j=0; j<10; j++)
		{
			cpu.Insert(objArray[j]);
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
				cpu.TurnBegin();
			}
			else if (Input.GetKeyDown(KeyCode.C))
			{
				cpu.DebugCount();
			}
	}
}
