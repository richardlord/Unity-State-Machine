using UnityEngine;
using System;
using System.Collections.Generic;

public class StateMachine : MonoBehaviour
{
	public State[] states;

	public string firstState;

	private State _currentState;

	private Dictionary<string, StateData> statesDictionary;

	public string currentState
	{
		get
		{
			return _currentState.stateName;
		}
	}

	void Start ()
	{
		statesDictionary = new Dictionary<string, StateData>( states.Length );
		foreach( State state in states )
		{
			statesDictionary[ state.stateName ] = new StateData( state.GetType(), JsonUtility.ToJson( state ) );
			DestroyImmediate( state );
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
			if( _currentState != null )
			{
				_currentState.Exit();
				statesDictionary[ _currentState.stateName ] = new StateData( _currentState.GetType(), JsonUtility.ToJson( _currentState ) );
				DestroyImmediate( _currentState );
			}
				
			StateData state = statesDictionary[ stateName ];
			_currentState = gameObject.AddComponent( state.type ) as State;
			JsonUtility.FromJsonOverwrite( state.data, _currentState );
			_currentState.Enter();
		}
	}

	class StateData
	{
		public string data;
		public Type type;

		public StateData( Type type, string data )
		{
			this.type = type;
			this.data = data;
		}

	}
}
