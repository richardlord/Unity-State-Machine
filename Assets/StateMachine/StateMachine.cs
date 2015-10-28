using UnityEngine;
using System.Collections.Generic;

public class StateMachine : MonoBehaviour
{
	public State[] states;

	public State firstState;

	private State _currentState;

	private Dictionary<string, State> statesDictionary;

	public State currentState
	{
		get
		{
			return _currentState;
		}
	}

	void Start ()
	{
		statesDictionary = new Dictionary<string, State>( states.Length );
		foreach( State state in states )
		{
			statesDictionary[ state.name ] = state;
		}
		if( firstState != null )
		{
			ChangeState( firstState );
		}
	}

	public void ChangeState( string stateName )
	{
		if( statesDictionary.ContainsKey( stateName ) )
		{
			State state = statesDictionary[ stateName ];
			ChangeState( state );
		}
	}

	public void ChangeState( State state )
	{
		if( _currentState != null )
		{
			_currentState.Exit();
		}
		_currentState = state;
		_currentState.Enter();
	}

	// Pass calls through to the current state object
	
	void Update ()
	{
		if( _currentState != null )
		{
			_currentState.Update();
		}
	}
	
	void FixedUpdate ()
	{
		if( _currentState != null )
		{
			_currentState.FixedUpdate();
		}
	}

	void LateUpdate ()
	{
		if( _currentState != null )
		{
			_currentState.LateUpdate();
		}
	}
}
