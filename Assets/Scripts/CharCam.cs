using UnityEngine;
using System.Collections;

public class CharCam : MonoBehaviour 
{
	private static CharCam _instance;
	public static CharCam instance
	{
		get
		{
			if (_instance == null)
				_instance = GameObject.FindObjectOfType<CharCam>();
			return _instance;
		}
	}

	public CharacterComponent currentFocus {get; private set;}

	private Camera cam;

	private const float movingDuration = 0.5f;

	void Awake()
	{
		cam = GetComponent<Camera>();
	}

	public void FocusCamTo(CharacterComponent charac, Transform newTrans)
	{
		if (charac.Equals(currentFocus)) //if already focuses on this char, ignore
			return;

		currentFocus = charac;
		//parent cam to new char
		cam.transform.parent = charac.transform;

		//move cam to new char
		StartCoroutine(LerpTo(newTrans));
	}

	private IEnumerator LerpTo(Transform newTrans)
	{
		Vector3 initPos = transform.position;
		Quaternion initRot = transform.rotation;

		float time = 0;
		float rate = 1 / movingDuration;

		while (time < 1)
		{
			time += rate * Time.deltaTime;
			transform.position = Vector3.Lerp(initPos, newTrans.position, time);
			transform.rotation = Quaternion.Lerp(initRot, newTrans.rotation, time);
			yield return null;
		}
	}
}
