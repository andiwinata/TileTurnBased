using UnityEngine;
using System.Collections;

public class EnemyType
{
	//list of stats
	public float health {get; private set;}
	public float baseDamage {get; private set;}
	
	//list of skills
	public AttackType attackType {get; private set;}
	
	public EnemyType(float hp, float bDmg, AttackType attType)
	{
		health = hp;
		baseDamage = bDmg;
		attackType = attType;
	}
}
