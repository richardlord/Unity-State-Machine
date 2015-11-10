using UnityEngine;
using System.Collections;

public class RigidBodyDisabler : MonoBehaviour
{
	Vector2 velocity;

	void Start()
	{
		velocity = GetComponent<Rigidbody2D>().velocity;
		GetComponent<Rigidbody2D>().Sleep();
	}

	void OnDestroy()
	{
		GetComponent<Rigidbody2D>().WakeUp();
		GetComponent<Rigidbody2D>().velocity = velocity;
	}
}
