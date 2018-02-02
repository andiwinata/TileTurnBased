using UnityEngine;
using System.Collections.Generic;

public class Tile 
{
	//tile type
	public TileEnum tileEnum { get; private set;}
	
	//position
	public int xPos { get; private set;}
	public int yPos { get; private set;}

	//cost to go to this tile
	public float cost { get; private set;}

	//is passable or not
	public bool isPassable { get; private set;}

	//can only be set once
	//passable neighbours
	public List<Tile> neighbours { get; private set;}
	//ALL neighbours including walls and void
	public List<Tile> allNeighbours { get; private set;}

	public Tile(int x, int y, TileEnum te = TileEnum.Wall)
	{	//assign data from constructor
		xPos = x;
		yPos = y;

		UpdateTileData(te);
	}

	public void UpdateTileData(TileEnum te)
	{
		tileEnum = te;

		switch (tileEnum)
		{
		case TileEnum.Void:
			isPassable = false;
			cost = Mathf.Infinity;
			break;

		case TileEnum.Grass:
			isPassable = true;
			cost = 1;
			break;

		case TileEnum.Water:
			isPassable = true;
			cost = 1;
			break;

		case TileEnum.Wall:
			isPassable = false;
			cost = Mathf.Infinity;
			break;
		}
	}

	public void SetNeighbours(List<Tile> passableNeigh, List<Tile> allNeigh)
	{
		if (neighbours == null)
			neighbours = passableNeigh;

		if (allNeighbours == null)
			allNeighbours = allNeigh;
	}

	public void PrintPosition()
	{
		Debug.Log("x: " + xPos + " y: " + yPos);
	}

}
