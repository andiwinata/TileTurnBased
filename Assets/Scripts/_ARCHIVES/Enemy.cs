/*using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Enemy : Unit 
{	
	public override float turnPoint {get; protected set;}	
	protected override TargetTypeEnum oppositeType {get; set;}

	private int turnSinceChase = 0;
	private bool possibleMove;
	private bool possibleAttack;

	private GameObject enemyMesh;
		
	[HideInInspector]
	public EnemyTypeEnum enemyType;

	//STATUSES
	protected override float maxHealth {get; set;}
	protected override float health {get; set;}
	protected override float baseDmg {get; set;}
	protected override AttackType basicAttackType {get; set;}

	protected override void Start()
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

		oppositeType = TargetTypeEnum.Character;

		UpdateHPBar();
	}
	
	void OnEnable()
	{	
		turnPoint = 1;
		GameMaster.instance.turnOrders.Enqueue(turnPoint, this);
	}

	void OnDisable()
	{
		if (GameMaster.instance != null)
			GameMaster.instance.turnOrders.RemoveItem(this);
	}

	public override void MyTurn (bool canMove, bool canAttack)
	{
		possibleMove = canMove;
		possibleAttack = canAttack;

		if (GameMaster.instance.battleState == BattleState.Peace && possibleMove)
			ChasePlayer();
		else
		{
			//try to attack
			GeneratePossibleAttackTiles(AttackType.Default);

			//try to attack random char in range
			PriorityQueue<CharacterComponent> charInRange = new PriorityQueue<CharacterComponent>();
			foreach (CharacterComponent ch in GameMaster.instance.charsList)
			{
				if (CheckAttackWithinRange(ch.currentTile))
					charInRange.Enqueue(currentTile.DistanceToTile(ch.currentTile), ch);

				if (TryAttackInsideRange(ch.currentTile))
				{
					//success attacking
					InvokeTurnEnd();
					return;
				}
				  
			}

			//if fail to attack
			//try to move to enemy
			if (charInRange.Count > 0 && TryMoveInsideRange(charInRange.Dequeue().currentTile, CollisionCheckEnum.All))
			{
				//success follow enemy
				InvokeTurnEnd();
			}
			else
				ChasePlayer();
		}

	}

	private void ChasePlayer()
	{
		if (turnSinceChase > 3) //every after 3 turn, update path so it will look quite stupid
		{
			GeneratePossibleMoveTiles(MovementType.MovementPeace, CollisionCheckEnum.All); //list all possible tiles to move
			Tile best = GetNearestToInsideRange(possibleMoveTiles.Keys.ToList(), GameMaster.instance.currPlayerChar.currentTile); //target the nearest tile to player in that range

			base.TryMakePathInsideRange(best); //generate the path to that tile
			turnSinceChase = 0;
		}

		if (TryMoveInPath(CollisionCheckEnum.All))
		{
			//success follow previous path
			InvokeTurnEnd();
		}
		else
		{
			//cant move //**later will be change to attack or etc
			TurnEnd();
		}
	}

	protected override void TurnEnd ()
	{
		turnSinceChase++;
		base.TurnEnd();
	}

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

}*/
