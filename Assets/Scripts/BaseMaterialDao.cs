using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMaterialDao {

	public string name;
	public string icon;

	public BaseMaterialDao() {}

	public BaseMaterialDao(string name, string icon) {
		this.name = name;
		this.icon = icon;
	}

	public BaseMaterial Instantiate() {
		BaseMaterial.InstantiateBaseMaterial(name, Resources.Load (icon, typeof(Sprite)) as Sprite);

		return BaseMaterial.GetInstance(name);
	}
}
