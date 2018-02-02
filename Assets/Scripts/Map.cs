using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum TileEnum
{
	Void = 0,
	Grass = 1,
	Water = 2,
	Wall = 3
}

public class Map : MonoBehaviour 
{
	//prefabs type
	public GameObject[] tilePrefabs;

	//map size and tile size
	public const int xSize = 50;
	public const int ySize = 50;
	public const int totalTile = xSize * ySize;
	public const float tileSize = 1;

	//data of tiles in the map
	public static Tile[][] tileData {get; private set;}

	//rooms data
	public static List<Room> rooms {get; private set;}
	private static int totalRoomAttempt = 10;
	public static int totalRoom {get; private set;}

	void Awake () 
	{
		totalRoom = 0;
		//initialize the tileData ARRAY
		tileData = new Tile[xSize][];
		for (int i = 0; i < tileData.Length; i++)
		{
			tileData[i] = new Tile[ySize];
		}

		//Generate Tile data first
		GenerateTile();
		//Do procedural generation for room
		GenerateRooms ();

		//Generate Corridors (choose type, linear or random)
		GenerateCorridors();
		//GenerateCorridorsToNearest();

		//Generate neighbours
		GenerateNeighbours();
		//Generate visual
		GenerateVisualTile();
	}
	
	//go through all the tileData ARRAY and make Tile for every slot
	private void GenerateTile()
	{
		for (int x = 0; x < xSize; x++)
		{
			for (int y = 0; y < ySize; y++)
			{
				tileData[x][y] = new Tile(x, y, TileEnum.Void);
			}
		}
	}

	//ROOMS
	private void GenerateRooms()
	{
		rooms = new List<Room>();

		for (int i = 0; i < totalRoomAttempt; i++)
		{
			int roomWidth = Random.Range (7, 15);
			int roomHeight = Random.Range (7, 15);
			int left = Random.Range(0, xSize - roomWidth - 1);
			int	top = Random.Range (roomHeight, ySize - 1);

			Room room = new Room (left, top, roomWidth, roomHeight);
			//if not collide then make the room
			if (!RoomCollides(room))
			{
				rooms.Add(room);
				MakeRoom(room);
				totalRoom++;
			}
		}
	}

	private void GenerateCorridorsToNearest() //this will try the nearest room, there is still a chance for it to have separated groups
	{
		Dictionary <Room, Room> connections = new Dictionary<Room, Room>();

		for (int i = 0; i < rooms.Count; i++)
		{
			//isConnected determine if it has been gone through the loop, therefore no one can add again

			//if the room already has 2 corridors, skip it and set it already connected
			if (rooms[i].totalCorridor >= 2)
			{
				rooms[i].isConnected = true;
				continue;
			}

			float smallestDistance = Mathf.Infinity;
			Room nearestRoom = null;
			//find nearest room
			for (int j = 0; j < rooms.Count; j++)
			{
				if (j == i)  //if the room is ourself, continue
					continue;

				if (connections.ContainsKey(rooms[j]) && connections[ rooms[j] ] == rooms[i]) //if room the room already connected to us, skip the room
					continue;

				//if room hasnt been connected OR it is the last room (force the last room to connect if it doesnt have any connection)
				if ( !rooms[j].isConnected || (i == rooms.Count - 1 && rooms[i].totalCorridor == 0) ) 
				{	
					//and if the room distance is smaller, it is the nearest room
					if ( rooms[i].DistanceToRoom(rooms[j]) < smallestDistance)
					{
						smallestDistance = rooms[i].DistanceToRoom(rooms[j]);
						nearestRoom = rooms[j];
					}
				}
			}

			if (nearestRoom != null)
			{
				MakeCorridor(rooms[i], nearestRoom);
				rooms[i].isConnected = true;
				rooms[i].totalCorridor++;
				nearestRoom.totalCorridor++;

				connections.Add (rooms[i], nearestRoom);
			}

		}
	}

	private void GenerateCorridors()
	{
		for (int i = 0; i < rooms.Count; i++)
		{
			if (!rooms[i].isConnected) //if not connected yet
			{
				//MakeCorridor(rooms[i], rooms[ (Random.Range(0, rooms.Count) + i) % rooms.Count]); //make corridor to random room
				MakeCorridor(rooms[i], rooms[ (i + 1) % rooms.Count ]); //make corridor to next room
				rooms[i].isConnected = true;

				//rooms[ (i + 1) %rooms.Count].isConnected = true; //to mark both room connected
			}
		}
	}

