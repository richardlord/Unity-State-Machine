using UnityEngine;
using System;
using System.Collections.Generic;

public class StateMachine : MonoBehaviour
{
	public string firstState;
	public StateConfig[] states;

	private MonoBehaviour currentStateComponent;
	private string currentStateName;

	private Dictionary<string, ComponentData> statesDictionary;

	public string currentState
	{
		get
		{
			return currentStateName;
		}
	}

	void Awake ()
	{
		statesDictionary = new Dictionary<string, ComponentData>( states.Length );
		foreach( StateConfig state in states )
		{
			statesDictionary[ state.name ] = SerializeComponent( state.component );
		}
		if( firstState != null )
		{
			ChangeState( firstState );
		}
	}

	public void ChangeState( string stateName )
	{
		if( stateName != currentStateName && statesDictionary.ContainsKey( stateName ) )
		{
			if( currentStateComponent != null )
			{
				statesDictionary[ currentStateName ] = SerializeComponent( currentStateComponent );
			}
				
			currentStateName = stateName;
			currentStateComponent = DeserializeComponent( statesDictionary[stateName] );
		}
	}

	private ComponentData SerializeComponent( MonoBehaviour component )
	{
		if( component != null )
		{
			ComponentData componentData = new ComponentData( component.GetType(), JsonUtility.ToJson( component ) );
			Destroy( component );
			return componentData;
		}
		return null;
	}

	private MonoBehaviour DeserializeComponent( ComponentData data )
	{
		if( data != null )
		{
			MonoBehaviour component = gameObject.AddComponent( data.type ) as MonoBehaviour;
			JsonUtility.FromJsonOverwrite( data.data, component );
			return component;
		}
		return null;
	}

	[Serializable]
	public struct StateConfig
	{
		public string name;
		public MonoBehaviour component;
	}

	class ComponentData
	{
		public Type type;
		public string data;

		public ComponentData( Type type, string data )
		{
			this.type = type;
			this.data = data;
		}
	}
}
