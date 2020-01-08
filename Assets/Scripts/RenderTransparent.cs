using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Gal Fudim

public class RenderTransparent : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		// Makes the object "invisible"
		gameObject.GetComponent<Renderer> ().enabled = false;
	}
}
