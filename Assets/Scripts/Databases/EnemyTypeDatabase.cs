using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EnemyTypeEnum
{
	Slime = 0,
	Goblin = 1,
	Phoenix = 2
}

public static class EnemyTypeDatabase 
{
	public static EnemyType slime {get; private set;}
	public static EnemyType goblin {get; private set;}
	public static EnemyType phoenix {get; private set;}

	static EnemyTypeDatabase()
	{
		slime = new EnemyType(20, 5, AttackType.MeleeAttack);
		goblin = new EnemyType(40, 10, AttackType.MeleeAttack);
		phoenix = new EnemyType(120, 15, AttackType.MeleeAttack);
	}

	public static EnemyType GetEnemyTypeOf(EnemyTypeEnum enemyType)
	{
		switch (enemyType)
		{
		case EnemyTypeEnum.Slime:
			return slime;

		case EnemyTypeEnum.Goblin:
			return goblin;

		case EnemyTypeEnum.Phoenix:
			return phoenix;
		}

		return null;
	}
}
