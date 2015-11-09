using UnityEngine;
using System.Collections;

public class StateSwitcher : MonoBehaviour
{
	public StateMachine stateMachine;

	void Update ()
	{
		if( Input.GetMouseButtonDown( 0 ) )
		{
			if( stateMachine.currentState == "Hello" )
			{
				stateMachine.ChangeState( "Update" );
			}
			else if( stateMachine.currentState == "Update" )
			{
				stateMachine.ChangeState( "Both" );
			}
			else
			{
				stateMachine.ChangeState( "Hello" );
			}
		}
	}
}
