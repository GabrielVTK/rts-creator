using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDao {

	public string name;
	public string model;
	public bool isWalkable;
	public bool canBuild;
	public float terraineType;

	public TileDao() {}

	public TileDao(string name, string model, bool isWalkable, bool canBuild, float terraineType) {
		this.name = name;
		this.model = model;
		this.isWalkable = isWalkable;
		this.canBuild = canBuild;
		this.terraineType = terraineType;
	}

	public Tile Instantiate() {
		return new Tile(name, Resources.Load(model, typeof(GameObject)) as GameObject, isWalkable, canBuild, terraineType);
	}

}
