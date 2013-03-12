using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class TestAttribute : MonoBehaviour
{
	
	//this number is used to test reference serialisation
	public int testNum;
	//this is to test if the grids can hold reference to game object correctly and maintain its state
	public GameObject test;
}
