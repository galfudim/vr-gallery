using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

// Author: Gal Fudim

public class SceneSwitch : MonoBehaviour {

	public string sceneName;

	// Switches to another scene based on the parameter name of the scene
	public void SwitchScene(string SceneName) {
		#if UNITY_EDITOR
		EditorSceneManager.LoadScene (SceneName);
		#endif
	}

	// Switches to another scene based on the global variable name of the scene
	// Is invoked if the player object collides with the object that has this script
	void OnTriggerEnter (Collider cubeTrigger)
	{    
		if (cubeTrigger.tag == "Player")
		{
			#if UNITY_EDITOR
			EditorSceneManager.LoadScene (sceneName); 
			#endif
		}
	}
}