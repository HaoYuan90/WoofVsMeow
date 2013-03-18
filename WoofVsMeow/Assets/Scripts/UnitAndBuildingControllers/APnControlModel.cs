using UnityEngine;

public class APnControlModel : MonoBehaviour
{
 	private int m_currentAP;
  	public int m_maxAP;
	
	private int m_debugID;

	public void Initialise()
	{
		//get this from data storage class
		//m_maxAP = 100;
		m_currentAP = m_maxAP;
		
		m_debugID = 0;
	}
	
	//random test instance
	public void InitialiseTestInstance(int id)
	{
		m_currentAP = Random.Range(1,500);
		m_maxAP = m_currentAP+Random.Range(0,50);
		m_debugID = id;
	}
	//fixed test instance
	public void InitialiseTestInstance(int id, int current, int max)
	{
		m_currentAP = current;
		m_maxAP = max;
		m_debugID = id;
	}
	
	//m_CurrentAP
	public void SetAP(int ap)
	{
		m_currentAP = ap;
	}
	public void ModifyAP(int ap)
	{
		m_currentAP += ap;
	}
	public int GetAP()
	{
		return m_currentAP;
	}
	
	public void ReplenishAP(double percentage)
	{
		m_currentAP = (int)percentage*m_maxAP;
	}
	
	//m_maxAP
	public void SetMax(int ap)
	{
		m_maxAP = ap;
	}
	public int GetMax()
	{
		return m_maxAP;
	}
	
	//For testing
	public int GetID()
	{
		return m_debugID;
	}
	
	public void Print()
	{
		Debug.Log("Index : " + m_debugID.ToString() + "     Current AP : " + m_currentAP.ToString() + "      Max AP : " + m_maxAP.ToString());
	}

	public override string ToString()
	{
		return m_debugID.ToString() + "-" + m_currentAP.ToString() + "-" + m_maxAP.ToString();
	}

}
