using UnityEngine;
using System.Collections;

public class GeneralizedUnit : MonoBehaviour 
{
	//control == 1 -> player 1
	//control == 2 -> player 2
	private int control;
	private int maxAP;
	private int currAP;
	private int apMinus;
	private GameObject controller; //A GameObject controller that oversees everything.
	private PriorityQueue pq; //The PQ.
	private int num; //A number id for the unit, Not sure if this is needed but it helps in debugging

	// Use this for initialization
	void Start () 
	{
		controller = GameObject.Find("Controller");
		pq = GameObject.Find("PQ2").GetComponent<PriorityQueue>();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	//After unit has completed its move, this function will take the saved ap value and
	//relay it to the PQ which will rearrange the PQ.
	void TurnElapses(int ap)
	{
		int tempAP = apMinus;
		apMinus = 0;
		currAP += ap;
		
		pq.TurnElapsed(this.gameObject, -tempAP);
		
		if (currAP > maxAP)
		{
			currAP = maxAP;
		}
		
	}
	
	//Can be used to initialize the unit
	public void Init(int curr, int max, int num, int control)
	{
		currAP = curr;
		maxAP = max;
		this.num = num;
		this.control = control;
	}
	
	//For Debugging
	public void Print()
	{
		Debug.Log(currAP.ToString() + ", " + num.ToString() + ",Control : " +  control.ToString());
	}
	public int GetAP()
	{
		return currAP;
	}
	
	public void ModifyAP(int ap)
	{

		currAP += ap;
		
		if (currAP < 0)
		{
			currAP = 0;
		}
		if (currAP > maxAP)
		{
			currAP = maxAP;
		}
	}
	
	//This function starts the units turn.
	//The AP that this unit has upon being dequeued from the PQ is saved as
	//apMinus which is then relayed back to the PQ.
	public void Call()
	{
		apMinus = currAP;
		currAP = 0;
		
		//......
		//......
		//......
		//Will call TurnElapses after the unit makes its move.
		TurnElapses(Random.Range (70,150));
	}
	
	public int GetControl()
	{
		return control;
	}
}
