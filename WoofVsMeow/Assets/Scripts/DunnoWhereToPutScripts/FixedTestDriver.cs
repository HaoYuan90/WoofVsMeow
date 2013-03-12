using UnityEngine;
using System.Collections;

public class FixedTestDriver : MonoBehaviour 
{
   private GameObject tempUnit; 
 	GameObject[] objArray;
	APSequenceController cpu;
	private string[] expectedResults;
	private int[] unitsToMove = {0,1,2,0,3,4,5,0,6,7};
	private bool running = true;

	// Use this for initialization
	void Start () 
	{
		cpu = GetComponent<APSequenceController>();
		objArray = new GameObject[10];
		InitializeString();
		
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
		
		if (cpu.GetCount() == 10)
		{
			Debug.Log("Objects inserted successfully\n");
			Run();
		}
		else
		{
			Debug.Log ("Insertion of objects unsuccessful\n");
		}

	}
	
	void Run()
	{
		GameObject objToMove; 
		GameUnitModel gum;
		
		//Turn Elapse 1
		
		for (int i=0; i<10; i++)
		{	
			objToMove = cpu.OnTurnBegin();
			gum = objToMove.GetComponent<GameUnitModel>();
			if(ValidateUnit(i,gum))
			{
				Debug.Log("Turn " + (i+1).ToString() + " : Correct object has been given control\n");
			}
			else
			{
				Debug.LogError("Object " + gum.GetID().ToString() + " has been given control. " +
					"Expected object " + objArray[unitsToMove[i]].GetComponent<GameUnitModel>().GetID().ToString() + "\n");
				
				break;
			}
			
			gum.SetAP(gum.GetMax());
			cpu.OnTurnEnd();	
			
			if (ValidateList(i))
			{
				Debug.Log ("Turn " + (i+1).ToString() + " : List correctly ordered\n");
			}
			else
			{
				Debug.LogError("Turn " + (i+1).ToString() + " : List is wrong\n");
				string actualList = cpu.ListToString().Replace(" ", "\n");
				string expectedList = expectedResults[i].ToString().Replace(" ", "\n");
				
				Debug.Log("Expected List\n" + expectedList);
				Debug.Log("\nActual List\n" + actualList);
				break;
			}
				
		}
		running = false;
	}
	
	bool ValidateUnit(int turn, GameUnitModel gum)
	{
		if (gum.ToString().Equals(objArray[unitsToMove[turn]].GetComponent<GameUnitModel>().ToString()))
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	
	bool ValidateList(int turn)
	{
		if (cpu.ListToString().Equals(expectedResults[turn]))
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	
	void InitializeString()
	{
		expectedResults = new string[10];
		
		expectedResults[0] = "1-35-80 2-35-88 0-35-35 3-40-77 4-45-68 5-65-88 6-80-105 7-80-111 8-85-111 9-96-125";
		expectedResults[1] = "2-0-88 0-0-35 3-5-77 4-10-68 5-30-88 6-45-105 7-45-111 8-50-111 9-61-125 1-80-80";
		expectedResults[2] = "0-0-35 3-5-77 4-10-68 5-30-88 6-45-105 7-45-111 8-50-111 9-61-125 1-80-80 2-88-88";
		expectedResults[3] = "3-5-77 4-10-68 5-30-88 0-35-35 6-45-105 7-45-111 8-50-111 9-61-125 1-80-80 2-88-88";
		expectedResults[4] = "4-5-68 5-25-88 0-30-35 6-40-105 7-40-111 8-45-111 9-56-125 1-75-80 3-77-77 2-83-88";
		expectedResults[5] = "5-20-88 0-25-35 6-35-105 7-35-111 8-40-111 9-51-125 4-68-68 1-70-80 3-72-77 2-78-88";
		expectedResults[6] = "0-5-35 6-15-105 7-15-111 8-20-111 9-31-125 4-48-68 1-50-80 3-52-77 2-58-88 5-88-88";			
		expectedResults[7] = "6-10-105 7-10-111 8-15-111 9-26-125 0-35-35 4-43-68 1-45-80 3-47-77 2-53-88 5-83-88";
		expectedResults[8] = "7-0-111 8-5-111 9-16-125 0-25-35 4-33-68 1-35-80 3-37-77 2-43-88 5-73-88 6-105-105";
		expectedResults[9] = "8-5-111 9-16-125 0-25-35 4-33-68 1-35-80 3-37-77 2-43-88 5-73-88 6-105-105 7-111-111";
	}	
	
	void Update () 
	{
		if (!running)
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
}
