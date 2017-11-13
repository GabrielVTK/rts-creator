using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile {

	public string name;
	public GameObject model;
	public bool isWalkable;
	public bool canBuild;
	public float terraineType;
	public MapComponent mapComponent;

	public Tile() {}

	public Tile(string name, GameObject model, bool isWalkable, bool canBuild, float terraineType) {
		this.name = name;
		this.model = model;
		this.isWalkable = isWalkable;
		this.canBuild = canBuild;
		this.terraineType = terraineType;
	}

}
