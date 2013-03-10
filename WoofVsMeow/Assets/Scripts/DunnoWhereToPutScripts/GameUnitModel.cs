using UnityEngine;
using System.Collections;

public class GameUnitModel : MonoBehaviour
{
  private int m_current_AP;
  private int m_max_AP;
	private int m_control;
	private int m_active;
	private int m_id_no;
	private int debug_timer;

	// Use this for initialization
	public void Start () 
	{
		m_active = 0;
		debug_timer = 0;
	}

	// Update is called once per frame
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
	}

	public void Initialize(int id, int curr, int max, int control)
	{
		m_id_no = id;
		m_current_AP = curr;
		m_max_AP = max;
		m_control = control;
	}

	public void Set_Ap(int ap)
	{
		m_current_AP = ap;
	}
	public void Modify_Ap(int ap)
	{
		m_current_AP += ap;
	}
	public int Get_AP()
	{
		return m_current_AP;
	}

	public void Activate()
	{
		m_current_AP=0;
		m_active = 1;
		debug_timer = 6;
	}

	public void Set_Control(int c)
	{
		m_control = c;
	}

	public int Get_Control()
	{
		return m_control;
	}
	public void Deactivate()
	{
		m_active = 0;
	}
	public void Set_Max(int ap)
	{
		m_max_AP = ap;
	}
	public int Get_Max()
	{
		return m_max_AP;
	}

	public void TurnEnd()
	{
		m_active = 0;
		GameObject.Find("CPU").GetComponent<CPU>().TurnEnd(gameObject);
	}

	public void Print()
	{
		Debug.Log("Index : " + m_id_no.ToString() + "     Current AP : " + m_current_AP.ToString());
	}

}
