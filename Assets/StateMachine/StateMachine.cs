using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class StateMachine : MonoBehaviour
{
	public string firstState;
	public StateConfig[] states;

	private ActiveComponentData currentStateData;
	private string currentStateName;

	private Dictionary<string, int> statesDictionary;
	private Dictionary<int, ComponentData> componentsDictionary;

	public string currentState
	{
		get
		{
			return currentStateName;
		}
	}

	void Awake ()
	{
		statesDictionary = new Dictionary<string, int>( states.Length );
		foreach( StateConfig state in states )
		{
			statesDictionary[ state.name ] = state.component.GetInstanceID();
		}
		componentsDictionary = new Dictionary<int, ComponentData>();
		foreach( StateConfig state in states )
		{
			serializeComponent( state.component );
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
			if( currentStateData.component != null )
			{
				serializeComponent( currentStateData );
			}
				
			currentStateName = stateName;
			currentStateData = deserializeComponent( statesDictionary[ stateName ] );
		}
	}

	private int[] getComponentIds( MonoBehaviour[] components )
	{
		int[] ids = new int[ components.Length ];
		for( int i = 0; i < components.Length; ++i )
		{
			MonoBehaviour component = components[i];
			ids[i] = component.GetInstanceID();
		}
		return ids;
	}

	private void serializeComponent( MonoBehaviour component )
	{
		if( component != null )
		{
			componentsDictionary[component.GetInstanceID()] = new ComponentData( component.GetInstanceID(), component.GetType(), JsonUtility.ToJson( component ) );
			DestroyImmediate( component );
		}
	}

	private void serializeComponent( ActiveComponentData componentData )
	{
		if( componentData.component != null )
		{
			componentsDictionary[componentData.id] = new ComponentData( componentData.id, componentData.component.GetType(), JsonUtility.ToJson( componentData.component ) );
			DestroyImmediate( componentData.component );
		}
	}

	private ActiveComponentData deserializeComponent( int id )
	{
		ComponentData data = componentsDictionary[id];
		MonoBehaviour component = gameObject.AddComponent( data.type ) as MonoBehaviour;
		JsonUtility.FromJsonOverwrite( data.data, component );
		return new ActiveComponentData( id, component );
	}

	[Serializable]
	public struct StateConfig
	{
		public string name;
		public MonoBehaviour component;
	}

	struct ComponentData
	{
		public int id;
		public Type type;
		public string data;

		public ComponentData( int id, Type type, string data )
		{
			this.id = id;
			this.type = type;
			this.data = data;
		}
	}

	struct ActiveComponentData
	{
		public int id;
		public MonoBehaviour component;

		public ActiveComponentData( int id, MonoBehaviour component )
		{
			this.id = id;
			this.component = component;
		}
	}
}
