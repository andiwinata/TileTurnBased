using UnityEngine;
using System.Collections.Generic;

public class TilePrefab : MonoBehaviour 
{
	void OnMouseDown()
	{
		/*if (GameMaster.instance.gameState == GameState.PlayerMoving)
			GameMaster.instance.MoveUnitTo(transform.position);*/

		/*Vector2 a = transform.position.WorldToTileCoord();
		List<Tile> n = Map.tileData[a.IntX()][a.IntY()].neighbours;

		foreach (Tile x in n)
		{
			Instantiate (gameObject, x.TileToWorldCoord() + Vector3.up, Quaternion.identity);
		}*/
	}

}
