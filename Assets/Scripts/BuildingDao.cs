using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingDao {
	
	public string name;
	public string icon;
	public string model;
	public List<UnitDao> units;
	public List<TechnologyDao> technologies;
	public Dictionary<BaseMaterialDao, int> cost;
	public Vector2 size;
	public float lifeTotal;

	public int visionField;

	public List<string> requireds;

	public BaseMaterialDao producedMaterial;
	public int producedPerTime;

	public float developTime;
	public bool constructed;

	public BuildingDao() {}

	public BuildingDao(string name, string icon, string model, Vector2 size, float lifeTotal, float developTime, bool constructed, int visionField, BaseMaterialDao producedMaterial = null, int producedPerTime = 0) {
		this.name = name;
		this.icon = icon;
		this.model = model;
		this.units = new List<UnitDao>();
		this.technologies = new List<TechnologyDao>();
		this.cost = new Dictionary<BaseMaterialDao, int>();
		this.size = size;
		this.lifeTotal = lifeTotal;
		this.developTime = developTime;
		this.constructed = constructed;
		this.visionField = visionField;
		this.producedMaterial = producedMaterial;
		this.producedPerTime = producedPerTime;
		this.requireds = new List<string>();
	}

	public Building Instantiate() {
        
		Dictionary<BaseMaterial, int> costBaseMaterial = new Dictionary<BaseMaterial, int>();

		foreach(BaseMaterialDao baseMaterialDao in cost.Keys) {
			costBaseMaterial.Add(baseMaterialDao.Instantiate(), cost[baseMaterialDao]);
		}

		BaseMaterial producedBaseMaterial = null;

		if (producedMaterial != null) {
			producedBaseMaterial = producedMaterial.Instantiate();
		}

		return new Building(name, Resources.Load(icon, typeof(Sprite)) as Sprite, Resources.Load(model, typeof(GameObject)) as GameObject, units, technologies, costBaseMaterial, size, lifeTotal, developTime, constructed, visionField, requireds, producedBaseMaterial, producedPerTime);
	}

}
