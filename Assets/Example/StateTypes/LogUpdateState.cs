using UnityEngine;
using System.Collections;

public class LogUpdateState : State
{
	public string message;

	void Update ()
	{
		Debug.Log ( message );
	}
}
