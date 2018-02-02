using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum ActionShapeEnum
{
	Rectangle
}

public class TileRange : MonoBehaviour 
{
	private static TileRange _instance;
	public static TileRange instance
	{
		get
		{
			if (_instance == null)
				_instance = GameObject.FindObjectOfType<TileRange>();
			return _instance;
		}
	}

	public Dictionary<Tile, Tile> SetMovementTileRange(MovementType moveType, Tile center, CollisionCheckEnum colCheck)
	{
		ActionData ad = ActionDatabase.GetMovementData(moveType);
		Dictionary<Tile, Tile> rangeTile = new Dictionary<Tile, Tile>();

		int maxCost = ad.maxDist / 2; //should be square so maxCost will always same
		rangeTile = Pathfinding.DijkstraPathfinding(center, maxCost, colCheck);

		return rangeTile;

	}

	public List<Tile> SetAttackTileRange(AttackType attType, Tile center) //attack ignore collision and only check list not dictionary
	{
		AttackTypeData ad = ActionDatabase.GetAttackData(attType);
		List<Tile> rangeTile = new List<Tile>();

		int maxCost = ad.maxDist / 2; 
		rangeTile = Pathfinding.DijkstraPathfinding(center, maxCost, CollisionCheckEnum.None).Keys.ToList();
		
		return rangeTile;
		
	}

}
