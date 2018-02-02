using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum CharacterTypeEnum
{
	Warrior = 0,
	Archer = 1,
	Mage = 2,
}

public static class CharacterTypeDatabase 
{
	public static CharacterType warrior {get; private set;}
	public static CharacterType archer {get; private set;}
	public static CharacterType mage {get; private set;}

	static CharacterTypeDatabase()
	{
		warrior = new CharacterType(100, 10, AttackType.MeleeAttack);
		archer = new CharacterType(70, 20, AttackType.RangeAttack);
		mage = new CharacterType(60, 25, AttackType.RangeAttack);
	}

	public static CharacterType GetCharTypeOf(CharacterTypeEnum charTypeEn)
	{
		switch(charTypeEn)
		{
		case CharacterTypeEnum.Warrior:
			return warrior;

		case CharacterTypeEnum.Archer:
			return archer;

		case CharacterTypeEnum.Mage:
			return mage;
		}

		return null;
	}
}
