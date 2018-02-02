using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInput : MonoBehaviour 
{
	enum PlayerState
	{
		None,
		//PlayerTurnBegin,
		PlayerSelectMoveTile,
		PlayerSelectOtherChar,
		PlayerMoving,
		PlayerSelectAttack,
		PlayerAttackDone,

	}

	private static PlayerInput _instance;
	public static PlayerInput instance
	{
		get
		{
			if (_instance == null)
				_instance = GameObject.FindObjectOfType<PlayerInput>();
			return _instance;
		}
	}

	public Camera charCam;
	[HideInInspector]
	private PlayerState playerState;

	public CharacterComponent currChar {get; private set;}

	public void SetCurrChar(CharacterComponent charac)
	{
		currChar = charac;
	}

	//Get Tile
	public void PlayerSelectTile()
	{
		playerState = PlayerState.PlayerSelectMoveTile;
		currChar.HighlightMoveRange();
	}

	public void PlayerSelectAttack(AttackType attType)
	{
		currChar.PlayerChooseAttack();
		playerState = PlayerState.PlayerSelectAttack;
		currChar.HighlightAttackRange();
	}

	//Get Character
	public void PlayerSelectOtherChar()
	{
		playerState = PlayerState.PlayerSelectOtherChar;
		
		List<Tile> charPositions = new List<Tile>();
		foreach (CharacterComponent ch in GameMaster.instance.charsList)
		{
			if (!ch.Equals(currChar))
				charPositions.Add(ch.currentTile);
		}
		HighlightManager.instance.ShowHighlight(charPositions);
		
	}

	public void PlayerTurnEnd()
	{
		playerState = PlayerState.None;
	}

	void Update() //everything in here is when controlling character is player
	{
		GameObject obj;
		
		//during player select tile
		if (playerState == PlayerState.PlayerSelectMoveTile)
		{
			Tile target = null;

			#region Input
			if (Input.GetAxisRaw("Horizontal") > 0)	//move to right tile
				target = Map.tileData[currChar.currentTile.xPos + 1][currChar.currentTile.yPos];

			if (Input.GetAxisRaw("Horizontal") < 0)	//move to left tile
				target = Map.tileData[currChar.currentTile.xPos - 1][currChar.currentTile.yPos];

			if (Input.GetAxisRaw("Vertical") > 0)	//move to top tile
				target = Map.tileData[currChar.currentTile.xPos][currChar.currentTile.yPos + 1];

			if (Input.GetAxisRaw("Vertical") < 0)	//move to bottom tile
				target = Map.tileData[currChar.currentTile.xPos][currChar.currentTile.yPos - 1];

			if (Input.GetMouseButtonDown(0))
			{
				obj = GetTagObj("Tile");
				if (obj != null)
					target = obj.transform.position.WorldToTile();
			}
			#endregion

			if (target != null && currChar.PlayerReceiveMoveTile(target))
			{
				playerState = PlayerState.PlayerMoving;
			}
		}
		
		//during swap leader
		if (playerState == PlayerState.PlayerSelectOtherChar && Input.GetMouseButtonDown(0))
		{
			obj = GetTagObj("Character");
			//check it the character is valid, and not same character
			if (obj != null && GameMaster.instance.SwapPlayerChar(obj.GetComponent<CharacterComponent>()) )
			{
				playerState = PlayerState.PlayerSelectMoveTile;
			}
		}

		//during attack
		if (playerState == PlayerState.PlayerSelectAttack && Input.GetMouseButtonDown(0))
		{
			obj = GetTagObj("Tile");
			
			//check if it is tile and valid altogether move it
			if (obj != null && currChar.PlayerReceiveAttackTile(obj.transform.position.WorldToTile()) )
			{
				//succeed attack
				playerState = PlayerState.PlayerAttackDone;
			}
		}
		
	}
	
	private GameObject GetTagObj(string tag) //get object with tag from mouse click
	{
		Ray ray = charCam.ScreenPointToRay(Input.mousePosition);
		//raycast from camera when mouse hit
		RaycastHit[] rayHits = Physics.RaycastAll(ray, 20000);
		
		foreach (RaycastHit rh in rayHits)
		{
			if (rh.collider.CompareTag(tag))
				return rh.collider.gameObject;
		}
		return null;
		
	}

}
