using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class Pathfinding 
{
	/// <summary>
	/// A star pathfinding to determine shortest path from start to goal
	/// </summary>
	public static List<Tile> AStarPathfinding(Tile startTile, Tile goal, CollisionCheckEnum colCheck)
	{
		//info of where this tile comes from other tile
		Dictionary<Tile, Tile> cameFrom = new Dictionary<Tile, Tile>();
		//info of total cost in certain tile
		Dictionary<Tile, float> totalCost = new Dictionary<Tile, float>();
		
		//list all the target tile that will be explored
		PriorityQueue<Tile> targets = new PriorityQueue<Tile>();
		
		targets.Enqueue(0, startTile);
		
		cameFrom.Add(startTile, null);
		totalCost.Add(startTile, 0);
		
		while (targets.Count > 0)
		{
			Tile current = targets.Dequeue(); //get the best tile
			
			if (current == goal)
			{
				break; //if reaches goal already
			}
			
			foreach (Tile neighbour in current.neighbours)
			{
				if (GameMaster.instance.IsTileOccupied(neighbour, colCheck)) //if next tile is occupied, skip
					continue;

				//add the total cost with next tile cost
				float newCost = totalCost[current] + neighbour.cost;
				//if neighbour is diagonal to current, add a little cost so they prefer straight line rather than diagonal
				if (current.DiagonalTo(neighbour))
				{
					newCost += 0.01f;
				}
				//if the neighbour is unexplored OR(then) if the new cost is cheaper than other path
				if (!totalCost.ContainsKey(neighbour) || newCost < totalCost[neighbour])
				{	//replace it
					totalCost[neighbour] = newCost;
					//priority based on cost of movement and total distance
					// !!!!!
					// 2016-10-09 I think this is wrong, there is no distance to goal counted...
					float priority = newCost + current.DistanceToTile(neighbour);
					//register the next tile based on the priority
					targets.Enqueue(priority, neighbour);
					
					//register current tile as previous tile before next tile
					if (cameFrom.ContainsKey(neighbour))
					{
						cameFrom[neighbour] = current;
					}
					else
					{
						cameFrom.Add(neighbour, current);
					}
				}
			}
		}
		
		List<Tile> results = new List<Tile>();
		Tile curr = goal;
		//return results until start point
		while(curr != null)
		{
			results.Add (curr); //add current tile
			
			if (cameFrom.ContainsKey(curr))
				curr = cameFrom[curr]; //next tile is previous tile
			else
				break;
		}
		results.Reverse(); //reverse it
		return results;
	}

	/// <summary>
	/// Dijkstras pathfinding for searching area from start tile within maxSteps, including where coming
	/// </summary>
	public static Dictionary<Tile, Tile> DijkstraPathfinding(Tile startTile, int maxCost, CollisionCheckEnum colCheck)
	{
		//info of where this tile comes from other tile
		Dictionary<Tile, Tile> cameFrom = new Dictionary<Tile, Tile>();
		//info of total cost in certain tile
		Dictionary<Tile, float> totalCost = new Dictionary<Tile, float>();
		
		//list all the target tile that will be explored
		PriorityQueue<Tile> targets = new PriorityQueue<Tile>();

		targets.Enqueue(0, startTile);

		totalCost.Add(startTile, 0);

		while (targets.Count > 0)
		{
			Tile current = targets.Dequeue(); //get the best tile
			
			foreach (Tile neighbour in current.neighbours)
			{
				if (GameMaster.instance.IsTileOccupied(neighbour, colCheck))
					continue;

				//add the total cost with next tile cost
				float newCost = totalCost[current] + neighbour.cost;
				//if the newCost is greater than maxCost, skip it
				if (newCost > maxCost )
					continue;

				//if the neighbour is unexplored OR(then) if the new cost is cheaper than other path
				if (!totalCost.ContainsKey(neighbour) || newCost < totalCost[neighbour])
				{	//replace it
					totalCost[neighbour] = newCost;
					//priority based on cost of movement and total distance
					float priority = newCost + current.DistanceToTile(neighbour);
					//register the next tile based on the priority
					targets.Enqueue(priority, neighbour);

					//register current tile as previous tile before next tile
					if (cameFrom.ContainsKey(neighbour))
					{
						cameFrom[neighbour] = current;
					}
					else
					{
						cameFrom.Add(neighbour, current);
					}

				}
			}
		}

		return cameFrom;
	}

	/// <summary>
	/// Dijkstras search area for searching area from start tile within maxSteps, NO cameFrom resulted
	/// </summary>
	public static List<Tile> DijkstraSearchTileRange(Tile startTile, int maxCost) //comment: it doesnt really need priorityqueue actually, could use normal list
	{
		//info of total cost in certain tile
		Dictionary<Tile, float> totalCost = new Dictionary<Tile, float>();
		
		//list all the target tile that will be explored
		PriorityQueue<Tile> targets = new PriorityQueue<Tile>();
		
		targets.Enqueue(0, startTile);
		
		totalCost.Add(startTile, 0);
		
		while (targets.Count > 0)
		{
			Tile current = targets.Dequeue(); //get the best tile
			
			foreach (Tile neighbour in current.neighbours)
			{
				//add the total cost with next tile cost
				float newCost = totalCost[current] + neighbour.cost;
				//if the newCost is greater than maxCost, skip it
				if (newCost > maxCost )
					continue;
				
				//if the neighbour is unexplored OR(then) if the new cost is cheaper than other path
				if (!totalCost.ContainsKey(neighbour) || newCost < totalCost[neighbour])
				{	//replace it
					totalCost[neighbour] = newCost;
					//priority based on cost of movement and total distance
					float priority = newCost + current.DistanceToTile(neighbour);
					//register the next tile based on the priority
					targets.Enqueue(priority, neighbour);
					
				}
			}
		}
		
		List<Tile> results = new List<Tile>(totalCost.Keys);
		return results;
	}

	/// <summary>
	/// Gets the path from dictionary keys from provided goal to start point, index 0 of list is own position
	/// </summary>
	public static List<Tile> GetPathFromDict(Dictionary<Tile, Tile> dict, Tile goal)
	{
		List<Tile> results = new List<Tile>();
		Tile curr = goal;
		//return results until start point
		while(curr != null)
		{
			results.Add (curr); //add current tile
			
			if (dict.ContainsKey(curr))
				curr = dict[curr]; //next tile is previous tile
			else
				break;
		}
		results.Reverse(); //reverse it
		return results;
	}
	
}
