using UnityEngine;
using System.Collections;

public class ActivateEnemy : MonoBehaviour 
{
	void OnTriggerEnter(Collider col)
	{
		if (col.CompareTag("Enemy"))
		{
			EnemyComponent e = col.GetComponent<EnemyComponent>();
			if (e != null)
				e.enabled = true;
		}
	}

	void OnTriggerExit(Collider col)
	{
		if (col.CompareTag("Enemy"))
		{
			col.GetComponent<EnemyComponent>().enabled = false;
		}

	}
	
}
