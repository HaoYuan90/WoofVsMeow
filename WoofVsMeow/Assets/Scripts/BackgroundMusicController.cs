using UnityEngine;
using System.Collections;

public class BackgroundMusicController : MonoBehaviour {
	
	public AudioClip m_backgroundMusic;
	
	// Use this for initialization
	void Start () {
		audio.clip = m_backgroundMusic;
		//Audio Source from http://incompetech.com/music/royalty-free/mp3-royaltyfree/Call%20to%20Adventure.mp3
		audio.loop = true;
		audio.volume = 1F;
		audio.Play();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
