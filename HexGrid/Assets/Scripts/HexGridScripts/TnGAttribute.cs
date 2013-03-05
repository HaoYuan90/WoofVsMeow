using UnityEngine;
using System.Collections;
using System;

public enum TerrainType
{
	plain,
	forest,
	hill,
	obstacle //debug
};

[Serializable]
//terrain and gamestate attributes
public class TnGAttribute : MonoBehaviour
{
	public TerrainType m_terrainType = TerrainType.plain;
	public GameObject m_building;
	public GameObject m_unit;
}
