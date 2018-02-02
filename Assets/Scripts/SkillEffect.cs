using UnityEngine;
using System.Collections;

public class SkillEffect : MonoBehaviour 
{
	ParticleSystem ps;

	void Awake()
	{
		ps = GetComponent<ParticleSystem>();
	}

	void OnEnable()
	{
		StartCoroutine(Inactivate());
	}

	IEnumerator Inactivate()
	{
		yield return new WaitForSeconds(ps.duration);
		gameObject.SetActive(false);
	}
}
