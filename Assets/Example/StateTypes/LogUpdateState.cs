using UnityEngine;
using System.Collections;

public class LogUpdateState : MonoBehaviour
{
	public string message;
	public int count = 0;

	void Start()
	{
		++count;
	}

	void Update ()
	{
		Debug.Log ( message + " : " + count );
	}
}
