using UnityEngine;
using System.Collections;

public class CharacterType 
{
	//list of stats
	public float health {get; private set;}
	public float baseDamage {get; private set;}
	
	//list of skills
	public AttackType attackType {get; private set;}
	
	public CharacterType(float hp, float bDmg, AttackType attType)
	{
		health = hp;
		baseDamage = bDmg;
		attackType = attType;
	}
}
