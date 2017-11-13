using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util {

	public static void ResetGameObject(GameObject gameObject, float x, float y) {

		gameObject.GetComponent<RectTransform> ().eulerAngles = new Vector3 (90.0f, 0.0f, 0.0f);
		gameObject.GetComponent<RectTransform> ().anchorMin = new Vector2 (x, y);
		gameObject.GetComponent<RectTransform> ().anchorMax = new Vector2 (x, y);
		gameObject.GetComponent<RectTransform> ().pivot = new Vector2 (x, y);
		gameObject.GetComponent<RectTransform> ().localScale = new Vector3 (1.0f, 1.0f, 1.0f);

	}

}
