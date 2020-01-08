using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using System.IO;

// Author: Gal Fudim

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Paint : MonoBehaviour
{
	public GameObject brush;
	public float brushSize = 0.05f;
	public Slider brushSizeSlider;
	public RenderTexture RTexture;
	public ColorSelector colorSelector;
	string sceneName;

	// Gets the scene name
	void Start ()
	{
		Scene scene = SceneManager.GetActiveScene ();
		sceneName = scene.name;
	}

	// Update is called once per frame
	void Update ()
	{
		brushSize = brushSizeSlider.value; // Brush size is determined by value of UI slider
		if (Input.GetMouseButton (0)) { // Boolean determining if the mouse is being clicked
			var Ray = Camera.main.ScreenPointToRay (Input.mousePosition); // A ray from the camera to the mouse position
			RaycastHit hit; // Creating a RaycastHit
			if (Physics.Raycast (Ray, out hit)) { // Seeing if the ray from the camera is hitting
				// Initializing a brush stroke with the brush object and the location of the hit
				var brushStroke = Instantiate (brush, hit.point + Vector3.up * 0.1f, Quaternion.identity, transform);
				// Setting the color of each brush stroke to the current color on the colorSelector
				brushStroke.GetComponent<Renderer> ().material.color = colorSelector.getColor ();
				// Setting the size of the brush stroke to the size retrieved from the slider
				brushStroke.transform.localScale = Vector3.one * brushSize;
			}

		}
	}
		
	// Saves the current drawing
	public void Save ()
	{
		// Calls on the auxiliary saving method
		StartCoroutine (SaveAuxiliary ());
	}

	private IEnumerator SaveAuxiliary ()
	{
		yield return new WaitForEndOfFrame ();

		// Setting the render texture's state to active
		RenderTexture.active = RTexture;
		// Initializing a new 2D texture with the width and height of the render texture
		var texture2D = new Texture2D (RTexture.width, RTexture.height);
		// Reading and writing the pixels from the render texture to the 2D texture
		texture2D.ReadPixels (new Rect (0, 0, RTexture.width, RTexture.height), 0, 0);
		texture2D.Apply ();
		// Converting the pixels to those of a .png image file
		var data = texture2D.EncodeToPNG ();
		// Writing the pixels to an image
		string imageName = "/savedImage" + sceneName + ".png";
		File.WriteAllBytes (Application.dataPath + imageName, data);
		#if UNITY_EDITOR
		// Updating the image at that reference with the new saved image
		AssetDatabase.Refresh ();
		#endif
		Application.Quit ();
	}
}
