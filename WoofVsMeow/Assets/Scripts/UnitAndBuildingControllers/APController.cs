using UnityEngine;

public class APController : MonoBehaviour
{
	private int m_maxAP;
 	private int m_currentAP;
	public int currentAP{
		get{return m_currentAP;}
	}
	
	private int m_debugID;

	public void Initialise(int maxAP)
	{
		m_maxAP = maxAP;
		m_currentAP = m_maxAP;
		
		m_debugID = 0;
	}
	
	public void DecreaseAP(int ap)
	{
		m_currentAP -= ap;
		if(m_currentAP < 0){
			Debug.LogError("ap is less than 0, should not happen");
			Debug.LogError(gameObject.name);
			if(gameObject.tag == "Unit"){
				IntVector2 temp = GetComponent<UnitController>().GetPositionOnMap();
				Debug.Log("position is at"+temp.x+" "+temp.y);
			}
			if(gameObject.tag == "Building"){
				IntVector2 temp = GetComponent<BuildingController>().GetPositionOnMap();
				Debug.Log("position is at"+temp.x+" "+temp.y);
			}
			m_currentAP= 0;
		}
	}
	public int ReplenishAP(double percentage)
	{
		m_currentAP = (int)(percentage*m_maxAP);
		return m_currentAP;
	}
	public void RPCSetAP(int ap)
	{
		m_currentAP = ap;
	}
	
	//For testing
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
		return gameObject.name + "-" + m_currentAP.ToString() + "-" + m_maxAP.ToString();
	}

}
