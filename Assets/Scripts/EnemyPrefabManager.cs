using UnityEngine;
using System.Collections;

public class EnemyPrefabManager : MonoBehaviour 
{
	private static EnemyPrefabManager _instance;
	public static EnemyPrefabManager instance
	{
		get
		{
			if (_instance == null)
				_instance = GameObject.FindObjectOfType<EnemyPrefabManager>();
			return _instance;
		}
	}
	
	public GameObject[] enemyPrefabs = new GameObject[3]; //need to adjust to enum
	
	public GameObject RetrieveEnemyPrefab(EnemyTypeEnum enemyType)
	{
		GameObject go = Instantiate(enemyPrefabs[(int) enemyType]) as GameObject;
		return go;
	}
}
