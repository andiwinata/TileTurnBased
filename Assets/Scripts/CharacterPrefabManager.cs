using UnityEngine;
using System.Collections;

public class CharacterPrefabManager : MonoBehaviour 
{
	private static CharacterPrefabManager _instance;
	public static CharacterPrefabManager instance
	{
		get
		{
			if (_instance == null)
				_instance = GameObject.FindObjectOfType<CharacterPrefabManager>();
			return _instance;
		}
	}

	public GameObject[] charPrefabs = new GameObject[3]; //need to adjust to enum

	public GameObject RetrieveCharacterPrefab(CharacterTypeEnum charType)
	{
		GameObject go = Instantiate(charPrefabs[(int) charType]) as GameObject;
		return go;
	}
}
