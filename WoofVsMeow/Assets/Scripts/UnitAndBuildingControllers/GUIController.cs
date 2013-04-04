using UnityEngine;
using System.Collections;

//display health or floating text
public class GUIController : MonoBehaviour 
{	
	//health bar
	private Rect m_rectangle;
	readonly private Vector2 m_offset = new Vector2(-15, 0);
	readonly private int m_barAnimationSpan = 240;
	private int m_barCurrentTimer;
	
	Texture2D m_background;
	Texture2D m_inter;
	Texture2D m_foreground;
	
	private int m_health;
	private int m_maxHealth;
	private float m_bridgeHealth;
	
	//floating text
	readonly private int m_textAnimationSpan = 120;
	private int m_textCurrentTimer;
	private string m_floatingText;
	private Rect m_textPos;
	private GUIStyle m_textStyle;
	
	readonly private float m_initHeight = 100;
	readonly private Rect m_textBox = new Rect(0,0,100,100);
	
	public void Initialise (int hp, int max)
	{
		//hp bar
		//apply texture
		m_background = new Texture2D(1,1);
		m_background.SetPixel(0,0, Color.red);
		m_background.Apply();
		m_inter = new Texture2D(1,1);
		m_inter.SetPixel(0,0, Color.Lerp(Color.red,Color.green,0.5f));
		m_inter.Apply();
		m_foreground = new Texture2D(1,1);
		m_foreground.SetPixel(0,0, Color.green);
		m_foreground.Apply();
		m_rectangle = new Rect(0, 0, 40, 6);
		
		m_health = hp;
		m_maxHealth = max;
		//floating text
		//nothing to init;
	}
	
	private void DisplayFloatingText(string msg)
	{
		m_textCurrentTimer = m_textAnimationSpan;
		m_floatingText = msg;
		Vector3 temp = Camera.main.WorldToScreenPoint(transform.position);
		m_textPos = new Rect(temp.x,Screen.height-temp.y-m_initHeight,m_textBox.width,m_textBox.height);
	}
	
	public void OnHealthLostBy(int amt)
	{
		DisplayFloatingText("-"+amt);
		//show hp bar
		m_bridgeHealth = m_health;
		m_barCurrentTimer = m_barAnimationSpan;
		m_health -= amt;
		if(m_health <0)
			m_health = 0;
	}
	
	void OnGUI()
	{
		GUI.depth = 1;
		//display floating text
		if(m_textCurrentTimer > 0)
		{
			m_textStyle = new GUIStyle(GUI.skin.label);
			m_textStyle.fontSize = 30;
			m_textStyle.fontStyle = FontStyle.Bold;
			m_textPos = new Rect(m_textPos.x, m_textPos.y-0.25f, m_textBox.width, m_textBox.height);
			GUI.Label(m_textPos, m_floatingText,m_textStyle);
			m_textCurrentTimer --;
		}
		
		//display health bar
		// Draw background red
		Vector3 topPos = new Vector3(transform.position.x,transform.position.y+6f,transform.position.z);
		m_rectangle.x = Camera.main.WorldToScreenPoint(topPos).x + m_offset.x;
		m_rectangle.y = Screen.height - Camera.main.WorldToScreenPoint(topPos).y + m_offset.y;	
		GUI.DrawTexture(m_rectangle, m_background);
		// Draw intermedia color
		if(m_barCurrentTimer > 0)
		{
			m_bridgeHealth -= m_bridgeHealth/m_barCurrentTimer;
			Rect interRect = m_rectangle;
			interRect.width = m_rectangle.width *m_bridgeHealth/m_maxHealth;
			GUI.DrawTexture(interRect, m_inter);
			m_barCurrentTimer --;
		}
		//Draw foreground green
		Rect partialRect = m_rectangle;
		partialRect.width = m_rectangle.width * (float)m_health/m_maxHealth;
		GUI.DrawTexture(partialRect, m_foreground);
	}
}