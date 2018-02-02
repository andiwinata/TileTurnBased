using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillEffectManager : MonoBehaviour 
{
	private static SkillEffectManager _instance;
	public static SkillEffectManager instance
	{
		get
		{
			if (_instance == null)
				_instance = GameObject.FindObjectOfType<SkillEffectManager>();
			return _instance;
		}
	}

	public GameObject[] skillEffectsPrefab;

	private List<List<GameObject>> skillEffects = new List<List<GameObject>>();
	private int totalPooled = 2;

	void Awake()
	{
		for (int i = 0; i < skillEffectsPrefab.Length; i++)
		{
			List<GameObject> newList = new List<GameObject>();
			for (int j = 0; j < totalPooled; j++)
			{
				GameObject effect = Instantiate(skillEffectsPrefab[i]) as GameObject;
				effect.SetActive(false);

				newList.Add(effect);
			}
			skillEffects.Add(newList);
		}
	}

	public void SpawnSkillEffect(AttackType attType, Vector3 loc)
	{
		GameObject fx = GetSkillEffect(attType);
		fx.transform.position = loc + Vector3.up;
		fx.SetActive(true);
	}

	public GameObject GetSkillEffect(AttackType attType)
	{
		int id = (int) attType;

		foreach (GameObject go in skillEffects[id])
		{
			if (!go.activeInHierarchy)
				return go;
		}

		GameObject obj = Instantiate(skillEffectsPrefab[id]) as GameObject;
		obj.SetActive(false);

		skillEffects[id].Add(obj);
		return obj;
	}
}
