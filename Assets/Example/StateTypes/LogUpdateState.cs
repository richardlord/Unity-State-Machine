using UnityEngine;
using System.Collections;

public class LogUpdateState : State
{
	public string message;

	public override void Update ()
	{
		Debug.Log ( message );
	}
}
