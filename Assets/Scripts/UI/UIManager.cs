using UnityEngine;
using System.Collections;

public enum PanelEnum
{
	PlayerTurn = 0,

}

public class UIManager : MonoBehaviour 
{
	private static UIManager _instance;
	public static UIManager instance
	{
		get
		{
			if (_instance == null)
				_instance = GameObject.FindObjectOfType<UIManager>();
			return _instance;
		}
	}

	[SerializeField]
	private GameObject[] panels;

	void Awake()
	{
		DeactivateAllPanels();
	}

	public void ActivatePanel(PanelEnum pEnum)
	{
		panels[(int) pEnum].SetActive(true);
	}

	public void DeactivateAllPanels()
	{
		foreach (GameObject go in panels)
		{
			go.SetActive(false);
		}
	}
}
