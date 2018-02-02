using UnityEngine;
using System.Collections;
using System.Linq;

public class CharacterComponent : UnitComponent 
{
	#region Character Status
	public override TargetTypeEnum foeType {get; protected set;}

	public override float maxHealth {get; protected set;}
	public override float baseDmg {get; protected set;}
	public override AttackType basicAttackType {get; protected set;}
	public override AttackType currentAttackType {get; protected set;}	

	[HideInInspector]
	public CharacterTypeEnum charType;

	[HideInInspector]
	public int charId;

	private bool isPlayer;
	#endregion

	public Transform camTransform;
	private GameObject charMesh;

	protected override void Start ()
	{
		base.Start ();

		//summon char prefab
		charMesh = CharacterPrefabManager.instance.RetrieveCharacterPrefab(charType);
		charMesh.transform.position = transform.position + Vector3.up;
		charMesh.transform.rotation = transform.rotation;
		charMesh.transform.parent = transform;
		
		//get statuses
		CharacterType ct = CharacterTypeDatabase.GetCharTypeOf(charType);
		maxHealth = ct.health;
		baseDmg = ct.baseDamage;
		basicAttackType = ct.attackType;

		health = maxHealth;
		
		foeType = TargetTypeEnum.Enemy;
		
		UpdateHPBar();
	}

	#region Status
	public void SetToPlayer()
	{
		if (!isPlayer)
			isPlayer = true;
	}

	public void RevokePlayer()
	{
		if (isPlayer)
			isPlayer = false;
	}

	public void UpdatePartyPosition()
	{	//update prev pos of respective charId
		GameMaster.instance.prevCharPosition[charId] = currentTile;
	}
	
	protected override void Die ()
	{
		print ("character is dead");
	}
	#endregion

	#region Turn
	public override void MyTurn ()
	{
		if (isPlayer)
			PlayerControl();
		else
			AIControl();
	}
	
	protected override void TurnEnd ()
	{
		if (isPlayer)
		{
			PlayerInput.instance.PlayerTurnEnd();
			UIManager.instance.DeactivateAllPanels();
			
			HighlightManager.instance.ClearHighlight();
		}
		
		base.TurnEnd();
	} 
	#endregion

	#region PLAYER possession
	private void PlayerControl()
	{
		PlayerInput.instance.SetCurrChar(this); //Set Player to input
		UIManager.instance.ActivatePanel(PanelEnum.PlayerTurn); //UI spawn choices
		CharCam.instance.FocusCamTo(this, camTransform); //set cam to this character

		//once player turn starts, default is movement

		//if in peace mode
		if (GameMaster.instance.battleState == BattleState.Peace)
		{
			if (base.movementComponent.TryMoveInPath(CollisionCheckEnum.Enemy)) //if there is previous path //*GONNA REMOVE THIS IN UE4
			{
				//success move in path
				UpdatePartyPosition();
				InvokeTurnEnd();
				return;
			}
			else //no previous path
			{
				base.movementComponent.GenerateMovementRange(MovementType.MovementPeace, CollisionCheckEnum.Enemy);
			}
		}
		else //in battle mode
		{
			base.movementComponent.GenerateMovementRange(MovementType.MovementCombat, CollisionCheckEnum.Enemy);	
		}

		PlayerInput.instance.PlayerSelectTile();
	}

	public bool PlayerReceiveMoveTile(Tile target)
	{
		//(*THIS CONDITIONAL IS CAUSED BY MOVEMENT PEACE USING MOUSE CLICK)
		//if target is the neighbour and contains character in there
		if (currentTile.neighbours.Contains(target) && GameMaster.instance.IsTileOccupied(target, CollisionCheckEnum.Character)) 
		{	//swap it
			SwapPosition(GameMaster.instance.GetUnitAt(target));
			InvokeTurnEnd();
			UpdatePartyPosition();
			return true;		
		}	
		else //target tile is empty or has enemy
		{
			if (base.movementComponent.TryMoveInsideRange(target, CollisionCheckEnum.Enemy)) //if succeed moving
			{
				InvokeTurnEnd();
				UpdatePartyPosition();
				return true;
			}
			else //if fail moving
			{
				return false;
			}
		}
	}

	public void PlayerChooseAttack()
	{
		if (currentAttackType == AttackType.Default)
		{
			currentAttackType = basicAttackType;
		}

		base.attackComponent.GenerateAttackRange(currentAttackType);
	}

	public bool PlayerReceiveAttackTile(Tile target)
	{
		if (base.attackComponent.TryAttackInsideRange(target))
		{
			InvokeTurnEnd();
			return true;
		}
		else
		{
			return false;
		}
	}

	public void HighlightMoveRange()
	{
		HighlightManager.instance.ShowHighlight(base.movementComponent.movementRange.Keys.ToList() ); //show highlight
	}
	
	public void HighlightAttackRange()
	{
		HighlightManager.instance.ShowHighlight(base.attackComponent.attackRange);
	}
	#endregion

	#region AI
	public override void AIControl ()
	{
		if (GameMaster.instance.battleState == BattleState.Peace)
		{
			//in peace can only do follow party
			if (FollowPartyFormation())
			{
				InvokeTurnEnd();
				UpdatePartyPosition();
			}
			else
			{
				TurnEnd();
			}
		}
		else //in battle
		{
			AIDecision();
		}
	}

	protected override void AIDecision ()
	{
		//first try to attack with default attack (since there is no skill yet)
		base.attackComponent.GenerateAttackRange(basicAttackType);

		PriorityQueue<EnemyComponent> enemyInRange = new PriorityQueue<EnemyComponent>();

		foreach (EnemyComponent en in GameMaster.instance.involvedInCombat)
		{
			if (base.attackComponent.CheckAttackInsideRange(en.currentTile))
			{
				float thisDist = currentTile.DistanceToTile(en.currentTile);
				enemyInRange.Enqueue(thisDist, en);
			}
				
		}

		EnemyComponent nearestEnemy = null;

		for (int i = 0; i < enemyInRange.Count; i++)
		{
			EnemyComponent currEnemy = enemyInRange.Dequeue();
			if (i == 0)
				nearestEnemy = currEnemy;

			if (base.attackComponent.TryAttackInsideRange(currEnemy.currentTile)) //try to attack enemy from nearest to furthest
			{
				InvokeTurnEnd();
				return;
			}
				
		}

		//if failed to attack all enemy, try to chase nearest enemy
		if (nearestEnemy != null && base.movementComponent.TryMoveInsideRange(nearestEnemy.currentTile, CollisionCheckEnum.All))
		{
			//char chase nearest enemy
			InvokeTurnEnd();
			UpdatePartyPosition();
			return;
		}

		//fail to follow closest enemy
		if (FollowPartyFormation())
		{
			InvokeTurnEnd();
			UpdatePartyPosition();
		}
		else
			TurnEnd();

	}
	
	private bool FollowPartyFormation()
	{
		Tile prevPartyPosition = GameMaster.instance.prevCharPosition[charId - 1];
		
		if (prevPartyPosition == null)
			return false;
		
		if (base.movementComponent.TryMoveDirectlyTo(prevPartyPosition, CollisionCheckEnum.All)) //success moved to previous position
			return true;
		else
			return false;
	}
	#endregion
	
}
