using UnityEngine;
using System.Collections;

public class LogUpdateState : MonoBehaviour
{
	public string message;

	void Update ()
	{
		Debug.Log ( message );
	}
}
