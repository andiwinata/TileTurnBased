using UnityEngine;
using System.Collections;

public class AttackTypeData  
{
	public int maxDist {get; private set;}
	public int areaDamageRange {get; private set;}
	public float dmgMulti {get; private set;}

	public AttackTypeData(int mDist, int aoeRange, float multiplier)
	{
		maxDist = mDist;
		areaDamageRange = aoeRange;
		dmgMulti = multiplier;
	}
}
