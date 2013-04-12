using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundEffectController : MonoBehaviour {
	
	public AudioClip m_catSoldierAttack;
	public AudioClip m_catArcherAttack;
	public AudioClip m_dogSoldierAttack;
	public AudioClip m_dogGunnerAttack;
	
	// Use this for initialization
	void Start () {
	
	}
	
	public void PlayAttackSoundEffect(GameObject src) {
		if (src.name=="catSoldier")
			audio.PlayOneShot(m_catSoldierAttack, 1F);
		else if (src.name=="catArcher")
			audio.PlayOneShot(m_catArcherAttack, 1F);
		else if (src.name=="dogSoldier")
			audio.PlayOneShot(m_dogSoldierAttack, 1F);
		else if (src.name=="dogGunner")
			audio.PlayOneShot(m_dogGunnerAttack, 1F);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