	private bool RoomCollides(Room room)
	{
		for (int i = 0; i < rooms.Count; i++)
		{
			if (room.Contains(rooms[i]))
			{
				return true;
			}
		}

		return false;
	}

	private void MakeRoom(Room room)
	{
		for (int x = 0; x < room.width; x++)
		{
			for (int y = room.height - 1; y >= 0; y--)
			{
				int rX = room.left;
				int rY = room.top;
				//surrounding walls of the room
				if (x == 0 || x == room.width - 1 || y == 0 || y == room.height - 1)
					tileData[x + rX][rY - y].UpdateTileData(TileEnum.Wall);
				else //other is grass //or chance
				{
					if (Extensions.GetChance(50))
						tileData[x + rX][rY - y].UpdateTileData(TileEnum.Grass);
					else
						tileData[x + rX][rY - y].UpdateTileData(TileEnum.Water);
				}

			}
		}
	}

	private void MakeCorridor(Room r1, Room r2)
	{
		int x = r1.center.IntX();
		int y = r1.center.IntY();
		int x2 = r2.center.IntX();
		int y2 = r2.center.IntY();

		while (x != x2 || y != y2)
		{
			//move X first only if the y is not other room's top/bottom to prevent destroyed border
			if (x != x2 && y != r2.top && y != r2.bottom)
			{
				tileData[x][y].UpdateTileData(TileEnum.Grass);
				x += x < x2 ? 1 : -1;
			}
			//then Y
			else
			{
				tileData[x][y].UpdateTileData(TileEnum.Grass);
				y += y < y2? 1 : -1;
			}

			//if surrounding corridor tile is void, change it into wall
			foreach (Tile tile in SetNeighbourTile(x,y)[1])
			{
				if (tile.tileEnum == TileEnum.Void)
					tile.UpdateTileData(TileEnum.Wall);
			}
		}
	}

	//NEIGHBOUR
	private void GenerateNeighbours()
	{
		for (int x = 0; x < xSize; x++)
		{
			for (int y = 0; y < ySize; y++)
			{
				List<Tile>[] neighboursType = SetNeighbourTile(x,y);
				tileData[x][y].SetNeighbours(neighboursType[0], neighboursType[1]);
			}
		}
	
	}

	/// <summary>
	/// Sets the neighbour tile for every index.
	/// return array of neighbours. Returned index[0] for only passable tiles and [1] is for all tiles
	/// </summary>
	private List<Tile>[] SetNeighbourTile(int x, int y)
	{
		List<Tile>[] returned = new List<Tile>[2];

		List<Tile> passable = new List<Tile>();
		List<Tile> all = new List<Tile>();

		returned[0] = passable;
		returned[1] = all;
			
		//add left
		if (x > 0)
		{
			passable.AddPassable(tileData[x-1][y], all);
			//add left top
			if (y < ySize - 1)
				passable.AddPassable(tileData[x-1][y+1], all);
			//add left bottom
			if (y > 0)
				passable.AddPassable(tileData[x-1][y-1], all);
		}
		
		//add right
		if (x < xSize - 1)
		{
			passable.AddPassable(tileData[x+1][y], all);
			//add right top
			if (y < ySize - 1)
				passable.AddPassable(tileData[x+1][y+1], all);
			//add right bottom
			if (y > 0)
				passable.AddPassable(tileData[x+1][y-1], all);
		}

		//add top
		if (y < ySize - 1)
			passable.AddPassable(tileData[x][y+1], all);
		//add bottom
		if (y > 0)
			passable.AddPassable(tileData[x][y-1], all);

		return returned;
	}

	//generate visual based on the data and spawn respective prefabs
	private void GenerateVisualTile()
	{
		for (int x = 0; x < xSize; x++)
		{
			for (int y = 0; y < ySize; y++)
			{	
				TileEnum tileType = tileData[x][y].tileEnum;
				GameObject go = Instantiate (tilePrefabs[(int) tileType], TileToWorldCoord(x,y), Quaternion.identity) as GameObject;
				go.transform.parent = transform;
			}
		}

	}
		
	public static Vector3 TileToWorldCoord(int x, int y)
	{
		return new Vector3( x * tileSize, 0, y * tileSize);
	}
}
