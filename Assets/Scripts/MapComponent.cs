using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapComponent {

	public string name;
	public GameObject model;
	public Vector2 position;

	public MapComponent() {}

	public MapComponent(string name, GameObject model) {
		this.name = name;
		this.model = model;
	}

}
