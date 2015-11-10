using UnityEngine;
using System.Collections;

public class Slide : MonoBehaviour
{
	public float speed;
	public float limit;

	void Update ()
	{
		Vector3 position = transform.position;
		position.x += speed * Time.deltaTime;
		transform.position = position;
		if( position.x >= limit || position.x <= -limit )
		{
			speed = -speed;
		}
	}
}
