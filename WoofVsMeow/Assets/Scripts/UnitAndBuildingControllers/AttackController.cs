using UnityEngine;
using System.Collections;

public class AttackController : MonoBehaviour 
{
	private int m_attackRange;
	public int attackRange
	{
		get{return m_attackRange;}
	}
	
	private int m_floatingTextLife;
	private string m_floatingText;
	private Rect m_textPos;
	private GUIStyle m_textStyle;
	
	readonly private float m_initHeight = 100;
	readonly private int m_floatingTextMaxLife = 120;
	readonly private Rect m_textBox = new Rect(0,0,100,100);
	
	public void Initialise ()
	{
		//get this from data file
		m_attackRange = 1;
		
		m_floatingTextLife = 0;
		m_floatingText = null;
		m_textPos = new Rect();
	}
	
	//grid under enemy is passed in
	public void Attack(GameObject tar)
	{
		//face the right direction
		Vector3 dir = tar.transform.position-transform.position;
		transform.rotation = Quaternion.LookRotation(new Vector3(dir.x,0,dir.z));
		//decrease health and whatever shit
		DisplayFloatingText(tar,"-50");
		animation.Play ("attack");
		GetComponent<UnitController>().AttackFinished();
	}
	
	private void DisplayFloatingText(GameObject target, string msg)
	{
		m_floatingTextLife = m_floatingTextMaxLife;
		m_floatingText = msg;
		Vector3 temp = Camera.main.WorldToScreenPoint(target.transform.position);
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
