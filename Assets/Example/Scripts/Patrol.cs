using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{
	private const float blendAdjustSpeed = 2f;
	private const float turnStopAtDotValue = 0.95f;
	private const float changeTargetAtTargetDistance = 0.1f;
	private const float newTargetMinimumDistance = 1f;

	[SerializeField] private Animator animator;
	[SerializeField] private Vector3[] points;
	[SerializeField] private int currentPointIndex = 0;

	float blend;
	float blendTarget;

	void Start ()
	{
		Debug.Assert( points.Length >= 2 );

		animator.SetTrigger( "Walk" );
		blend = 0;
		blendTarget = 0;
		animator.SetFloat( "WalkBlend", blend );
		RotateToTarget();
	}

	void RotateToTarget()
	{
		Vector3 toTarget = ( points[ currentPointIndex ] - transform.position );
		if( toTarget.magnitude < newTargetMinimumDistance )
		{
			NextTarget();
			return;
		}
		if( Vector3.Dot( toTarget, transform.forward ) >= turnStopAtDotValue )
		{
			WalkToTarget();
			return;
		}
		bool turnRight = Vector3.Dot( toTarget, transform.right ) > 0;
		if( turnRight )
		{
			blendTarget = 1;
		}
		else
		{
			blendTarget = -1;
		}
	}

	void NextTarget()
	{
		++currentPointIndex;
		if( currentPointIndex >= points.Length )
		{
			currentPointIndex = 0;
		}
		RotateToTarget();
	}

	void WalkToTarget()
	{
		Vector3 toTarget = ( points[ currentPointIndex ] - transform.position );
		transform.rotation = Quaternion.LookRotation( toTarget, Vector3.up );
		blendTarget = 0;
	}

	void Update()
	{
		if( !blendTarget.Equals( 0 ) )
		{
			Vector3 toTarget = ( points[ currentPointIndex ] - transform.position ).normalized;
			if( Vector3.Dot( toTarget, transform.forward ) >= turnStopAtDotValue )
			{
				WalkToTarget();
			}
		}
		else if( Vector3.Distance( points[ currentPointIndex ], transform.position ) < changeTargetAtTargetDistance )
		{
			NextTarget();
		}
		else
		{
			Vector3 toTarget = ( points[ currentPointIndex ] - transform.position );
			transform.rotation = Quaternion.LookRotation( toTarget, Vector3.up );
		}

		if( !blend.Equals( blendTarget ) )
		{
			float toTarget = blendTarget - blend;
			float changeAmount = blendAdjustSpeed * Time.deltaTime;
			if( toTarget < 0 )
			{
				blend -= changeAmount;
				if( blend < blendTarget )
				{
					blend = blendTarget;
				}
			}
			else
			{
				blend += changeAmount;
				if( blend > blendTarget )
				{
					blend = blendTarget;
				}
			}
			animator.SetFloat( "WalkBlend", blend );
		}
	}
}
