using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public abstract class UnitComponent : MonoBehaviour 
{	
	//turn status
	public const float turnEndDelay = 0.2f;
	protected float totalActionCost = 1;
	[HideInInspector]
	public float queuePoint = 1;
	
	[Header("Individual UI")]
	public Slider hpSlider;
	public Text hpText;

	//foe status
	public abstract TargetTypeEnum foeType {get; protected set;} //the enemy of this unit type (for attack)

	//base status
	public abstract float maxHealth {get; protected set;}
	public abstract float baseDmg {get; protected set;}
	public abstract AttackType basicAttackType {get; protected set;}
	public abstract AttackType currentAttackType {get; protected set;}

	//live status
	private float _health;
	protected float health 
	{
		get {return _health;}
		set {_health = Mathf.Clamp(value, 0, maxHealth);}
	}
	protected bool die = false;

	//location
	public Tile currentTile {get; protected set;}

	//components
	protected MovementComponent movementComponent;
	protected AttackComponent attackComponent;

	void Awake()
	{
		movementComponent = GetComponent<MovementComponent>();
		attackComponent = GetComponent<AttackComponent>();
	}

	protected virtual void Start()
	{
		/*movementComponent = GetComponent<MovementComponent>();
		attackComponent = GetComponent<AttackComponent>();*/
	}

	#region status
	protected void UpdateHPBar()
	{
		hpSlider.maxValue = health;
		hpSlider.value = health;
		hpText.text = (int) health + "/" + maxHealth;
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
	#endregion

	#region Turn	
	public abstract void MyTurn();
	
	protected void InvokeTurnEnd()
	{
		Invoke("TurnEnd", turnEndDelay);
	}
	
	protected virtual void TurnEnd()
	{
		if (GameMaster.instance.battleState == BattleState.Peace)
		{
			health++;
		}
		
		UpdateHPBar();
		//add turn cost
		queuePoint = totalActionCost;
		GameMaster.instance.turnOrders.Enqueue(queuePoint, this); //subscribe for next turn
		GameMaster.instance.NextTurn(); //end turn
	}
	#endregion

	#region Location	
	public void ChangeCurrentTile(Tile tile)
	{
		currentTile = tile;
	}
	
	public void TeleportTo(Tile tile)
	{
		ChangeCurrentTile(tile);
		transform.position = tile.TileToWorldCoord();
	}
	
	public void SwapPosition(UnitComponent other)
	{
		Tile thisTile = currentTile;
		Tile otherTile = other.currentTile;
		
		StartCoroutine(movementComponent.MovingDirectlyTo(otherTile));
		StartCoroutine(other.GetComponent<MovementComponent>().MovingDirectlyTo(thisTile));
	}

	protected Tile GetNearestInRange(List<Tile> range, Tile target)
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
	#endregion

	#region AI
	public abstract void AIControl();
	protected abstract void AIDecision();
	#endregion
}
