using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingUpgrade : Technology {

	private Building building;

    public BuildingUpgrade(string name, Sprite icon, float developTime) : base(name, icon, developTime) { }

	public void SetBuilding(Building building) {
		this.building = building;
	}

	public Building GetBuilding() {
		return building;
	}

}
