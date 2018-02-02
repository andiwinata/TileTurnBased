/*using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public abstract class Unit : MonoBehaviour 
{
	public Slider hpSlider;
	public Text hpText;

	public Tile currentTile {get; protected set;}
	public abstract float turnPoint {get; protected set;}
	protected int totalActionCost = 1;

	private float movingTime = 0.1f;
	private bool isMoving = false;
	
	protected Dictionary<Tile, Tile> possibleMoveTiles;
	protected List<Tile> possibleAttackTiles;

	private List<Tile> steps;
	private int stepsId;

	protected AttackType currentAttackType;
	protected abstract TargetTypeEnum oppositeType {get; set;}

	public const float turnEndDelay = 0.2f;

	//STATUS
	protected abstract float maxHealth {get; set;}
	protected abstract float health {get; set;}
	protected abstract float baseDmg {get; set;}
	protected abstract AttackType basicAttackType {get; set;}
	protected bool die = false;

	protected void UpdateHPBar()
	{
		hpSlider.maxValue = health;
		hpSlider.value = health;
		hpText.text = (int) health + "/" + maxHealth;
	}

	protected virtual void Start()
	{
		UpdateHPBar();
	}
		
	public abstract void MyTurn(bool canMove, bool canAttack);

	public virtual void InvokeTurnEnd()
	{
		Invoke ("TurnEnd", turnEndDelay);
	}
	
	protected virtual void TurnEnd() //everytime action is done, call this
	{
		if (GameMaster.instance.battleState == BattleState.Peace)
		{
			health++;
			health = Mathf.Clamp(health, 0, maxHealth);
		}

		UpdateHPBar();
		//add turn cost
		turnPoint = totalActionCost;
		GameMaster.instance.turnOrders.Enqueue(turnPoint, this); //subscribe for next turn
		GameMaster.instance.NextTurn(); //end turn
	}

	//TELEPORT TO
	public void TeleportTo(Tile tile)
	{
		currentTile = tile;
		transform.position = tile.TileToWorldCoord();
	}

	//CHECK IF ACTION WITHIN RANGE
	public bool CheckMovementWithinRange(Tile target)
	{
		return possibleMoveTiles.ContainsKey(target);
	}

	public bool CheckAttackWithinRange(Tile target)
	{
		return possibleAttackTiles.Contains(target);
	}

	//-------ATTACK--------

	public void GeneratePossibleAttackTiles(AttackType at)
	{
		if (at == AttackType.Default)
		{
			currentAttackType = basicAttackType;
		}
		else
		{
			currentAttackType = at;
		}

		possibleAttackTiles = TileRange.instance.SetAttackTileRange(currentAttackType, currentTile);
	}
	
	public bool TryAttackInsideRange(Tile target)
	{
		if (CheckAttackWithinRange(target))
		{
			AttackTypeData skill = ActionDatabase.GetAttackData(currentAttackType);
			float totalDmg = baseDmg * skill.dmgMulti;

			DamageManager.instance.SpawnDamager(totalDmg, skill.areaDamageRange, target.TileToWorldCoord(), oppositeType );
			SkillEffectManager.instance.SpawnSkillEffect(currentAttackType, target.TileToWorldCoord());
			//Invoke("TurnEnd", 1f);
			return true;
		}
		else
			return false;
				
	}
		
	//----------MOVING----------	

	public void GeneratePossibleMoveTiles(MovementType at, CollisionCheckEnum colCheck)
	{
		possibleMoveTiles = TileRange.instance.SetMovementTileRange(at, currentTile, colCheck);
	}

	protected bool TryMakePathInsideRange(Tile goal) //the available tiles (possibleMoveTiles) already filter collision, no need to check again
	{	//only call it when discarding old path
		if (CheckMovementWithinRange(goal)) //need to have possible tiles made first (the dictionary)
		{
			steps = Pathfinding.GetPathFromDict(possibleMoveTiles, goal);
			stepsId = 0;
			return true;
		}
		else
		{
			steps = null;
			stepsId = 0;
			return false;
		}

	}

	protected bool TryMoveInPath(CollisionCheckEnum colCheck)
	{
		if (steps == null)
		{
			return false;
		}

		stepsId++;
		if (stepsId >= steps.Count) //if stepsId larger than list, it means previous path has done
		{
			steps = null;
			return false;
		}

		Tile nextInPath = steps[stepsId];
		if (!GameMaster.instance.IsTileOccupied(nextInPath, colCheck))
		{
			StartCoroutine(MovingToInstant(nextInPath));
			return true;
		}
		else
		{
			return false;
		}

	}
	
	protected bool TryMoveInstantTo(Tile target, CollisionCheckEnum colCheck)
	{
		if (!GameMaster.instance.IsTileOccupied(target, colCheck))
		{
			StartCoroutine(MovingToInstant(target));
			return true;
		}
		else
		{
			return false;
		}
	}

	public bool TryMoveInsideRange(Tile goal, CollisionCheckEnum colCheck) //shortcut
	{
		if (TryMakePathInsideRange(goal) && TryMoveInPath(colCheck)) //if path can be created
		{
			InvokeTurnEnd();
			return true;
		}
		else
			return false;
	}
	
	protected IEnumerator MovingToInstant(Tile target)
	{
		isMoving = true;
		
		Vector3 end = target.TileToWorldCoord();
		Debug.DrawLine(transform.position + Vector3.up, end + Vector3.up , Color.red, 10f);

		if (end != transform.position)
			yield return StartCoroutine(LerpTo(end));

		currentTile = target;
		isMoving = false;
	}	

	protected IEnumerator LerpTo(Vector3 target)
	{
		Vector3 from = transform.position;

		float time = 0;
		float rate = 1 / movingTime;
		while (time < 1)
		{
			time += rate * Time.deltaTime;
			transform.position = Vector3.Lerp(from, target, time);
			yield return null;
		}
	}
	
	protected Tile GetNearestToInsideRange(List<Tile> range, Tile target)
	{
		range.Add(currentTile); //if you add this, enemy wont back off if stick to player
		
		if (range == null)
		{
			return null;
		}
		
		int nearestId = 0;
		float closest = float.MaxValue;
		
		for (int i = 0; i < range.Count; i++)
		{
			float dist = range[i].DistanceToTile(target);
			if (dist < closest)
			{
				closest = dist;
				nearestId = i;
			}
		}
		
		return range[nearestId];
	}

	public void SwapPositionWith(Unit other)
	{
		Tile thisTile = currentTile;
		Tile otherTile = other.currentTile;

		StartCoroutine(this.MovingToInstant(otherTile));
		StartCoroutine(other.MovingToInstant(thisTile));
	}
	
	public void GetDamage(float dmg)
	{
		health -= dmg;
		UpdateHPBar();
		
		if (health <= 0)
		{
			Die ();
			die = true;
		}
			
	}

	protected abstract void Die();

}*/
