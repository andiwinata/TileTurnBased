using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class AttackComponent : MonoBehaviour 
{
	//components
	private UnitComponent thisUnit;

	//statuses
	private AttackType currentAttackType;
	public List<Tile> attackRange {get; private set;}

	void Awake()
	{
		thisUnit = GetComponent<UnitComponent>();
	}

	public bool CheckAttackInsideRange(Tile target)
	{
		return attackRange.Contains(target);
	}

	public void GenerateAttackRange(AttackType at)
	{
		currentAttackType = at;	
		attackRange = TileRange.instance.SetAttackTileRange(currentAttackType, thisUnit.currentTile);
	}
	
	public bool TryAttackInsideRange(Tile target)
	{
		if (CheckAttackInsideRange(target))
		{
			AttackTypeData skill = ActionDatabase.GetAttackData(currentAttackType);
			float totalDmg = thisUnit.baseDmg * skill.dmgMulti;
			
			DamageManager.instance.SpawnDamager(totalDmg, skill.areaDamageRange, target.TileToWorldCoord(), thisUnit.foeType);
			SkillEffectManager.instance.SpawnSkillEffect(currentAttackType, target.TileToWorldCoord());

			return true;
		}
		else
			return false;
		
	}
}
