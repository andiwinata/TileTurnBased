using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DamageManager : MonoBehaviour 
{
	private static DamageManager _instance;
	public static DamageManager instance
	{
		get
		{
			if (_instance == null)
				_instance = GameObject.FindObjectOfType<DamageManager>();
			return _instance;
		}
	}

	public GameObject damageCollider;

	private List<GameObject> dmgColliders = new List<GameObject>();
	private int totalPooled = 5;

	void Awake () 
	{
		for (int i = 0; i < totalPooled; i++)
		{
			InstantiateDamager();
		}
	}

	public void SpawnDamager (float damage, float size, Vector3 pos, TargetTypeEnum targetType)
	{
		GameObject dmgr = GetDamager();
		dmgr.transform.localScale = Vector3.one * size;
		dmgr.transform.position = pos + Vector3.up;
		dmgr.SetActive(true);

		dmgr.GetComponent<Damager>().InitializeDamager(damage, size, targetType, 1f);
	}
	
	private GameObject GetDamager()
	{
		foreach (GameObject g in dmgColliders)
		{
			if (!g.activeInHierarchy)
				return g;
		}

		return InstantiateDamager();
	}

	private GameObject InstantiateDamager()
	{
		GameObject go = Instantiate (damageCollider) as GameObject;
		go.SetActive(false);
		go.transform.parent = transform;

		dmgColliders.Add (go);

		return go;
	}
}
