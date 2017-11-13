using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechnologyDao {

	public string name;
	public float developTime;
	public string icon;

	public TechnologyDao() {}

	public TechnologyDao(string name, string icon, float developTime) {
		this.name = name;
		this.icon = icon;
		this.developTime = developTime;
	}

	public Technology Instantiate() {
		return new Technology(name, Resources.Load(icon, typeof(Sprite)) as Sprite, developTime);
	}

}
