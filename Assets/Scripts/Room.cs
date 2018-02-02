using UnityEngine;
using System.Collections.Generic;

public class Room 
{
	public int left { get; private set;}	
	public int top { get; private set;}	
	public int width { get; private set;}	
	public int height { get; private set;}	
	public bool isConnected {get; set;}
	public int totalCorridor {get; set;}

	//list of tile which is which
	public List<Tile> borders {get; private set;}
	public List<Tile> contents {get; private set;}

	public int right 
	{
		get {return left + width - 1;}
	}
	public int bottom
	{
		get {return top - height + 1;}
	}

	public Vector2 center
	{
		get 
		{
			return new Vector2 ( Mathf.Floor(left + right) / 2, Mathf.Floor(top + bottom) / 2);
		}
	}

	public Room(int l, int t, int w, int h, bool connect = false)
	{
		left = l;
		top = t;
		width = w;
		height = h;
		isConnected = connect;

		DetermineRoomTiles();
	}

	public bool Contains(Room r)
	{	
		return right >= r.left && 
				left <= r.right &&
				top >= r.bottom &&
		        bottom <= r.top;
	}

	private void DetermineRoomTiles()
	{
		borders = new List<Tile>();
		contents = new List<Tile>();

		for (int i = left; i < right + 1; i++)
		{
			for (int j = bottom; j < top + 1; j++)
			{
				if (i == left || i == right || j == bottom || j == top)
				{
					borders.Add (new Vector2(i, j).CoordToTile() );
				}
				else
				{
					contents.Add ( new Vector2(i, j).CoordToTile() );
				}
			}
		}
	}

	public Tile GetRandomBorderTile()
	{
		return borders[Random.Range(0, borders.Count)];
	}

	public Tile GetRandomContentTile()
	{
		return contents[Random.Range(0, contents.Count)];
	}

	public void PrintCoordinate()
	{
		Debug.Log ("l: " + left + " r: " + right +
		           " t: " + top + " b: " + bottom);
	}
	
}
