/*using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Character : Unit 
{
	public Transform camTransform;
	public override float turnPoint {get; protected set;}

	protected override TargetTypeEnum oppositeType {get; set;}

	private bool isPlayer = false;
	private bool possibleMove;
	private bool possibleAttack;

	private GameObject charMesh;
		
	[HideInInspector]
	public int charId;

	[HideInInspector]
	public CharacterTypeEnum charType;

	//STATUSES
	protected override float maxHealth {get; set;}
	protected override float health {get; set;}
	protected override float baseDmg {get; set;}
	protected override AttackType basicAttackType {get; set;}

	protected override void Start()
	{
		base.Start();
		//summon char prefab
		charMesh = CharacterPrefabManager.instance.RetrieveCharacterPrefab(charType);
		charMesh.transform.position = transform.position + Vector3.up;
		charMesh.transform.rotation = transform.rotation;
		charMesh.transform.parent = transform;

		//get statuses
		CharacterType ct = CharacterTypeDatabase.GetCharTypeOf(charType);
		maxHealth = ct.health;
		health = ct.health;
		baseDmg = ct.baseDamage;
		basicAttackType = ct.attackType;

		oppositeType = TargetTypeEnum.Enemy;

		UpdateHPBar();
	}
	
	void OnEnable()
	{
		turnPoint = 0;
	}

	public void SetToPlayer()
	{
		isPlayer = true;
	}

	public void RevokePlayer()
	{
		isPlayer = false;
	}

	public void UpdatePrevPosition()
	{
		//update prev pos of respective charId
		GameMaster.instance.prevCharPosition[charId] = currentTile;
	}

	public override void MyTurn (bool canMove, bool canAttack)
	{
		possibleMove = canMove;
		possibleAttack = canAttack;
		//print ("my char " + charId + " turn, can Move: " + possibleMove + ", can Attack: " + possibleAttack);

		if (isPlayer)
		{
			PlayerControl();
		}
		else
		{
			//Controller by AI
			AIControl();
		}
	}

	private void PlayerControl()
	{
		//Set Player to input
		PlayerInput.instance.SetCurrChar(this);
		//UI spawn choices
		UIManager.instance.ActivatePanel(PanelEnum.PlayerTurn);
		//set cam to this character
		CharCam.instance.FocusCamTo(this, camTransform);

		if (possibleMove)
		{	
			if (GameMaster.instance.battleState == BattleState.Peace && //if in peace
			    base.TryMoveInPath(CollisionCheckEnum.Enemy)) //try to move in previous path and if there is no enemy in path
			{
				//succees moving to next in path
				UpdatePrevPosition();
				InvokeTurnEnd();
			}
			else //player either has no path or in battle mode
			{
				//generate possible tile of movement
				if (GameMaster.instance.battleState == BattleState.Peace)
					GeneratePossibleMoveTiles(MovementType.MovementPeace, CollisionCheckEnum.Enemy);
				else if (GameMaster.instance.battleState == BattleState.Battle)
					GeneratePossibleMoveTiles(MovementType.MovementCombat, CollisionCheckEnum.Enemy); 

				PlayerInput.instance.PlayerSelectTile();
			}
		}

	}

	private void AIControl()
	{
		if (GameMaster.instance.battleState == BattleState.Peace)
		{
			FollowPlayerChar();
		}
		else
		{
			/*if (currentTile == GameMaster.instance.currPlayerChar.currentTile) //do something
			{
				Tile prevTile = GameMaster.instance.prevCharPosition[0];
				LerpTo(prevTile.TileToWorldCoord());
				currentTile = prevTile;
				UpdatePrevPosition();
			}

			AIDecision();
		}

	}

	private void AIDecision()
	{	
		//first try to attack
		GeneratePossibleAttackTiles(AttackType.Default);

		PriorityQueue<EnemyComponent> enemyInRange = new PriorityQueue<EnemyComponent>();

		foreach (EnemyComponent en in GameMaster.instance.involvedInCombat)
		{
			if (CheckAttackWithinRange(en.currentTile))
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

			if (TryAttackInsideRange(currEnemy.currentTile))
			{
				InvokeTurnEnd();
				return;
			}
				
		}

		//failed to attack
		if (nearestEnemy != null && TryMoveInsideRange(nearestEnemy.currentTile, CollisionCheckEnum.All))
		{
			//char chase nearest enemy
			InvokeTurnEnd();
		}
		else
			FollowPlayerChar();

	}
	
	private bool FollowPlayerChar(bool strictFollowPlayer = false) //only available when it is not a player
	{
		Tile prevTile = GameMaster.instance.prevCharPosition[charId - 1];

		if (prevTile != null && TryMoveInstantTo(prevTile, CollisionCheckEnum.All) ) //if previous char position exist
		{	//success follow player
			UpdatePrevPosition();
			InvokeTurnEnd();
			return true;
		}
		else
		{	
			print ("CHar AI Fail follow player");
			TurnEnd();
			return false;
		}
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

	protected override void Die ()
	{
		print ("Char is die");
	}

	public void HighlightPossibleMoveTile()
	{
		HighlightManager.instance.ShowHighlight(possibleMoveTiles.Keys.ToList()); //show highlight
	}

	public void HighlightPossibleAttackTile()
	{
		HighlightManager.instance.ShowHighlight(possibleAttackTiles);
	}
	
}*/
