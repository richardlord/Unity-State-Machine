using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Helper class for instantiating ScriptableObjects.
/// </summary>
public class StateFactory
{
	[MenuItem("Assets/Create/State")]
	public static void Create()
	{
		var assembly = GetAssembly ();
		
		// Get all classes derived from ScriptableObject
		var allStateClasses = (from t in assembly.GetTypes()
		                            where t.IsSubclassOf(typeof(State))
		                            select t).ToArray();
		
		// Show the selection window.
		var window = EditorWindow.GetWindow<StateWindow>(true, "Create a new State", true);
		window.ShowPopup();
		
		window.Types = allStateClasses;
	}
	
	/// <summary>
	/// Returns the assembly that contains the script code for this project (currently hard coded)
	/// </summary>
	private static Assembly GetAssembly ()
	{
		return Assembly.Load (new AssemblyName ("Assembly-CSharp"));
	}
}