using UnityEngine;
using System.Collections;
using System.Linq;

public class EnemyComponent : UnitComponent 
{
	public override TargetTypeEnum foeType {get; protected set;}
	public override AttackType basicAttackType {get; protected set;}
	public override AttackType currentAttackType {get; protected set;}

	public override float maxHealth {get; protected set;}
	public override float baseDmg {get; protected set;}

	private GameObject enemyMesh;
	
	[HideInInspector]
	public EnemyTypeEnum enemyType;
	private int turnSinceChase;

	protected override void Start ()
	{
		base.Start();

		//summon enemy prefab
		enemyMesh = EnemyPrefabManager.instance.RetrieveEnemyPrefab(enemyType);
		enemyMesh.transform.position = transform.position + Vector3.up;
		enemyMesh.transform.rotation = transform.rotation;
		enemyMesh.transform.parent = transform;
		
		//get statuses
		EnemyType et = EnemyTypeDatabase.GetEnemyTypeOf(enemyType);
		maxHealth = et.health;
		health = et.health;
		baseDmg = et.baseDamage;
		basicAttackType = et.attackType;
		
		foeType = TargetTypeEnum.Character;
		
		UpdateHPBar();
	}
	
	void OnEnable()
	{	
		GameMaster.instance.turnOrders.Enqueue(base.queuePoint, this);
		turnSinceChase = int.MaxValue;
	}
	
	void OnDisable()
	{
		if (GameMaster.instance != null)
			GameMaster.instance.turnOrders.RemoveItem(this);
	}


	#region Status
	protected override void Die ()
	{
		GameMaster.instance.RemoveEnemy(this);
		GameMaster.instance.turnOrders.RemoveItem(this);
		GameMaster.instance.ExitCombat(this);
		
		StartCoroutine(Dying ());
		print ("enemy die");
	}
	
	private IEnumerator Dying()
	{
		transform.position += Vector3.up * 5;
		yield return new WaitForSeconds(2);
		gameObject.SetActive(false);
	}
	#endregion

	#region Turn
	public override void MyTurn ()
	{
		AIControl();
	}

	protected override void TurnEnd ()
	{
		base.TurnEnd ();
		turnSinceChase++;
	}
	#endregion

	#region AI
	public override void AIControl ()
	{
		if (GameMaster.instance.battleState == BattleState.Peace)
			ChasePlayer();
		else
			AIDecision();
	}
	protected override void AIDecision ()
	{
		//set skill to basic attack
		currentAttackType = basicAttackType;
		//try to attack
		base.attackComponent.GenerateAttackRange(currentAttackType);
		
		//try to attack nearest char in range
		PriorityQueue<CharacterComponent> charInRange = new PriorityQueue<CharacterComponent>();
		foreach (CharacterComponent ch in GameMaster.instance.charsList)
		{
			if (base.attackComponent.CheckAttackInsideRange(ch.currentTile))
			{
				float thisDist = currentTile.DistanceToTile(ch.currentTile);
				charInRange.Enqueue(thisDist, ch);
			}
		}

		CharacterComponent nearestChar = null;
		
		for (int i = 0; i < charInRange.Count; i++)
		{
			CharacterComponent currChar = charInRange.Dequeue();
			if (i == 0)
				nearestChar = currChar;
			
			if (base.attackComponent.TryAttackInsideRange(currChar.currentTile)) //try to attack char from nearest to furthest
			{
				InvokeTurnEnd();
				return;
			}
			
		}
		
		//if failed to attack all char, try to chase nearest char
		if (nearestChar != null && base.movementComponent.TryMoveInsideRange(nearestChar.currentTile, CollisionCheckEnum.All))
		{
			//char chase nearest char
			InvokeTurnEnd();
		}
		else //fail to follow char
		{
			ChasePlayer();
		}
	}

	private void ChasePlayer()
	{
		if (turnSinceChase > 3) //every after 3 turn, update path so it will look quite stupid
		{
			base.movementComponent.GenerateMovementRange(MovementType.MovementPeace, CollisionCheckEnum.All); //list all possible tiles to move
			Tile best = GetNearestInRange(base.movementComponent.movementRange.Keys.ToList(), 
			                              GameMaster.instance.currPlayerChar.currentTile); //target the nearest tile to player in that range
			
			base.movementComponent.TryMakePathInsideRange(best); //generate the path to that tile
			turnSinceChase = 0;
		}
		
		if (base.movementComponent.TryMoveInPath(CollisionCheckEnum.All))
		{
			//success follow previous path
			InvokeTurnEnd();
		}
		else
		{
			//cant chase
			TurnEnd();
		}
	}
	#endregion
}
