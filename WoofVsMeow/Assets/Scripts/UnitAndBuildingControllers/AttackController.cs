using UnityEngine;
using System.Collections;

public class AttackController : MonoBehaviour 
{
	private int m_floatingTextLife;
	private string m_floatingText;
	private Rect m_textPos;
	private GUIStyle m_textStyle;
	
	readonly private float m_initHeight = 100;
	readonly private int m_floatingTextMaxLife = 120;
	readonly private Rect m_textBox = new Rect(0,0,100,100);
	
	public void Initialise ()
	{
		m_floatingTextLife = 0;
		m_floatingText = null;
		m_textPos = new Rect();
	}
	
	//perform the animation related to attack
	public void DoAttack (GameObject tar)
	{
		//face the right direction and play animation
		Vector3 dir = tar.transform.position-transform.position;
		transform.rotation = Quaternion.LookRotation(new Vector3(dir.x,0,dir.z));
		animation.Play ("attack");
	}
	
	public int AttackUnit (GameObject tar, int dmg)
	{
		DoAttack (tar);
		//calculate damage and decrease target hp
		//get target armor type and calculate
		int realDmg = dmg;
		tar.GetComponent<UnitController>().LoseHealthBy(realDmg);
		GetComponent<UnitController>().AttackFinished();
		return realDmg;
	}
	
	public int AttackBuilding (GameObject tar, int dmg)
	{
		DoAttack (tar);
		int realDmg = dmg;
		/*
		tar.GetComponent<UnitController>().LoseHealthBy(realDmg);*/
		GetComponent<UnitController>().AttackFinished();
		return realDmg;
	}
	
	public void DisplayFloatingText(string msg)
	{
		m_floatingTextLife = m_floatingTextMaxLife;
		m_floatingText = msg;
		Vector3 temp = Camera.main.WorldToScreenPoint(transform.position);
		m_textPos = new Rect(temp.x,Screen.height-temp.y-m_initHeight,m_textBox.width,m_textBox.height);
	}
	
	void OnGUI()
	{
		if(m_floatingTextLife > 0)
		{
			m_textStyle = new GUIStyle(GUI.skin.label);
			m_textStyle.fontSize = 30;
			m_textStyle.fontStyle = FontStyle.Bold;
			m_floatingTextLife --;
			m_textPos = new Rect(m_textPos.x, m_textPos.y-0.25f, m_textBox.width, m_textBox.height);
			GUI.Label(m_textPos, m_floatingText,m_textStyle);
		}
	}
}
