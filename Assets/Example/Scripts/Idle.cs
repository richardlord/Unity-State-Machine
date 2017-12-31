using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : MonoBehaviour
{
	[SerializeField] private Animator animator;

	void Start()
	{
		animator.SetTrigger( "Idle" );
	}
}
