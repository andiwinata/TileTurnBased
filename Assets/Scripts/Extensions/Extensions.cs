using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Extensions
{
	//-------LIST<TILE> EXTENSTIONS
	public static void AddPassable(this List<Tile> tiles, Tile tile)
	{
		if (tile.isPassable)
			tiles.Add(tile);
	}

	//capture all list to contain all rejected tile
	public static void AddPassable(this List<Tile> tiles, Tile tile, List<Tile> captureAll)
	{
		if (tile.isPassable)
			tiles.Add(tile);

		captureAll.Add (tile);
	}

	//-------TILE EXTENSIONS
	public static float DistanceToTile(this Tile a, Tile b)
	{
		return Mathf.Abs(a.xPos - b.xPos) + Mathf.Abs(a.yPos - b.yPos);
	}

	public static bool DiagonalTo(this Tile a, Tile b)
	{
		return !(a.xPos == b.xPos || a.yPos == b.yPos);
	}

	//convert tile coord to world coord
	public static Vector3 TileToWorldCoord(this Tile tile, float tileSize = 1)
	{
		return new Vector3( tile.xPos * tileSize, 0, tile.yPos * tileSize);
	}

	public static Vector2 TileToCoord(this Tile tile) //convert tile to tile coordinate
	{
		return new Vector2(tile.xPos, tile.yPos);
	}

	//--------ROOM EXTENSIONS
	public static float DistanceToRoom(this Room r1, Room r2)
	{
		return Mathf.Abs(r1.center.IntX() - r2.center.IntX()) + Mathf.Abs(r1.center.IntY() - r2.center.IntY());
	}
	
	//--------VECTOR3 EXTENSIONS
	//convert world to tile
	public static Vector2 WorldToTileCoord(this Vector3 pos, float tileSize = 1)
	{
		int xTilePos = Mathf.FloorToInt(pos.x / tileSize);
		int yTilePos = Mathf.FloorToInt(pos.z / tileSize);
		
		return new Vector2 (xTilePos, yTilePos);
	}

	public static Tile WorldToTile(this Vector3 pos, float tileSize = 1)
	{
		Vector2 p = pos.WorldToTileCoord(tileSize);
		return Map.tileData[p.IntX()][p.IntY()];
	}

	//return int instead of float
	public static int IntX(this Vector2 vec)
	{
		return Mathf.FloorToInt(vec.x);
	}

	public static int IntY(this Vector2 vec)
	{
		return Mathf.FloorToInt(vec.y);
	}

	public static Tile CoordToTile(this Vector2 vec)
	{
		return Map.tileData[vec.IntX()][vec.IntY()];
	}

	//Custom
	public static bool GetChance(int x)
	{
		return x > Random.Range(0, 100);
	}

	public static string GetStringValue(this TargetTypeEnum tte)
	{
		switch (tte)
		{
		case TargetTypeEnum.Character:
			return "Character";
			
		case TargetTypeEnum.Enemy:
			return "Enemy";
		}

		return null;
	}
}
