using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class MovementComponent : MonoBehaviour 
{
	//components
	private UnitComponent thisUnit;

	//data
	public Dictionary<Tile, Tile> movementRange {get; private set;}

	private List<Tile> path;
	private int pathIndex;

	private float movingTime = 0.1f;

	void Awake()
	{
		thisUnit = GetComponent<UnitComponent>();
	}

	public bool CheckTargetInsideRange(Tile target)
	{
		return movementRange.ContainsKey(target);
	}

	public void GenerateMovementRange(MovementType at, CollisionCheckEnum colCheck)
	{
		movementRange = TileRange.instance.SetMovementTileRange(at, thisUnit.currentTile, colCheck);
	}
	
	public bool TryMakePathInsideRange(Tile goal) 
	{	
		if (CheckTargetInsideRange(goal)) 
		{
			//success making path
			path = Pathfinding.GetPathFromDict(movementRange, goal); //convert range (dict) to list
			pathIndex = 0;
			return true;
		}
		else
		{
			//fail to make path
			path = null;
			pathIndex = 0;
			return false;
		}
		
	}
	
	public bool TryMoveInPath(CollisionCheckEnum colCheck) //try to move in previous path
	{
		if (path == null)
			return false;
		
		pathIndex++;
		if (pathIndex >= path.Count) //if stepsId larger than list, it means previous path has done
		{
			path = null;
			return false;
		}
		
		Tile nextInPath = path[pathIndex];
		if (!GameMaster.instance.IsTileOccupied(nextInPath, colCheck)) //if not occupied, move
		{
			StartCoroutine(MovingDirectlyTo(nextInPath));
			return true;
		}
		else
		{
			return false;
		}
		
	}
	
	public bool TryMoveDirectlyTo(Tile target, CollisionCheckEnum colCheck)
	{
		if (!GameMaster.instance.IsTileOccupied(target, colCheck))
		{
			StartCoroutine(MovingDirectlyTo(target));
			return true;
		}
		else
		{
			return false;
		}
	}
	
	public bool TryMoveInsideRange(Tile goal, CollisionCheckEnum colCheck) //shortcut for creating path and move in one call
	{
		if (TryMakePathInsideRange(goal) && TryMoveInPath(colCheck)) //if path successfully created and successfully moved
		{
			return true;
		}
		else
			return false;
	}
	
	public IEnumerator MovingDirectlyTo(Tile target)
	{
		Vector3 end = target.TileToWorldCoord();
		Debug.DrawLine(transform.position + Vector3.up, end + Vector3.up , Color.red, 10f);
		
		if (end != transform.position)
			yield return StartCoroutine(LerpTo(end));
		
		thisUnit.ChangeCurrentTile(target);
	}	
	
	private IEnumerator LerpTo(Vector3 target)
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
}
