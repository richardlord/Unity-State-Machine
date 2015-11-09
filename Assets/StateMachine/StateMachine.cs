using UnityEngine;
using System;
using System.Collections.Generic;

public class StateMachine : MonoBehaviour
{
	public string firstState;
	public StateConfig[] states;

	private MonoBehaviour[] _currentState;
	private string _currentStateName;

	private Dictionary<string, StateData[]> statesDictionary;

	public string currentState
	{
		get
		{
			return _currentStateName;
		}
	}

	void Awake ()
	{
		statesDictionary = new Dictionary<string, StateData[]>( states.Length );
		foreach( StateConfig state in states )
		{
			statesDictionary[ state.name ] = serializeComponents( state.components );
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
				statesDictionary[ _currentStateName ] = serializeComponents( _currentState );
			}
				
			StateData[] state = statesDictionary[ stateName ];
			_currentStateName = stateName;
			_currentState = deserializeComponents( state );
		}
	}

	private StateData[] serializeComponents( MonoBehaviour[] components )
	{
		StateData[] data = new StateData[ components.Length ];
		for( int i = 0; i < components.Length; ++i )
		{
			MonoBehaviour component = components[i];
			data[i] = new StateData( component.GetType(), JsonUtility.ToJson( component ) );
			DestroyImmediate( component );
		}
		return data;
	}

	private MonoBehaviour[] deserializeComponents( StateData[] data )
	{
		MonoBehaviour[] components = new MonoBehaviour[ data.Length ];
		for( int i = 0; i < data.Length; ++i )
		{
			StateData state = data[i];
			MonoBehaviour component = gameObject.AddComponent( state.type ) as MonoBehaviour;
			JsonUtility.FromJsonOverwrite( state.data, component );
			components[i] = component;
		}
		return components;
	}

	[Serializable]
	public struct StateConfig
	{
		public string name;
		public MonoBehaviour[] components;
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
