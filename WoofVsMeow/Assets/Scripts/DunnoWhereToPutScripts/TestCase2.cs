using UnityEngine;
using System.Collections;

public class TestCase2 : MonoBehaviour 
{
	private GameObject tempUnit; 
	private PriorityQueue pq;
	GameObject[] objArray;

	// Use this for initialization
	void Start () 
	{
		pq = GameObject.Find("PQ2").GetComponent<PriorityQueue>();
		objArray = new GameObject[50];
		
		for (int i=0; i<50; i++)
		{
			objArray[i] = new GameObject();
			objArray[i].AddComponent("GeneralizedUnit");
		}
		
		for (int m=0; m<50; m++)
		{
			objArray[m].GetComponent<GeneralizedUnit>().Init(Random.Range(1,500), 500, m+1, Random.Range(1,2));
		}
		
		for (int j=0; j<50; j++)
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
