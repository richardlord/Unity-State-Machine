using UnityEngine;
using System.Collections;

public class StateSwitcher : MonoBehaviour
{
	public StateMachine stateMachine;

	void Update ()
	{
		if( Input.GetMouseButtonDown( 0 ) )
		{
			stateMachine.ChangeState( "LogWaiting" );
		}
	}
}
