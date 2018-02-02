using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Damager : MonoBehaviour
{
	private float damage;
	private float rad;
	private string preferedTag;

	public void InitializeDamager(float dmg, float diameter, TargetTypeEnum prefTarget, float lifeTime)
	{
		damage = dmg;
		preferedTag = prefTarget.GetStringValue();
		rad = diameter / 2;

		StartCoroutine(Alive(lifeTime));

		Collider[] hitColliders = Physics.OverlapSphere(transform.position, diameter / 2);

		for (int i = 0; i < hitColliders.Length; i++)
		{
			if (hitColliders[i].CompareTag(preferedTag))
			{
				hitColliders[i].GetComponent<UnitComponent>().GetDamage(damage);
			}
		}
	}

	private IEnumerator Alive(float t)
	{
		yield return new WaitForSeconds(t);
		gameObject.SetActive(false);
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.green;
		//Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
		Gizmos.DrawWireSphere (transform.position, rad);
	}
}
