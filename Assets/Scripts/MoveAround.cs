using UnityEngine;
using System.Collections;

// Author: Gal Fudim

public class MoveAround : MonoBehaviour
{
	public float speed = 0.1f;

	// Update is called once per frame
	public void Update()
	{
		// Moves the current object that invokes this script in the direction that is being passed 
		// by the keyboard
		var rate = 50 * Input.GetAxis ("Horizontal") * Time.deltaTime;
		transform.Rotate (0, rate, 0 );
		// Moves right
		if(Input.GetKey(KeyCode.RightArrow))
		{
			transform.position = new Vector3(transform.position.x + speed, transform.position.y, transform.position.z);
		}
		// Moves left
		if(Input.GetKey(KeyCode.LeftArrow))
		{
			transform.position = new Vector3(transform.position.x - speed, transform.position.y, transform.position.z);
		}
		// Moves down
		if(Input.GetKey(KeyCode.DownArrow))
		{
			transform.position = new Vector3(transform.position.x, transform.position.y - speed, transform.position.z);
		}
		// Moves up
		if(Input.GetKey(KeyCode.UpArrow))
		{
			transform.position = new Vector3(transform.position.x, transform.position.y + speed, transform.position.z);
		}
	}
}