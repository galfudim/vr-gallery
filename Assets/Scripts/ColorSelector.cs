using System;
using UnityEngine;
using UnityEngine.UI;

// Authors: SnapshotGames, Gal Fudim
// SnapshotGames has released parts of this open-sources script on GitHub with an MIT license

public class ColorSelector : MonoBehaviour
{
	private Color myColor;
	private Color generatedColor;
	private Action<Color> onValueChange;
	private Action update;

	// Accessor and modifier methods for the color
	public Color Color { 
		get { 
			return generatedColor; 
		} 
		set { 
			createColor (value); 
		} 
	}

	// Sets onValueChange action to value of parameter
	public void setOnValueChangeCallback (Action<Color> onValueChangeIn)
	{
		onValueChange = onValueChangeIn;
	}

	// Creates an RGB color from the hue, saturation and value of the input.
	private static void RGBToHSV (Color color, out float hue, out float saturation, out float value)
	{
		var minValue = Mathf.Min (color.r, color.g, color.b);
		var maxValue = Mathf.Max (color.r, color.g, color.b);
		var distance = maxValue - minValue;
		if (distance == 0) {
			hue = 0;
		} else if (maxValue == color.r) {
			hue = Mathf.Repeat ((color.g - color.b) / distance, 6);
		} else if (maxValue == color.g) {
			hue = (color.b - color.r) / distance + 2;
		} else {
			hue = (color.r - color.g) / distance + 4;
		}
		saturation = maxValue == 0 ? 0 : distance / maxValue;
		value = maxValue;
	}

	// Returns a boolean that says whether the mouse is in the rectangular grid of the colorSelector
	private static bool getMousePosition (GameObject go, out Vector2 result)
	{
		var rectangleTransformation = (RectTransform)go.transform;
		var currentMousePosition = rectangleTransformation.InverseTransformPoint (Input.mousePosition);
		result.x = Mathf.Clamp (currentMousePosition.x, rectangleTransformation.rect.min.x, rectangleTransformation.rect.max.x);
		result.y = Mathf.Clamp (currentMousePosition.y, rectangleTransformation.rect.min.y, rectangleTransformation.rect.max.y);
		return rectangleTransformation.rect.Contains (currentMousePosition);
	}

	// Returns a 2D vector
	private static Vector2 getSize (GameObject go)
	{
		var rectangleTransformation = (RectTransform)go.transform;
		return rectangleTransformation.rect.size;
	}

	// Returns the game object by the parameter name
	private GameObject GO (string name)
	{
		return transform.Find (name).gameObject;
	}

	// Creates the color from the input color
	private void createColor (Color inputColor)
	{
		var saturationValueGO = GO ("SaturationValue");
		var saturationValueKnob = GO ("SaturationValue/Knob");
		var hueGO = GO ("Hue");
		var hueKnob = GO ("Hue/Knob");
		var result = GO ("Result");
		var hueColors = new Color [] {
			Color.red,
			Color.yellow,
			Color.green,
			Color.cyan,
			Color.blue,
			Color.magenta,
		};
		var saturationValueColors = new Color [] {
			new Color (0, 0, 0),
			new Color (0, 0, 0),
			new Color (1, 1, 1),
			hueColors [0],
		};
		var hueTexture = new Texture2D (1, 7);
		for (int i = 0; i < 7; i++) {
			hueTexture.SetPixel (0, i, hueColors [i % 6]);
		}
		hueTexture.Apply ();
		hueGO.GetComponent<Image> ().sprite = Sprite.Create (hueTexture, new Rect (0, 0.5f, 1, 6), new Vector2 (0.5f, 0.5f));
		var hueSize = getSize (hueGO);
		var saturationValueTexture = new Texture2D (2, 2);
		saturationValueGO.GetComponent<Image> ().sprite = Sprite.Create (saturationValueTexture, new Rect (0.5f, 0.5f, 1, 1), new Vector2 (0.5f, 0.5f));
		Action resetSaturationValueTexture = () => {
			for (int j = 0; j < 2; j++) {
				for (int i = 0; i < 2; i++) {
					saturationValueTexture.SetPixel (i, j, saturationValueColors [i + j * 2]);
				}
			}
			saturationValueTexture.Apply ();
		};
		var saturationValueSize = getSize (saturationValueGO);
		float Hue, Saturation, Value;
		RGBToHSV (inputColor, out Hue, out Saturation, out Value);
		Action applyHue = () => {
			var i0 = Mathf.Clamp ((int)Hue, 0, 5);
			var i1 = (i0 + 1) % 6;
			var resultColor = Color.Lerp (hueColors [i0], hueColors [i1], Hue - i0);
			saturationValueColors [3] = resultColor;
			resetSaturationValueTexture ();
		};
		Action applySaturationValue = () => {
			var sv = new Vector2 (Saturation, Value);
			var isv = new Vector2 (1 - sv.x, 1 - sv.y);
			var c0 = isv.x * isv.y * saturationValueColors [0];
			var c1 = sv.x * isv.y * saturationValueColors [1];
			var c2 = isv.x * sv.y * saturationValueColors [2];
			var c3 = sv.x * sv.y * saturationValueColors [3];
			var resultColor = c0 + c1 + c2 + c3;
			var resImg = result.GetComponent<Image> ();
			resImg.color = resultColor;
			if (generatedColor != resultColor) {
				if (onValueChange != null) {
					onValueChange (resultColor);
				}
				generatedColor = resultColor;
				myColor = new Color(resultColor.r, resultColor.g, resultColor.b);
			}
		};
		applyHue ();
		applySaturationValue ();
		saturationValueKnob.transform.localPosition = new Vector2 (Saturation * saturationValueSize.x, Value * saturationValueSize.y);
		hueKnob.transform.localPosition = new Vector2 (hueKnob.transform.localPosition.x, Hue / 6 * saturationValueSize.y);
		Action dragHue = null;
		Action dragSaturationValue = null;
		Action idle = () => {
			if (Input.GetMouseButtonDown (0)) {
				Vector2 currentMousePosition;
				if (getMousePosition (hueGO, out currentMousePosition)) {
					update = dragHue;
				} else if (getMousePosition (saturationValueGO, out currentMousePosition)) {
					update = dragSaturationValue;
				}
			}
		};
		dragHue = () => {
			Vector2 currentMousePosition;
			getMousePosition (hueGO, out currentMousePosition);
			Hue = currentMousePosition.y / hueSize.y * 6;
			applyHue ();
			applySaturationValue ();
			hueKnob.transform.localPosition = new Vector2 (hueKnob.transform.localPosition.x, currentMousePosition.y);
			if (Input.GetMouseButtonUp (0)) {
				update = idle;
			}
		};
		dragSaturationValue = () => {
			Vector2 currentMousePosition;
			getMousePosition (saturationValueGO, out currentMousePosition);
			Saturation = currentMousePosition.x / saturationValueSize.x;
			Value = currentMousePosition.y / saturationValueSize.y;
			applySaturationValue ();
			saturationValueKnob.transform.localPosition = currentMousePosition;
			if (Input.GetMouseButtonUp (0)) {
				update = idle;
			}
		};
		update = idle;
	}

	// Returns the color currently selected on the colorSelector 
	public Color getColor() {
		var r = myColor.r;
		var g = myColor.g;
		var b = myColor.b;
		return new Color(r, g, b);
	}

	// Initializes the color
	void Awake ()
	{
		Color = Color.red;
	}

	// Update is called once per frame
	void Update ()
	{
		update ();
	}
}