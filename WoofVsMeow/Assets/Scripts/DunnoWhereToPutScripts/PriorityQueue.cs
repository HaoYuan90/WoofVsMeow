using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//This script should be attached to a separate object and should be the only script attached to that object
public class PriorityQueue : MonoBehaviour 
{
	private Queue<GameObject> q;
	private GeneralizedUnit gu;
	//The controller object is intended to oversee the whole game
	private Controller cc;
	private Queue<GameObject> printQ; //For printing out the queue

	void Start () 
	{
		q = new Queue<GameObject>();
		cc = GameObject.Find("Controller").GetComponent<Controller>();
		printQ = new Queue<GameObject>();
	
	}
	
	void Update () 
	{

	
	}
	
	public void DebugCount()
	{
		//Prints out the num of units in the Q
		Debug.Log(q.Count);
	}
	
	public void Print()
	{
		while (q.Count > 0)
		{
			q.Peek().GetComponent<GeneralizedUnit>().Print();
			printQ.Enqueue(q.Dequeue());
		}
		while (printQ.Count > 0)
		{
			q.Enqueue(printQ.Dequeue());
		}
	}
	
	public void EnQ(GameObject obj)
	{
		q.Enqueue(obj);
	}
	
	public GameObject DeQ()
	{
		return q.Dequeue();
	}
	
	//Adds a new unit to its place in the queue according to its current Ap
	public void Add(GameObject obj)
	{
		Queue<GameObject> tempQ = new Queue<GameObject>();
		GameObject tempObj;
		bool enqueued = false;
		GeneralizedUnit tempGU;
		int ap;
		gu = obj.GetComponent<GeneralizedUnit>();
		ap = gu.GetAP();
		
		if (q.Count == 0)
		{
			q.Enqueue(obj);
			return;
		}

		while (q.Count > 0)
		{
			tempObj = q.Dequeue();
			tempGU = tempObj.GetComponent<GeneralizedUnit>();
			if (tempGU.GetAP() <= ap)
			{
				tempQ.Enqueue(tempObj);
			}
			else
			{
				tempQ.Enqueue(obj);
				tempQ.Enqueue(tempObj);
				enqueued = true;
				while (q.Count > 0)
				{
					tempQ.Enqueue(q.Dequeue());
				}
			}			
		}
		
		while (tempQ.Count > 0)
		{
			q.Enqueue(tempQ.Dequeue());
		}
		if (!enqueued)
		{
			q.Enqueue(obj);
		}
		
	}
	
	//This will Dequeue the first element in the Queue and call the Call() function of that object.
	public void GiveControl()
	{
		if (q.Count > 0)
		{
		GameObject tempObj = q.Dequeue();
		GeneralizedUnit tempGU = tempObj.GetComponent<GeneralizedUnit>();
		cc.SetControl(tempGU.GetControl()); //Sets the value of control in the Controller
		tempGU.Call();
		}
	}
	
	//Same as Add, except apMinus is added to all the objects as they are dequeued.
	//This function is used after a unit has finished its turn
	public void TurnElapsed(GameObject obj, int apMinus)
	{
		Queue<GameObject> tempQ = new Queue<GameObject>();
		GameObject tempObj;
		bool enqueued = false;
		GeneralizedUnit tempGU;
		int ap;
		gu = obj.GetComponent<GeneralizedUnit>();
		ap = gu.GetAP();
		
		while (q.Count > 0)
		{
			tempObj = q.Dequeue();
			tempGU = tempObj.GetComponent<GeneralizedUnit>();
			tempGU.ModifyAP(apMinus);
			if (tempGU.GetAP() <= ap)
			{
				tempQ.Enqueue(tempObj);
			}
			else
			{
				tempQ.Enqueue(obj);
				tempQ.Enqueue(tempObj);
				enqueued = true;
				while (q.Count > 0)
				{
					tempQ.Enqueue(q.Dequeue());
				}
			}			
		}
		
		while (tempQ.Count > 0)
		{
			q.Enqueue(tempQ.Dequeue());
		}
		if (!enqueued)
		{
			q.Enqueue(obj);
		}
		
	}
	
	public void Remove(GameObject obj)
	{
		Queue<GameObject> tempQ = new Queue<GameObject>();
		GameObject tempObj;
		while (q.Count > 0)
		{
			tempObj = q.Dequeue();
			
			if (tempObj.Equals(obj))
			{
			}
			else
			{
				tempQ.Enqueue(tempObj);
			}
		}
		
		while (tempQ.Count > 0)
		{
			q.Enqueue(tempQ.Dequeue());
		}
	}
	
	public int Count()
	{
		return q.Count;
	}
	
	public GameObject Peek()
	{
		return q.Peek();
	}
		
}
