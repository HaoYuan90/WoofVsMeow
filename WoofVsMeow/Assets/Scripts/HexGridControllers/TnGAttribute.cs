using UnityEngine;
using System.Collections;
using System;

[Serializable]
//terrain and gamestate attributes
public class TnGAttribute : MonoBehaviour
{
	//allow 1 to 10, enforced by grid generator
	public int m_height = 1;
	public GameObject m_building;
	public GameObject m_unit;
	
	public void MirrorTnGAttributesFrom(TnGAttribute src)
	{
		m_height = src.m_height;
		if(src.m_unit != null){
			m_unit = (GameObject)Instantiate(src.m_unit);
			m_unit.name = src.m_unit.name;
		}
		if(src.m_building != null){
			m_building = (GameObject)Instantiate(src.m_building);
			m_building.name = src.m_building.name;
		}
	}
}
