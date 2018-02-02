using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum BattleState
{
	Peace,
	Battle
}

public enum CollisionCheckEnum
{
	Character,
	Enemy,
	All,
	None
}

public class GameMaster : MonoBehaviour 
{
	private static GameMaster _instance;
	public static GameMaster instance
	{
		get
		{
			if (_instance == null)
				_instance = GameObject.FindObjectOfType<GameMaster>();
			return _instance;
		}
	}
	
	public CharacterComponent charPrefab;
	public EnemyComponent enemyPrefab;
	public GameObject playerArea;

	public PriorityQueue<UnitComponent> turnOrders {get; private set;}

	[HideInInspector]
	public BattleState battleState = BattleState.Peace;
	
	public List<CharacterComponent> charsList {get; private set;}
	public List<EnemyComponent> enemiesList {get; private set;}
	public HashSet<EnemyComponent> involvedInCombat {get; private set;}

	public Tile[] prevCharPosition {get; set;}

	public CharacterComponent currPlayerChar {get; private set;}

	public UnitComponent currUnit {get; private set;}

	void Awake()
	{
		involvedInCombat = new HashSet<EnemyComponent>();
		turnOrders = new PriorityQueue<UnitComponent>();
		SpawnCharacter();
		SpawnEnemies();

		//Select first character as leader
		SetPlayerChar(charsList[0]);
		NextTurn();
	}

	public void NextTurn()
	{
		//let next unit do its turn
		currUnit = turnOrders.Dequeue();

		if (currUnit == currPlayerChar)
			currPlayerChar.SetToPlayer();

		currUnit.MyTurn(); //default all can move and attack

		//reduce all queue
		turnOrders.ReduceAllPriorityBy(1);
	}

	//SPAWN CHARS
	private void SpawnCharacter()
	{
		charsList = new List<CharacterComponent>();
		for (int i = 0; i < 3; i++)
		{
			CharacterComponent c = Instantiate(charPrefab) as CharacterComponent;

			c.charType = (CharacterTypeEnum) i; //set the char type to determine prefab
			c.TeleportTo( Map.rooms[0].GetRandomContentTile() );//get random position in room 0 and place character
			charsList.Add (c);
			turnOrders.Enqueue(c.queuePoint, c);
			c.charId = i; //set player id
		}

		//register total of previous positions data based on total char (for follow player)
		prevCharPosition = new Tile[charsList.Count]; 
	}

	//SPAWN ENEMIES
	private void SpawnEnemies()
	{
		enemiesList = new List<EnemyComponent>();
		for (int j = 0; j < 5; j++)
		{
			EnemyComponent e = Instantiate(enemyPrefab) as EnemyComponent;

			e.enemyType = (EnemyTypeEnum) (j % 3);
			e.TeleportTo( Map.rooms[Random.Range(0, Map.rooms.Count)].GetRandomContentTile());
			enemiesList.Add (e);
			e.enabled = false;
		}
	}
	
	public void SetPlayerChar(CharacterComponent charac)
	{
		currPlayerChar = charac;
		currPlayerChar.SetToPlayer();
		
		playerArea.transform.position = currPlayerChar.transform.position;
		playerArea.transform.parent = currPlayerChar.transform;
	}
	
	public bool SwapPlayerChar(CharacterComponent charac)
	{
		if (charac.Equals(currPlayerChar))
			return false;

		//remove highlight
		HighlightManager.instance.ClearHighlight();

		//swap currChar and charac position in turnOrder 
		//(since curr is already dequeued, then remove the charac from the order and get the new one)
		turnOrders.ReplaceItem(charac, currPlayerChar);

		//swap charId
		int tempId = charac.charId;
		charac.charId = currPlayerChar.charId;
		currPlayerChar.charId = tempId;

		//replace currChar isPlayer to new one
		currPlayerChar.RevokePlayer();
		SetPlayerChar(charac);

		//replace currUnit to this char
		currUnit = charac;
		//change to new charac turn
		currUnit.MyTurn();
		return true;
	}

	public bool IsTileOccupied(Tile tile, CollisionCheckEnum cce)
	{
		if (cce == CollisionCheckEnum.Character || cce == CollisionCheckEnum.All)
		{
			foreach (CharacterComponent ch in charsList)
			{
				if (tile.Equals(ch.currentTile))
					return true;
			}
		}

		if (cce == CollisionCheckEnum.Enemy || cce == CollisionCheckEnum.All)
		{
			foreach (EnemyComponent en in enemiesList)
			{
				if (tile.Equals(en.currentTile))
					return true;
			}
		}

		return false;
	}

	public UnitComponent GetUnitAt(Tile tile)
	{
		foreach (CharacterComponent ch in charsList)
		{
			if (tile.Equals(ch.currentTile))
				return ch;
		}

		foreach (EnemyComponent en in enemiesList)
		{
			if (tile.Equals(en.currentTile))
				return en;
		}

		return null;
	}
	
	public void RemoveEnemy(EnemyComponent en)
	{
		enemiesList.Remove(en);
	}

	public void EnterCombat(EnemyComponent enemy)
	{
		involvedInCombat.Add(enemy);
		battleState = BattleState.Battle;
	}

	public void ExitCombat(EnemyComponent enemy)
	{
		involvedInCombat.Remove(enemy);
		print ("combat: " + involvedInCombat.Count);
		if (involvedInCombat.Count == 0)
		{
			battleState = BattleState.Peace;
		}
			
	}
}
