using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HighlightManager : MonoBehaviour 
{
	private static HighlightManager _instance;
	public static HighlightManager instance
	{
		get
		{
			if (_instance == null)
				_instance = GameObject.FindObjectOfType<HighlightManager>();
			return _instance;
		}
	}

	public GameObject highlightPrefab;

	private List<GameObject> highlightPool = new List<GameObject>();
	private const int totalPooled = 20;
	
	void Awake () 
	{
		for (int i = 0; i < totalPooled; i++)
		{
			GameObject go = Instantiate(highlightPrefab) as GameObject;
			go.SetActive(false);
			go.transform.parent = transform;
			highlightPool.Add (go);
		}
	}

	public void ShowHighlight (List<Tile> tiles)
	{
		ClearHighlight();

		foreach (Tile tile in tiles)
		{
			GameObject hl = GetHighlightPrefab();
			hl.transform.position = tile.TileToWorldCoord() + (Vector3.up * 0.51f);
			hl.SetActive(true);
		}
	}

	public void ClearHighlight()
	{
		foreach (GameObject go in highlightPool)
		{
			go.SetActive(false);
		}
	}

	private GameObject GetHighlightPrefab()
	{
		foreach (GameObject go in highlightPool)
		{
			if (!go.activeInHierarchy)
			{
				return go;
			}
		}

		GameObject g = Instantiate(highlightPrefab) as GameObject;
		g.SetActive(false);
		g.transform.parent = transform;
		highlightPool.Add(g);
		return g;
	}
}
