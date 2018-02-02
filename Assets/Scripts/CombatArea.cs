using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TargetTypeEnum
{
	Character,
	Enemy
}

public class CombatArea : MonoBehaviour 
{
	public TargetTypeEnum targetType;
	private string targetTag;

	//public List<Unit> units {get; private set;}
	//private Unit myself;

	void Awake()
	{
		targetTag = targetType.GetStringValue();
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.CompareTag(targetTag))
		{
			EnemyComponent colEnemy = col.GetComponent<EnemyComponent>();
			GameMaster.instance.EnterCombat(colEnemy);
		}
	}

	void OnTriggerExit(Collider col)
	{
		if (col.CompareTag(targetTag))
		{
			EnemyComponent colEnemy = col.GetComponent<EnemyComponent>();
			GameMaster.instance.ExitCombat(colEnemy);
		}
	}
}
