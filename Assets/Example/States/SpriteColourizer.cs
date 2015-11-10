using UnityEngine;
using System.Collections;

public class SpriteColourizer : MonoBehaviour
{
	public Color color;

	void Start ()
	{
		GetComponent<SpriteRenderer>().color = color;
	}
}
