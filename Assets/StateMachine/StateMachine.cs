using UnityEngine;
using System;
using System.Linq;
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
			statesDictionary[ state.name ] = getComponentIds( state.components );
		}
		componentsDictionary = new Dictionary<int, ComponentData>();
		foreach( StateConfig state in states )
		{
			serializeComponents( state.components );
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
			List<int> newStateIds = statesDictionary[ stateName ].ToList();
			List<ActiveComponentData> newStateComponents = new List<ActiveComponentData>();

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

				serializeComponents( currentStateComponents );
			}
				
			currentStateName = stateName;
			deserializeComponents( newStateIds, newStateComponents );
			currentStateComponents = newStateComponents;
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

	private void serializeComponents( MonoBehaviour[] components )
	{
		for( int i = 0; i < components.Length; ++i )
		{
			MonoBehaviour component = components[i];
			if( component != null )
			{
				componentsDictionary[component.GetInstanceID()] = new ComponentData( component.GetInstanceID(), component.GetType(), JsonUtility.ToJson( component ) );
				DestroyImmediate( component );
			}
		}
	}

	private void serializeComponents( List<ActiveComponentData> components )
	{
		for( int i = 0; i < components.Count; ++i )
		{
			ActiveComponentData data = components[i];
			if( data.component != null )
			{
				componentsDictionary[data.id] = new ComponentData( data.id, data.component.GetType(), JsonUtility.ToJson( data.component ) );
				DestroyImmediate( data.component );
			}
		}
	}

	private void deserializeComponents( List<int> ids, List<ActiveComponentData> store )
	{
		for( int i = 0; i < ids.Count; ++i )
		{
			int id = ids[i];
			ComponentData data = componentsDictionary[id];
			MonoBehaviour component = gameObject.AddComponent( data.type ) as MonoBehaviour;
			JsonUtility.FromJsonOverwrite( data.data, component );
			store.Add( new ActiveComponentData( id, component ) );
		}
	}

	[Serializable]
	public struct StateConfig
	{
		public string name;
		public MonoBehaviour[] components;
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
