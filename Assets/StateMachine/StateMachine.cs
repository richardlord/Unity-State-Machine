using UnityEngine;
using System;
using System.Collections.Generic;

public class StateMachine : MonoBehaviour
{
	public string firstState;
	public StateConfig[] states;

	private MonoBehaviour _currentState;
	private string _currentStateName;

	private Dictionary<string, StateData> statesDictionary;

	public string currentState
	{
		get
		{
			return _currentStateName;
		}
	}

	void Awake ()
	{
		statesDictionary = new Dictionary<string, StateData>( states.Length );
		foreach( StateConfig state in states )
		{
			statesDictionary[ state.name ] = new StateData( state.component.GetType(), JsonUtility.ToJson( state.component ) );
			DestroyImmediate( state.component );
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
				statesDictionary[ _currentStateName ] = new StateData( _currentState.GetType(), JsonUtility.ToJson( _currentState ) );
				DestroyImmediate( _currentState );
			}
				
			StateData state = statesDictionary[ stateName ];
			_currentStateName = stateName;
			_currentState = gameObject.AddComponent( state.type ) as MonoBehaviour;
			JsonUtility.FromJsonOverwrite( state.data, _currentState );
		}
	}

	[Serializable]
	public struct StateConfig
	{
		public string name;
		public MonoBehaviour component;
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
