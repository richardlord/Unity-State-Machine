using UnityEngine;
using System;
using System.Collections.Generic;

public class StateMachine : MonoBehaviour
{
	public string firstState;
	public StateConfig[] states;

	private List<ActiveComponentData> currentStateComponents;
	private string currentStateName;

	private Dictionary<string, int[]> statesDictionary;
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
		statesDictionary = new Dictionary<string, int[]>( states.Length );
		foreach( StateConfig state in states )
		{
			int[] ids = new int[ state.components.Length ];
			for( int i = 0; i < state.components.Length; ++i )
			{
				ids[i] = state.components[i].GetInstanceID();
			}
			statesDictionary[ state.name ] = ids;
		}
		componentsDictionary = new Dictionary<int, ComponentData>();
		foreach( StateConfig state in states )
		{
			SerializeComponents( state.components );
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
			List<int> newStateIds = new List<int>( statesDictionary[ stateName ] );
			List<ActiveComponentData> newStateComponents = new List<ActiveComponentData>( newStateIds.Count );

			if( currentStateComponents != null )
			{
				for( int i = currentStateComponents.Count - 1; i >= 0; --i )
				{
					if( newStateIds.Contains( currentStateComponents[i].id ) )
					{
						newStateComponents.Add( currentStateComponents[i] );
						newStateIds.Remove( currentStateComponents[i].id );
						currentStateComponents.RemoveAt( i );
					}
				}

				SerializeComponents( currentStateComponents );
			}

			currentStateName = stateName;
			DeserializeComponents( newStateIds, newStateComponents );
			currentStateComponents = newStateComponents;
		}
	}

	private void SerializeComponents( MonoBehaviour[] components )
	{
		for( int i = 0; i < components.Length; ++i )
		{
			MonoBehaviour component = components[i];
			if( component != null )
			{
				componentsDictionary[component.GetInstanceID()] = new ComponentData( component.GetInstanceID(), component.GetType(), JsonUtility.ToJson( component ), component.gameObject );
				Destroy( component );
			}
		}
	}

	private void SerializeComponents( List<ActiveComponentData> components )
	{
		for( int i = 0; i < components.Count; ++i )
		{
			ActiveComponentData data = components[i];
			if( data.component != null )
			{
				componentsDictionary[data.id] = new ComponentData( data.id, data.component.GetType(), JsonUtility.ToJson( data.component ), data.component.gameObject );
				Destroy( data.component );
			}
		}
	}

	private void DeserializeComponents( List<int> ids, List<ActiveComponentData> store )
	{
		for( int i = 0; i < ids.Count; ++i )
		{
			int id = ids[i];
			ComponentData data = componentsDictionary[id];
			MonoBehaviour component = data.gameObject.AddComponent( data.type ) as MonoBehaviour;
			JsonUtility.FromJsonOverwrite( data.data, component );
			store.Add( new ActiveComponentData( id, component ) );
		}
	}

	[Serializable]
	public class StateConfig
	{
		public string name;
		public MonoBehaviour[] components;
	}

	class ComponentData
	{
		public int id;
		public Type type;
		public string data;
		public GameObject gameObject;

		public ComponentData( int id, Type type, string data, GameObject gameObject )
		{
			this.id = id;
			this.type = type;
			this.data = data;
			this.gameObject = gameObject;
		}
	}

	class ActiveComponentData
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
