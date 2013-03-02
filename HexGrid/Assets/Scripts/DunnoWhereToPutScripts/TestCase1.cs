using UnityEngine;
using System.Collections;

public class TestCase1 : MonoBehaviour 
{
	private GameObject tempUnit; 
	private PriorityQueue pq;
	GameObject[] objArray;

	// Use this for initialization
	void Start () 
	{
		pq = GameObject.Find("PQ").GetComponent<PriorityQueue>();
		objArray = new GameObject[15];
		
		for (int i=0; i<15; i++)
		{
			objArray[i] = new GameObject();
			objArray[i].AddComponent("GeneralizedUnit");
		}
		
		
		objArray[0].GetComponent<GeneralizedUnit>().Init(15,150,1,1);
		objArray[1].GetComponent<GeneralizedUnit>().Init(45,150,2,1);
		objArray[2].GetComponent<GeneralizedUnit>().Init(12,150,3,2);
		objArray[3].GetComponent<GeneralizedUnit>().Init(55,150,4,1);
		objArray[4].GetComponent<GeneralizedUnit>().Init(64,150,5,1);
		objArray[5].GetComponent<GeneralizedUnit>().Init(10,150,6,2);
		objArray[6].GetComponent<GeneralizedUnit>().Init(44,150,7,1);
		objArray[7].GetComponent<GeneralizedUnit>().Init(39,150,8,1);
		objArray[8].GetComponent<GeneralizedUnit>().Init(8,150,9,2);
		objArray[9].GetComponent<GeneralizedUnit>().Init(22,150,10,1);
		objArray[10].GetComponent<GeneralizedUnit>().Init(30,150,11,2);
		objArray[11].GetComponent<GeneralizedUnit>().Init(10,150,12,1);
		objArray[12].GetComponent<GeneralizedUnit>().Init(44,150,13,2);
		objArray[13].GetComponent<GeneralizedUnit>().Init(77,150,14,2);
		objArray[14].GetComponent<GeneralizedUnit>().Init(88,150,15,1);
		
		for (int j=0; j<15; j++)
		{
			pq.Add(objArray[j]);
		}
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Press P to see the contents of the Queue
		if (Input.GetKeyDown(KeyCode.P))
		{
			pq.Print();
		}
		
		//Press T to elapse the Turn
		else if (Input.GetKeyDown(KeyCode.T))
		{
			pq.GiveControl();
		}
		
		//Press C to display the count
		else if (Input.GetKeyDown(KeyCode.C))
		{
			pq.DebugCount();
		}
	
	}
}
