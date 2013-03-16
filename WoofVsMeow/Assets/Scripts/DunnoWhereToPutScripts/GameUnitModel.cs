public class GameUnitModel : MonoBehaviour
{
 	private int m_current_AP;
  	private int m_max_AP;
	private int m_control;
	private int m_active;
	
	private int m_debugID;
	//private int m_id_no;
	//private int debug_timer;

	// Use this for initialization
	public void Start () 
	{
		m_active = 0;
	}

	/*
	void Update () 
	{
		if (debug_timer > 0)
		{
			debug_timer--;
			if (debug_timer == 0)
			{
				TurnEnd();
			}
		}
	}*/

	public void Initialize(int curr, int max, int control)
	{
		m_current_AP = curr;
		m_max_AP = max;
		m_control = control;
		
		m_debugID = 0;
	}
	
	public void InitialiseTestInstance(int id, int control)
	{
		m_current_AP = Random.Range(1,500);
		m_max_AP = m_current_AP+Random.Range(0,50);
		m_control = control;
		m_debugID = id;
	}
	
	public void InitialiseTestInstance(int id, int current, int max, int control)
	{
		m_current_AP = current;
		m_max_AP = max;
		m_control = control;
		m_debugID = id;
	}

	public void SetAP(int ap)
	{
		m_current_AP = ap;
	}
	public void ModifyAP(int ap)
	{
		m_current_AP += ap;
	}
	public int GetAP()
	{
		return m_current_AP;
	}
	
	public int GetID()
	{
		return m_debugID;
	}

	public void Activate()
	{
		m_current_AP=0;
		m_active = 1;
	}

	public void SetControl(int c)
	{
		m_control = c;
	}

	public int GetControl()
	{
		return m_control;
	}
	public void Deactivate()
	{
		m_active = 0;
	}
	public void SetMax(int ap)
	{
		m_max_AP = ap;
	}
	public int GetMax()
	{
		return m_max_AP;
	}

	public void TurnEnd()
	{
		m_active = 0;
		GameObject.Find("APSequenceController").GetComponent<APSequenceController>().OnTurnEnd();
	}

	public void Print()
	{
		Debug.Log("Index : " + m_debugID.ToString() + "     Current AP : " + m_current_AP.ToString() + "      Max AP : " + m_max_AP.ToString());
	}
	
	//For testing
	public string ToString()
	{
		return m_debugID.ToString() + "-" + m_current_AP.ToString() + "-" + m_max_AP.ToString();
	}

}
