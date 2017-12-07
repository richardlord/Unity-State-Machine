using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor( typeof( StateMachine ) )]
[CanEditMultipleObjects]
public class StateMachineEditor : Editor
{
	private const float buttonWidth = 20f;

	SerializedProperty firstStateProperty;
	SerializedProperty statesProperty;

	void OnEnable()
	{
		firstStateProperty = serializedObject.FindProperty( "firstState" );
		statesProperty = serializedObject.FindProperty( "states" );
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		string firstStateName = firstStateProperty.stringValue;
		int numberOfStates = statesProperty.arraySize;
		int selectedIndex = 0;
		string[] stateNames = new string[ numberOfStates + 1 ];
		stateNames[ 0 ] = "None";
		for( int i = 0; i < numberOfStates; ++i )
		{
			SerializedProperty stateProperty = statesProperty.GetArrayElementAtIndex( i );
			string stateName = stateProperty.FindPropertyRelative( "name" ).stringValue;
			stateNames[ i + 1 ] = stateName;
			if( stateName == firstStateName && stateName != "" )
			{
				selectedIndex = i + 1;
			}
		}
		int newIndex = EditorGUILayout.Popup( "Initial state", selectedIndex, stateNames);
		if( newIndex != selectedIndex )
		{
			if( newIndex == 0 )
			{
				firstStateProperty.stringValue = "";
			}
			else
			{
				firstStateProperty.stringValue = stateNames[ newIndex ];
			}
		}

		EditorGUILayout.Space();
		EditorGUILayout.LabelField( "States:" );
		++EditorGUI.indentLevel;
		float nameWidth = EditorGUIUtility.labelWidth - buttonWidth - 25;

		EditorGUILayout.BeginHorizontal();
		GUILayout.Space( buttonWidth + 3 );
		EditorGUILayout.LabelField( "Name", GUILayout.Width( nameWidth + 5 ) );
		EditorGUILayout.LabelField( "Components" );
		EditorGUILayout.EndHorizontal();

		for( int i = 0; i < numberOfStates; ++i )
		{
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();

			bool removeState = GUILayout.Button( "-", GUILayout.Width( buttonWidth ) );
			if( removeState )
			{
				statesProperty.DeleteArrayElementAtIndex( i );
				for( int k = i + 1; k < numberOfStates; ++k )
				{
					statesProperty.MoveArrayElement( k, k - 1 );
				}
				--numberOfStates;
				statesProperty.arraySize = numberOfStates;
				--i;
				continue;
			}
			
			SerializedProperty stateProperty = statesProperty.GetArrayElementAtIndex( i );
			string stateName = stateProperty.FindPropertyRelative( "name" ).stringValue;
			string newStateName = EditorGUILayout.TextField( stateName, GUILayout.Width( nameWidth ) );
			if( newStateName != stateName )
			{
				stateProperty.FindPropertyRelative( "name" ).stringValue = newStateName;
				if( firstStateProperty.stringValue == stateName && stateName != "" )
				{
					firstStateProperty.stringValue = newStateName;
				}
			}

			EditorGUILayout.BeginVertical();
			SerializedProperty componentsProperty = stateProperty.FindPropertyRelative( ( "components" ) );
			int numberOfComponents = componentsProperty.isArray ? componentsProperty.arraySize : 0;
			for( int j = 0; j < numberOfComponents; ++j )
			{
				EditorGUILayout.BeginHorizontal();
				Object component = componentsProperty.GetArrayElementAtIndex( j ).objectReferenceValue;
				Object updatedComponent = EditorGUILayout.ObjectField( component, typeof( MonoBehaviour ), true );
				if( updatedComponent != component )
				{
					componentsProperty.GetArrayElementAtIndex( j ).objectReferenceValue = updatedComponent;
				}
				bool removeComponent = GUILayout.Button( "-", GUILayout.Width( buttonWidth ) );
				if( removeComponent )
				{
					componentsProperty.DeleteArrayElementAtIndex( j );
					for( int k = j + 1; k < numberOfComponents; ++k )
					{
						componentsProperty.MoveArrayElement( k, k - 1 );
					}
					--numberOfComponents;
					componentsProperty.arraySize = numberOfComponents;
					--j;
				}
				EditorGUILayout.EndHorizontal();
			}

			if( numberOfComponents == 0 )
			{
				EditorGUILayout.BeginHorizontal();
				Object updatedComponent = EditorGUILayout.ObjectField( null, typeof( MonoBehaviour ), true );
				if( updatedComponent != null )
				{
					numberOfComponents = 1;
					componentsProperty.arraySize = 1;
					componentsProperty.GetArrayElementAtIndex( 0 ).objectReferenceValue = updatedComponent;
				}
				EditorGUILayout.EndHorizontal();
			}

			if( numberOfComponents > 0 )
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();
				bool addComponent = GUILayout.Button( "+", GUILayout.Width( buttonWidth ) );
				if( addComponent )
				{
					++numberOfComponents;
					componentsProperty.arraySize = numberOfComponents;
					componentsProperty.GetArrayElementAtIndex( numberOfComponents - 1 ).objectReferenceValue = null;
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.Space();
		EditorGUILayout.BeginHorizontal();
		bool addState = GUILayout.Button( "+", GUILayout.Width( buttonWidth ) );
		if( addState )
		{
			++numberOfStates;
			statesProperty.arraySize = numberOfStates;
			SerializedProperty stateProperty = statesProperty.GetArrayElementAtIndex( numberOfStates - 1 );
			stateProperty.FindPropertyRelative( "name" ).stringValue = "";
			stateProperty.FindPropertyRelative( "components" ).arraySize = 0;
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Space();

		--EditorGUI.indentLevel;

		//EditorGUILayout.PropertyField( firstStateProperty );
		//EditorGUILayout.PropertyField( statesProperty, true );

		serializedObject.ApplyModifiedProperties();
	}
}
