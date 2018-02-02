using UnityEngine;
using System.Collections;

public class PlayerTurnPanel : MonoBehaviour 
{
	public void MoveButtonPressed()	
	{
		//Player chooses move
		PlayerInput.instance.PlayerSelectTile();
		print ("player chose move");
	}

	public void AttackButtonPressed()	
	{
		//Player chooses attack (basic attack)
		print ("player chose attack");
		PlayerInput.instance.PlayerSelectAttack(AttackType.Default);

	}

	public void SwapLeaderPressed()
	{
		//Player chooses swap
		PlayerInput.instance.PlayerSelectOtherChar();
		print ("player chose swap char");
	}
}
