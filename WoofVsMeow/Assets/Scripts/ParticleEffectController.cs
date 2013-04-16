using UnityEngine;
using System.Collections;

public class ParticleEffectController : MonoBehaviour {

	void Start () {
		StartCoroutine(SelfDestruct());
	}
	
	IEnumerator SelfDestruct()
	{
		yield return new WaitForSeconds(5.0f);
		Destroy(gameObject);
	}
}
