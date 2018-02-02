using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MovementType
{
	MovementPeace,
	MovementCombat,
}

public enum AttackType
{
	Default,
	MeleeAttack = 0,
	RangeAttack = 1,
}

public static class ActionDatabase
{
	//variable data
	public static ActionData movementPeace {get; private set;}
	public static ActionData movementCombat {get; private set;}

	public static AttackTypeData meleeAttack {get; private set;}
	public static AttackTypeData rangeAttack {get; private set;}

	static ActionDatabase()
	{
		//make the data when created
		movementPeace = new ActionData(10);
		movementCombat = new ActionData(3);

		meleeAttack = new AttackTypeData(3, 1, 1);
		rangeAttack = new AttackTypeData(7, 3, 1);
	}

	public static ActionData GetMovementData (MovementType mt)
	{
		switch (mt)
		{
		case MovementType.MovementPeace:
			return movementPeace;
			
		case MovementType.MovementCombat:
			return movementCombat;
		}
		return null;
	}

	public static AttackTypeData GetAttackData (AttackType at)
	{
		switch(at)
		{
		case AttackType.MeleeAttack:
			return meleeAttack;

		case AttackType.RangeAttack:
			return rangeAttack;
		}

		return null;
	}
}
